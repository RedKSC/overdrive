using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Doozy.Runtime.Signals;

public class NewFileDialogue : MonoBehaviour {
    public bool Active;

    public TMP_Text NameText;
    public SignalSender Sender;

    string currentName;
    int activeFile;

    void Update() {
        if (!Active) {
            return;
        }

        string inputStream = Input.inputString.ToUpper();

        if (Input.anyKeyDown && inputStream.Length == 1 && ((inputStream[0] >= 'a' && inputStream[0] <= 'z') || (inputStream[0] >= '0' && inputStream[0] <= '9') || (inputStream[0] >= 'A' && inputStream[0] <= 'Z'))) {
            currentName += inputStream[0];
            ConstructName();
        }
    }

    public void ConstructName() {
        string name = "";
        for (int i = 0; i < 3; i++) {
            if (i < currentName.Length) {
                name += currentName[i];
                continue;
            }

            if (i == currentName.Length) {
                name += "<u>X</u>";
                continue;
            }

            name += 'X';
        }

        NameText.text = name;

        if (currentName.Length >= 3) {
            FileSystem.CreateNewSaveFile(currentName, activeFile);
            Sender.SendSignal();
            Active = false;
        }
    }

    public void SetActiveState(bool active) => Active = active;
    public void SetActiveFile(int file) => activeFile = file;
}
