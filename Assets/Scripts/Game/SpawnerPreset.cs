using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(menuName = "Overdrive/Data/Spawner Preset")]
public class SpawnerPreset : ScriptableObject {
    public SpawnerSpawn[] Spawns;
}
