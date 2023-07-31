using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueStorySetter : MonoBehaviour {
    public void Start() {
        int id = FileSystem.CurrentFile.FileID;
        StoryData dta = FileSystem.GetStory(id);
        WaveHandler.Instance.Loop = dta.Loop;
        WaveHandler.Instance.Wave = dta.Wave;

        WaveHandler.Instance.postDialogueAction = WaveHandler.PostDialogueActions.ShopOpen;
        if (dta.Wave == 1)
            DialogueSpawner.Instance.SpawnPreWave();
        else {
            if(!DialogueSpawner.Instance.SpawnPostWave()) {
                GameEvents.SetShopState(true);
            }
        }
    }
}
