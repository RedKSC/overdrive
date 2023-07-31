using System.Runtime.Versioning;
using System.Security.Cryptography;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueSpawner : MonoBehaviour {
    public static DialogueSpawner Instance;

    #region Story
    const string StoryObjectNameFormat = "Dialogue_{0}_{1}_{2}Wave";
    string ConstructStoryObjectName(int Loop, int Wave, bool PostWave) => string.Format(StoryObjectNameFormat, Loop, Wave - (PostWave ? 1 : 0), (PostWave ? "Post" : "Pre"));
    public bool SpawnPostWave() => SpawnStoryDialogue(ConstructStoryObjectName(WaveHandler.Instance.Loop, WaveHandler.Instance.Wave, true));
    public bool SpawnPreWave() {
        bool check = SpawnStoryDialogue(ConstructStoryObjectName(WaveHandler.Instance.Loop, WaveHandler.Instance.Wave, false));
        if (check) {
            WaveHandler.Instance.postDialogueAction = WaveHandler.PostDialogueActions.WaveStart;
        }

        return check;
    }
    bool SpawnStoryDialogue(string name) {
        Debug.Log($"Dialogue/Story/{name}");
        GameObject dialogue = Resources.Load<GameObject>($"Dialogue/Story/{name}");

        if (dialogue) {
            Instantiate(dialogue);
        }

        Debug.Log(dialogue != null);
        return dialogue;
    }
    #endregion

    #region Shop

    #region General
    public bool SpawnDialogue(string dialogueName) {
        GameObject dialogue = Resources.Load<GameObject>("Dialogue/" + dialogueName);

        if (dialogue) {
            Instantiate(dialogue);
            return true;
        }

        return false;
    }
    #endregion
    Dictionary<DialogueCharacter, GameObject[]> characterShopDialogues = new Dictionary<DialogueCharacter, GameObject[]>();

    public void SpawnShopDialogue(DialogueCharacter character) => SpawnShopDialogue(character, Random.Range(0, characterShopDialogues[character].Length));

    public void SpawnShopDialogue(DialogueCharacter character, int convoID) {
        GameObject dialogue = characterShopDialogues[character][convoID.Clamp(0, characterShopDialogues[character].Length)];

        Instantiate(dialogue);
    }
    #endregion

    private void Awake() {
        if (Instance) {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        characterShopDialogues.Add(DialogueCharacter.Stella, Resources.LoadAll<GameObject>("Dialogue/Shop/Stella"));
        characterShopDialogues.Add(DialogueCharacter.Ansel, Resources.LoadAll<GameObject>("Dialogue/Shop/Ansel"));
        characterShopDialogues.Add(DialogueCharacter.Zane, Resources.LoadAll<GameObject>("Dialogue/Shop/Zane"));
    }
}

public enum DialogueCharacter {
    Stella,
    Ansel,
    Zane
}
