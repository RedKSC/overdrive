using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Doozy.Runtime.Signals;

public class FileSelect : MonoBehaviour {
    public TMP_Text[] Names;
    public SignalSender FileLoadedSender;
    public SignalSender[] NewFileSenders;

    private void Awake() => LoadFileDatas();

    public void NewFileReady() {
        LoadFileDatas();
    }

    public void LoadGame(int saveFile)
    {
        if (!FileSystem.IsSaveFileValid(saveFile)) {
            NewFileSenders[saveFile].SendSignal();
            return;
        }

        FileSystem.LoadFile(saveFile);
        FileLoadedSender.SendSignal();
    }

    void LoadFileDatas() {
        for (int i = 0; i < Names.Length; i++) {
            if (!FileSystem.FileExists(i))
            {
                Debug.Log($"No valid file for save file {i}");
                continue;
            }

            Debug.Log($"Loaded Save File {i}:{FileSystem.GetFileInfo(i).Name}");
            Names[i].text = FileSystem.GetFileInfo(i).Name;
        }
    }
}
