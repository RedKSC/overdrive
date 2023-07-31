//
// ShaderGraphEssentials for Unity
// (c) 2019 PH Graphics
// Source code may be used and modified for personal or commercial projects.
// Source code may NOT be redistributed or sold.
// 
// *** A NOTE ABOUT PIRACY ***
// 
// If you got this asset from a pirate site, please consider buying it from the Unity asset store. This asset is only legally available from the Unity Asset Store.
// 
// I'm a single indie dev supporting my family by spending hundreds and thousands of hours on this and other assets. It's very offensive, rude and just plain evil to steal when I (and many others) put so much hard work into the software.
// 
// Thank you.
//
// *** END NOTE ABOUT PIRACY ***
//

using System;
using System.Collections.Generic;
using System.Linq;
using ShaderGraphEssentials.Legacy;
using UnityEditor;
using UnityEditor.Rendering.Universal.ShaderGraph;
using UnityEditor.ShaderGraph;
using UnityEditor.ShaderGraph.Internal;
using UnityEditor.ShaderGraph.Legacy;
using UnityEditor.ShaderGraph.Serialization;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UIElements;
using static ShaderGraphEssentials.SGESubShaderUtils;
using static ShaderGraphEssentials.SGEShaderUtils;

namespace ShaderGraphEssentials
{
   sealed class SGEUniversalUnlitSubTarget : SGEUniversalSubTarget, ILegacyTarget
    {
        static readonly GUID kSourceCodeGuid = new GUID("d3ebbd9754eb42746a066ace2a731da6"); // SGEUniversalUnlitSubTarget.cs

        public SGEUniversalUnlitSubTarget()
        {
            displayName = "SGE Unlit";
        }
        
        protected override SGEShaderID shaderID => SGEShaderID.SGE_Unlit;

        public override bool IsActive() => true;

        public override void Setup(ref TargetSetupContext context)
        {
            context.AddAssetDependency(kSourceCodeGuid, AssetCollection.Flags.SourceDependency);

            base.Setup(ref context);
            
            // Process SubShaders
            context.AddSubShader(PostProcessSubShader(SubShaders.UnlitDOTS(target, target.renderType.ToString(), target.renderQueue.ToString())));
            context.AddSubShader(PostProcessSubShader(SubShaders.Unlit(target, target.renderType.ToString(), target.renderQueue.ToString())));
        }
        
        public override void ProcessPreviewMaterial(Material material)
        {
        }

        public override void GetFields(ref TargetFieldContext context)
        {
            base.GetFields(ref context);
        }

        public override void GetActiveBlocks(ref TargetActiveBlockContext context)
        {
        }
        
        public override void CollectShaderProperties(PropertyCollector collector, GenerationMode generationMode)
        {
        }

        public override void GetPropertiesGUI(ref TargetPropertyGUIContext context, Action onChange, Action<string> registerUndo)
        {
        }

        public bool TryUpgradeFromMasterNode(IMasterNode1 masterNode, out Dictionary<BlockFieldDescriptor, int> blockMap)
        {
            blockMap = null;
            if(!(masterNode is SGEUnlitMasterNode1 sgeUnlitMasterNode))
                return false;

            // Set blockmap
            blockMap = new Dictionary<BlockFieldDescriptor, int>()
            {
                { BlockFields.VertexDescription.Position, 9 },
                { BlockFields.VertexDescription.Normal, 10 },
                { BlockFields.VertexDescription.Tangent, 11 },
                { BlockFields.SurfaceDescription.BaseColor, 0 },
                { BlockFields.SurfaceDescription.Alpha, 7 },
                { BlockFields.SurfaceDescription.AlphaClipThreshold, 8 },
            };

            return true;
        }

#region SubShader
        static class SubShaders
        {
            public static SubShaderDescriptor Unlit(SGEUniversalTarget target, string renderType, string renderQueue)
            {
                var result = new SubShaderDescriptor()
                {
                    pipelineTag = UniversalTarget.kPipelineTag,
                    customTags = UniversalTarget.kUnlitMaterialTypeTag,
                    renderType = renderType,
                    renderQueue = renderQueue,
                    generatesPreview = true,
                    passes = new PassCollection()
                };

                result.passes.Add(UnlitPasses.Forward(target));

                if (target.zWrite == ZWrite.On)
                    result.passes.Add(CorePasses.DepthOnly(target));
                
                result.passes.Add(CorePasses.DepthNormalOnly(target));

                if (target.castShadows)
                    result.passes.Add(CorePasses.ShadowCaster(target));
                
                // Currently neither of these passes (selection/picking) can be last for the game view for
                // UI shaders to render correctly. Verify [1352225] before changing this order.
                result.passes.Add(CorePasses.SceneSelection(target));
                result.passes.Add(CorePasses.ScenePicking(target));

                result.passes.Add(UnlitPasses.DepthNormalOnly(target));

                return result;
            }

            public static SubShaderDescriptor UnlitDOTS(SGEUniversalTarget target, string renderType, string renderQueue)
            {
                var result = new SubShaderDescriptor()
                {
                    pipelineTag = UniversalTarget.kPipelineTag,
                    customTags = UniversalTarget.kUnlitMaterialTypeTag,
                    renderType = renderType,
                    renderQueue = renderQueue,
                    generatesPreview = true,
                    passes = new PassCollection()
                };

                result.passes.Add(PassVariant(UnlitPasses.Forward(target), CorePragmas.DOTSForward));

                if (target.zWrite == ZWrite.On)
                    result.passes.Add(PassVariant(CorePasses.DepthOnly(target), CorePragmas.DOTSInstanced));
                
                result.passes.Add(PassVariant(CorePasses.DepthNormalOnly(target), CorePragmas.DOTSInstanced));

                if (target.castShadows)
                    result.passes.Add(PassVariant(CorePasses.ShadowCaster(target), CorePragmas.DOTSInstanced));
                
                // Currently neither of these passes (selection/picking) can be last for the game view for
                // UI shaders to render correctly. Verify [1352225] before changing this order.
                result.passes.Add(PassVariant(CorePasses.SceneSelection(target), CorePragmas.DOTSDefault));
                result.passes.Add(PassVariant(CorePasses.ScenePicking(target), CorePragmas.DOTSDefault));

                result.passes.Add(PassVariant(UnlitPasses.DepthNormalOnly(target), CorePragmas.DOTSInstanced));

                return result;
            }
        }
#endregion

#region Pass
        static class UnlitPasses
        {
            public static PassDescriptor Forward(SGEUniversalTarget target)
            {
                var result = new PassDescriptor
                {
                    // Definition
                    displayName = "SGE Universal Forward",
                    referenceName = "SHADERPASS_UNLIT",
                    useInPreview = true,

                    // Template
                    passTemplatePath = UniversalTarget.kUberTemplatePath,
                    sharedTemplateDirectories = UniversalTarget.kSharedTemplateDirectories,

                    // Port Mask
                    validVertexBlocks = CoreBlockMasks.Vertex,
                    validPixelBlocks = CoreBlockMasks.FragmentColorAlpha,

                    // Fields
                    structs = CoreStructCollections.Default,
                    requiredFields = UnlitRequiredFields.Unlit,
                    fieldDependencies = CoreFieldDependencies.Default,

                    // Conditional State
                    renderStates = CoreRenderStates.UberSwitchedRenderState(target),
                    pragmas = CorePragmas.Forward,
                    defines = new DefineCollection() { CoreDefines.UseFragmentFog },
                    keywords = new KeywordCollection() { UnlitKeywords.UnlitBaseKeywords },
                    includes = UnlitIncludes.Unlit,

                    // Custom Interpolator Support
                    customInterpolators = CoreCustomInterpDescriptors.Common
                };

                CorePasses.AddTargetSurfaceControlsToPass(ref result, target);

                return result;
            }
            
            public static PassDescriptor DepthNormalOnly(SGEUniversalTarget target)
            {
                var result = new PassDescriptor
                {
                    // Definition
                    displayName = "SGE DepthNormals",
                    referenceName = "SHADERPASS_DEPTHNORMALSONLY",
                    lightMode = "DepthNormalsOnly",
                    useInPreview = false,

                    // Template
                    passTemplatePath = UniversalTarget.kUberTemplatePath,
                    sharedTemplateDirectories = UniversalTarget.kSharedTemplateDirectories,

                    // Port Mask
                    validVertexBlocks = CoreBlockMasks.Vertex,
                    validPixelBlocks = UnlitBlockMasks.FragmentDepthNormals,

                    // Fields
                    structs = CoreStructCollections.Default,
                    requiredFields = UnlitRequiredFields.DepthNormalsOnly,
                    fieldDependencies = CoreFieldDependencies.Default,

                    // Conditional State
                    renderStates = CoreRenderStates.DepthNormalsOnly(target),
                    pragmas = CorePragmas.Forward,
                    defines = new DefineCollection(),
                    keywords = new KeywordCollection(),
                    includes = CoreIncludes.DepthNormalsOnly,

                    // Custom Interpolator Support
                    customInterpolators = CoreCustomInterpDescriptors.Common
                };

                CorePasses.AddTargetSurfaceControlsToPass(ref result, target);

                return result;
            }
            
            #region PortMasks
            static class UnlitBlockMasks
            {
                public static readonly BlockFieldDescriptor[] FragmentDepthNormals = new BlockFieldDescriptor[]
                {
                    BlockFields.SurfaceDescription.NormalWS,
                    BlockFields.SurfaceDescription.Alpha,
                    BlockFields.SurfaceDescription.AlphaClipThreshold,
                };
            }
            #endregion

            
            #region RequiredFields
            static class UnlitRequiredFields
            {
                public static readonly FieldCollection Unlit = new FieldCollection()
                {
                    StructFields.Varyings.positionWS,
                    StructFields.Varyings.normalWS,
                    StructFields.Varyings.viewDirectionWS,
                };
                
                public static readonly FieldCollection DepthNormalsOnly = new FieldCollection()
                {
                    StructFields.Varyings.normalWS,
                };
            }
            #endregion
        }
#endregion

#region Keywords
        static class UnlitKeywords
        {
            public static readonly KeywordCollection UnlitBaseKeywords = new KeywordCollection()
            {
                // This contain lightmaps because without a proper custom lighting solution in Shadergraph,
                // people start with the unlit then add lightmapping nodes to it.
                // If we removed lightmaps from the unlit target this would ruin a lot of peoples days.
                CoreKeywordDescriptors.StaticLightmap,
                CoreKeywordDescriptors.DirectionalLightmapCombined,
                CoreKeywordDescriptors.SampleGI,
                CoreKeywordDescriptors.DBuffer,
                CoreKeywordDescriptors.DebugDisplay,
            };
        }
#endregion

#region Includes
        static class UnlitIncludes
        {
            const string kUnlitPass = "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/UnlitPass.hlsl";

            public static IncludeCollection Unlit = new IncludeCollection
            {
                // Pre-graph
                { CoreIncludes.CorePregraph },
                { CoreIncludes.ShaderGraphPregraph },
                { CoreIncludes.DBufferPregraph },

                // Post-graph
                { CoreIncludes.CorePostgraph },
                { kUnlitPass, IncludeLocation.Postgraph },
            };
        }
#endregion
    }
}