using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(menuName = "Overdrive/Game/Era Waves")]
public class EraWaves : ScriptableObject {
    [ListDrawerSettings(NumberOfItemsPerPage = 1)]
    public EnemyWave[] Waves;
}

[System.Serializable]
public class EnemyWave {
    public bool Runout {
        get{
            bool runout = true;
            foreach(SpawnerSpawn spawn in EnemiesInWave) {
                if(spawn.Count > 0) {
                    runout = false;
                    break;
                }
            }

            return runout;
        }
    }

    public int MinActiveSpawners;

    [MinMaxSlider(0f, "MaxSpawnerDelayTime"), HorizontalGroup("SpawnerDelay"), LabelWidth(130f)]
    public Vector2 SpawnerDelayTime;
    [HideLabel, HorizontalGroup("SpawnerDelay"), SuffixLabel("Max Delay", true), LabelWidth(1f)]
    public float MaxSpawnerDelayTime;


    [MinMaxSlider(0f, "MaxBaiterSpawnTime"), HorizontalGroup("BaiterSpawn"), LabelWidth(110f)]
    public Vector2 BaiterSpawnTime;
    [HideLabel, HorizontalGroup("BaiterSpawn"), SuffixLabel("Max Time", true), LabelWidth(1f)]
    public float MaxBaiterSpawnTime;

    [HideLabel, EnumToggleButtons]
    public WaveSpawnerPresetType Type;
    [ShowIf("@Type == WaveSpawnerPresetType.Presets")]
    public SpawnerPreset[] PossiblePresets;
    [ShowIf("@Type == WaveSpawnerPresetType.Enemies"), ListDrawerSettings(NumberOfItemsPerPage = 1)]
    public SpawnerSpawn[] EnemiesInWave;
}

[System.Serializable]
public struct SpawnerSpawn {
    [AssetsOnly, LabelText("Enemy Prefab")]
    public GameObject Spawn;
    public int Count;

    public static bool operator ==(SpawnerSpawn lhs, SpawnerSpawn rhs) => lhs.Spawn == rhs.Spawn;
    public static bool operator !=(SpawnerSpawn lhs, SpawnerSpawn rhs) => lhs.Spawn != rhs.Spawn;

    public override bool Equals(object obj) {
        return base.Equals(obj);
    }

    public override int GetHashCode() {
        return base.GetHashCode();
    }

    public override string ToString() {
        return $"{Spawn.name}:{Count}";
    }
}

public enum WaveSpawnerPresetType {
    Presets,
    Enemies
}