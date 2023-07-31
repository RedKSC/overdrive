using System.Runtime.InteropServices.WindowsRuntime;
using System.Reflection.Emit;
using Microsoft.Win32.SafeHandles;
using System.Runtime.InteropServices;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.SceneManagement;

public class WaveHandler : SerializedMonoBehaviour {
    #region DebugViewing
    [FoldoutGroup("Debug"), LabelText("Wave"), SerializeField]
    public int Debug_Wave{
        get{
            return Wave;
        }
        set{
            Wave = value;
        }
    }

    [FoldoutGroup("Debug"), LabelText("Spawner Count"), ReadOnly, SerializeField]
    public int Debug_Spawners{
        get{
            return activeSpawners;
        }
        set{
            activeSpawners = value;
        }
    }
    #endregion

    [AssetsOnly]
    public Spawner SpawnerPrefab;

    [FoldoutGroup("Debug")]
    public int Loop;
    public int Wave {
        get{
            return currentWave;
        }
        set{
            currentWave = value;
            CurrentWave = null;
        }
    }
    int currentWave;

    float timeSinceLastSpawner;

    EraWaves[] Waves;
    EraWaves GetCurrentLoop => Waves[Loop.Clamp(0, Waves.Length - 1)];
    EnemyWave CurrentWave {
        get{
            if(wave == null) {
                Debug.Log("Creating deep copy of current wave...");
                EnemyWave currentWave = GetCurrentLoop.Waves[Wave-1];

                wave = new EnemyWave();

                wave.EnemiesInWave = new SpawnerSpawn[currentWave.EnemiesInWave.Length];
                currentWave.EnemiesInWave.CopyTo(wave.EnemiesInWave, 0);

                wave.PossiblePresets = currentWave.PossiblePresets;
                wave.MinActiveSpawners = currentWave.MinActiveSpawners;
                wave.SpawnerDelayTime = currentWave.SpawnerDelayTime;
                wave.MaxSpawnerDelayTime = currentWave.MaxSpawnerDelayTime;
            }
            return wave;
        }
        set{
            wave = value;
        }
    }

    EnemyWave wave;

    int activeSpawners;
    [SerializeField, FoldoutGroup("Debug")]
    bool waveRunning = false;
    [SerializeField, FoldoutGroup("Debug")]
    bool waveCompleted = true;
    [SerializeField, FoldoutGroup("Debug")]
    float waveCompleteDelay = 0;

    [FoldoutGroup("Debug")]
    public static List<GameObject> enemies = new List<GameObject>();

    public static WaveHandler Instance { get; private set; }

    public PostDialogueActions postDialogueAction;

    public enum PostDialogueActions {
        WaveStart,
        ShopOpen,
        SecretMoreSinisterThirdThing
    }

    private void Awake() {
        //Load all possible waves for each era
        Waves = Resources.LoadAll<EraWaves>("Era Waves");
        WaveEvents.OnWaveStart += OnWaveStart;
        WaveEvents.OnWaveEnd += OnWaveEnd;
        WaveEvents.OnWaveComplete += OnWaveComplete;
        WaveEvents.OnShopClose += OnShopClose;

        if (Instance != null) {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

       // Wave = 1; // DEBUG WAVE SETTER

        SceneManager.activeSceneChanged += SceneChange;

    }

    private void Update() {
        if (!waveRunning) {
            if (!waveCompleted) {

                for (int i = 0; i < enemies.Count; i++) {
                    if (!enemies[i]){
                        enemies.RemoveAt(i);
                    }
                }

                if (enemies.Count == 0) {
                    if (waveCompleteDelay >= 2) {
                        WaveEvents.WaveComplete();
                    }
                    else
                        waveCompleteDelay += Time.deltaTime;
                } else {
                    waveCompleteDelay = 0;
                }
            }
            return;
        }
        
        if (CurrentWave.Runout){
            WaveEvents.WaveEnd();
        }

        timeSinceLastSpawner += Time.deltaTime;

        if(activeSpawners >= CurrentWave.MinActiveSpawners && timeSinceLastSpawner < CurrentWave.MaxSpawnerDelayTime) {
            return;
        }

        SpawnerSpawn[] spawns = ConstructSpawn();
        List<Vector2> spawnpoints = new List<Vector2>();

        spawnerPoint:
        Vector2 spawnPoint = new Vector2 (
            Random.Range(Playfield.Instance.Settings.TopLeftCorner.x, Playfield.Instance.Settings.BottomRightCorner.x),
            Random.Range(Playfield.Instance.Settings.BottomRightCorner.y, Playfield.Instance.Settings.TopLeftCorner.y)
            );

        for(int x = 0; x < PlanetSpawner.Instance.Planets.Count; x++) {
            if (PlanetSpawner.Instance.Planets[x] != null) {
                float dist = Vector2.Distance(PlanetSpawner.Instance.Planets[x].position, spawnPoint);

                if (dist <= 5) {
                    goto spawnerPoint;
                }
            }
        }

        spawnpoints.Add(spawnPoint);
        Spawner spawner = Instantiate(SpawnerPrefab, spawnPoint, Quaternion.identity);
        foreach(SpawnerSpawn spawn in spawns) {
            for(int i = 0; i < spawn.Count; i++){
                spawner.toSpawn.Add(spawn.Spawn);
            }
        }
        activeSpawners++;

        timeSinceLastSpawner = 0f;
    }

    //This gets called when the dialogue closes
    public static void DialogueFinished() {
        Debug.Log("Dialogue is finished boys");
        switch(Instance.postDialogueAction) {
            case (PostDialogueActions.WaveStart):
                WaveEvents.WaveStart();
                break;
            case (PostDialogueActions.ShopOpen):
                Debug.Log("We gonna open da shop now");
                GameEvents.SetShopState(true);
                break;
        }
    }

    void OnWaveStart() {
        waveRunning = true;
        waveCompleted = false;
        activeSpawners = 0;
        PlayerController.Instance.canControl = true;
    }

    void OnShopClose() {
        postDialogueAction = PostDialogueActions.WaveStart;
        if (!DialogueSpawner.Instance.SpawnPreWave()) {
            WaveEvents.WaveStart();
        }
    }

    void OnWaveEnd() {
        CurrentWave = null;
        waveRunning = false;
    }

    void OnWaveComplete() {
        waveCompleted = true;
        Wave++;
        PlayerController.Instance.canControl = false;
        postDialogueAction = PostDialogueActions.ShopOpen;
        if(!DialogueSpawner.Instance.SpawnPostWave()) {
            GameEvents.SetShopState(true);
        }
        FileSystem.SetFileStory(Loop, Wave);
        FileSystem.SaveFile();
    }

    void SceneChange(Scene old, Scene newScene) {
        if (SceneDescription.GetSceneDescrpiptor.Type == SceneType.Game) {
            OnWaveEnd();
            waveCompleted = true;
            WaveEvents.GameStart();
        }
    }

    public SpawnerSpawn[] ConstructSpawn() {
        SpawnerSpawn[] spawn = new SpawnerSpawn[0];
        SpawnerPreset[] presets = CurrentWave.PossiblePresets;
        List<SpawnerPreset> chosenPresets = new List<SpawnerPreset>();

        int randomCount = 0;

        randomize:
        randomCount++;
        SpawnerPreset preset = presets[Random.Range(0, presets.Length)];
 
        //Running through all the enemies in the preset
        //The conditions for using a preset are as follows:
        /*
        *   1. We have all the enemies in the preset available
        *   2. We have enough of each enemy in the preset
        */
        //As long as those 2 checks pass, then we just use the 1st preset that we choose that matches
        for(int p = 0; p < preset.Spawns.Length; p++) {
            bool matching = true;
            for(int s = 0; s < CurrentWave.EnemiesInWave.Length; s++) {
                if(preset.Spawns[p] == CurrentWave.EnemiesInWave[s] && CurrentWave.EnemiesInWave[s].Count < preset.Spawns[p].Count) {
                    matching = false;
                    break;
                }
            }

            if(!matching && randomCount < presets.Length * 3) {
                goto randomize;
            }
        }

        if(preset != null) {
            spawn = new SpawnerSpawn[preset.Spawns.Length];
            for(int i = 0; i < preset.Spawns.Length; i++) {
                spawn[i].Spawn = preset.Spawns[i].Spawn;
                spawn[i].Count = preset.Spawns[i].Count;
                
                for(int x = 0; x < CurrentWave.EnemiesInWave.Length; x++) {
                    if(CurrentWave.EnemiesInWave[x] == preset.Spawns[i]) {

                        CurrentWave.EnemiesInWave[x].Count -= preset.Spawns[i].Count;
                        break;
                    }
                }
            }

            return spawn;
        }

        //If we get here then none of the presets fit into what we have
        //We assume then that we just have a random assortment of leftover enemies
        //So we'll construct 1 more spawner that contains all of them
        spawn = new SpawnerSpawn[CurrentWave.EnemiesInWave.Length];

        for(int i = 0; i < CurrentWave.EnemiesInWave.Length; i++) {
            spawn[i] = new SpawnerSpawn(){
                Spawn = CurrentWave.EnemiesInWave[i].Spawn,
                Count = CurrentWave.EnemiesInWave[i].Count,
            };
            CurrentWave.EnemiesInWave[i].Count = 0;
        }

        return spawn;
    }
}