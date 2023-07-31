using System.Security.Cryptography;
using System.Text;
using System.Collections.Generic;
using System;
using System.IO;
using UnityEngine;

public class FileSystem
{
    #region File Paths
    //Info
    static readonly string SAVE_FILE_PATH = Application.persistentDataPath + "/Saves";
    static readonly string TEMP_FILE_PATH = Application.temporaryCachePath;
    static readonly string WAVE_FILE_PATH = TEMP_FILE_PATH + "/lockFile.lck";

    //Core File stuff
    static readonly string SAVE_FOLDER_FORMAT = SAVE_FILE_PATH + "/File-{0}";
    static readonly string FILE_INFO_PATH = SAVE_FOLDER_FORMAT + "/FileInfo.podta";

    //Weapons
    static readonly string WEAPONS_PATH = SAVE_FOLDER_FORMAT + "/Weapons.podta";

    //World
    static readonly string WORLD_FOLDER_PATH = SAVE_FOLDER_FORMAT + "/World";
    static readonly string WORLD_PLANET_PATH = WORLD_FOLDER_PATH + "/Planets.podta";
    static readonly string WORLD_STAR_PATH = WORLD_FOLDER_PATH + "/Stars.podta";

    //Story
    static readonly string STORY_DATA_PATH = SAVE_FOLDER_FORMAT + "/Story.podta";
    #endregion

    static bool[] validSaveFiles = new bool[] {
        true,
        true,
        true
    };

    public static bool IsSaveFileValid(int saveFile) => validSaveFiles[saveFile];

    static FileInfoData[] fileDatas = new FileInfoData[3];
    public static FileData CurrentFile;
    public static int saveFileID;

    static string GetSaveFilePath(string filePath, int saveFile) => string.Format(filePath, saveFile);

    static bool SaveFileExists(int saveFile) => Directory.Exists(GetSaveFilePath(SAVE_FOLDER_FORMAT, saveFile));

    static bool EnsureSaveFileIsValid(int saveFile, out string errorReason) {
        bool correctFiles = Directory.Exists(GetSaveFilePath(SAVE_FILE_PATH, saveFile)) &&
                                                       Directory.Exists(GetSaveFilePath(WORLD_FOLDER_PATH, saveFile)) &&
                                                       File.Exists(GetSaveFilePath(FILE_INFO_PATH, saveFile)) &&
                                                       File.Exists(GetSaveFilePath(WEAPONS_PATH, saveFile)) &&
                                                       File.Exists(GetSaveFilePath(WORLD_PLANET_PATH, saveFile)) &&
                                                       File.Exists(GetSaveFilePath(WORLD_STAR_PATH, saveFile)) &&
                                                       File.Exists(GetSaveFilePath(STORY_DATA_PATH, saveFile));

        FileData data = LoadFile(saveFile);

        //Grab the checksum then set the info's checksum to the original value to compare against
        string checksum = data.FileInfo.Checksum;
        string check = "";

        data.FileInfo.Checksum = "1234567890";

        string json = JsonUtility.ToJson(data.FileInfo, true);
        json += JsonUtility.ToJson(data.Weapons, true);
        json += JsonUtility.ToJson(data.Planets, true);
        json += JsonUtility.ToJson(data.Stars, true);
        json += JsonUtility.ToJson(data.Story, true);

        using (var md5 = MD5.Create()) {
            check = BitConverter.ToString(md5.ComputeHash(Encoding.ASCII.GetBytes(json)));
        }

        bool correctChecksum = check == checksum;

        if (!correctChecksum) {
            errorReason = $"Invalid checksum: {checksum}:{check}";
            return false;
        }

        if (!correctFiles) {
            errorReason = "Incorrect file format";
            return false;
        }


        errorReason = "No Error";
        return true;
    }


    static void EnsureCoreFoldersExist() {
        if (!Directory.Exists(SAVE_FILE_PATH)) {
            Directory.CreateDirectory(SAVE_FILE_PATH);
        }
    }

    #region Get Pieces of Save Files

    static byte[] GetSaveFileBytes(string filePath, int saveFile) {
        string path = GetSaveFilePath(filePath, saveFile);
        return File.ReadAllBytes(path);
    }

    public static bool FileExists(int saveFile) => validSaveFiles[saveFile];

    public static FileInfoData GetFileInfo(int saveFile) {
        if (fileDatas[saveFile] != null) {
            return fileDatas[saveFile];
        }

        //Fallback/inital stuff that gets run
        if (!validSaveFiles[saveFile]) {
            Debug.LogError($"Save File {saveFile} is not valid");
            return null;
        }

        string json = Encoding.ASCII.GetString(DecryptBytes(GetSaveFileBytes(FILE_INFO_PATH, saveFile)));
        Debug.Log(json);
        return JsonUtility.FromJson<FileInfoData>(json);
    }

    public static WeaponData[] GetUnlockedWeapons(int saveFile) {
        if (CurrentFile != null) {
            return CurrentFile.Weapons.UnlockedWeapons;
        }

        if (!validSaveFiles[saveFile]) {
            Debug.LogError($"Save File {saveFile} is not valid");
            return null;
        }

        string json = Encoding.ASCII.GetString(DecryptBytes(GetSaveFileBytes(WEAPONS_PATH, saveFile)));
        return JsonUtility.FromJson<WeaponFileData>(json).UnlockedWeapons;
    }

    public static WeaponData[] GetEquippedWeapons(int saveFile) {
        if (CurrentFile != null) {
            return CurrentFile.Weapons.EquippedWeapons;
        }

        if (!validSaveFiles[saveFile]) {
            Debug.LogError($"Save File {saveFile} is not valid");
            return null;
        }

        string json = Encoding.ASCII.GetString(DecryptBytes(GetSaveFileBytes(WEAPONS_PATH, saveFile)));
        return JsonUtility.FromJson<WeaponFileData>(json).EquippedWeapons;
    }

    public static PlanetData GetGalaxyPlanets(int saveFile) {
        if (CurrentFile != null) {
            return CurrentFile.Planets;
        }

        if (!validSaveFiles[saveFile]) {
            Debug.LogError($"Save File {saveFile} is not valid");
            return null;
        }

        string json = Encoding.ASCII.GetString(DecryptBytes(GetSaveFileBytes(WORLD_PLANET_PATH, saveFile)));
        return JsonUtility.FromJson<PlanetData>(json);
    }

    public static StarData GetGalaxyStars(int saveFile) {
        if (CurrentFile != null) {
            return CurrentFile.Stars;
        }

        if (!validSaveFiles[saveFile]) {
            Debug.LogError($"Save File {saveFile} is not valid");
            return null;
        }

        string json = Encoding.ASCII.GetString(DecryptBytes(GetSaveFileBytes(WORLD_STAR_PATH, saveFile)));
        return JsonUtility.FromJson<StarData>(json);
    }

    public static StoryData GetStory(int saveFile) {
        if (CurrentFile != null) {
            return CurrentFile.Story;
        }

        if (!validSaveFiles[saveFile]) {
            Debug.LogError($"Save File {saveFile} is not valid");
            return null;
        }

        string json = Encoding.ASCII.GetString(DecryptBytes(GetSaveFileBytes(STORY_DATA_PATH, saveFile)));
        return JsonUtility.FromJson<StoryData>(json);
    }

    public static void SetWaveFile() => File.WriteAllText(WAVE_FILE_PATH, "idk lol");
    public static void DeleteWaveFile() => File.Delete(WAVE_FILE_PATH);
    #endregion

    public static FileData LoadFile(int saveFile) {
        if (CurrentFile != null) {
            return CurrentFile;
        }

        FileData info = new FileData() {
            FileID = saveFile,
            FileInfo = GetFileInfo(saveFile),
            Weapons = new WeaponFileData() {
                UnlockedWeapons = GetUnlockedWeapons(saveFile),
                EquippedWeapons = GetEquippedWeapons(saveFile),
            },
            Planets = GetGalaxyPlanets(saveFile),
            Stars = GetGalaxyStars(saveFile),
            Story = GetStory(saveFile)
        };

        //Debug.Log(info.FileInfo);

        saveFileID = saveFile;
        CurrentFile = info;
        return info;
    }

    public static void SaveFile() {
        CurrentFile.FileInfo.Checksum = "1234567890";
        
        Dictionary<string, string> jsons;
        string checksum = CreateCheckSum(CurrentFile, saveFileID, out jsons);
        CurrentFile.FileInfo.Checksum = checksum;

        foreach (KeyValuePair<string, string> key in jsons) {
            File.WriteAllBytes(key.Key, EncryptBytes(Encoding.ASCII.GetBytes(key.Value)));
        }
    }

    public static void SetFileStory(int loop, int wave, bool autoSave = true) {
        CurrentFile.Story.Loop = loop;
        CurrentFile.Story.Wave = wave;

        if (autoSave) {
            SaveFile();
        }
    }

    public static void SetFileWeapons(WeaponData[] unlockedWeapons, WeaponData[] equippedWeapons, bool autoSave = true) {
        CurrentFile.Weapons.UnlockedWeapons = unlockedWeapons;
        CurrentFile.Weapons.EquippedWeapons = equippedWeapons;

        if (autoSave) {
            SaveFile();
        }
    }

    public static bool CreateNewSaveFile(string Name, int saveFile) {
        //Create Folders
        Directory.CreateDirectory(GetSaveFilePath(SAVE_FOLDER_FORMAT, saveFile));
        Directory.CreateDirectory(GetSaveFilePath(WORLD_FOLDER_PATH, saveFile));

        //Create new save data objects
        FileInfoData infoData = new FileInfoData() {
            Name = Name,
            StoryProgress = 0f,
            WeaponProgress = 0f,
            Points = 0u,
            Checksum = "1234567890",
        };

        WeaponFileData weapons = new WeaponFileData() {
            UnlockedWeapons = new WeaponData[] {
                new WeaponData() {
                    ID = 0,
                    Level = 1,
                }
            },
            EquippedWeapons = new WeaponData[] {
                new WeaponData() {
                    ID = 0,
                    Level = 1,
                }
            },
        };


        PlanetData planets = new PlanetData();
        StarData stars = new StarData();
        StoryData story = new StoryData() {
            Wave = 1,
            Loop = 1
        };

        Dictionary<string, string> dataJson;

        infoData.Checksum = CreateCheckSum(new FileData() {
            FileInfo = infoData,
            Weapons = weapons,
            Planets = planets,
            Stars = stars,
            Story = story
        }, saveFile, out dataJson);

        foreach (KeyValuePair<string, string> key in dataJson) {
            File.WriteAllBytes(key.Key, EncryptBytes(Encoding.ASCII.GetBytes(key.Value)));
        }

        validSaveFiles[saveFile] = true;

        return true;
    }

    public static void DeleteSaveFile(int saveFile) => Directory.Delete(GetSaveFilePath(SAVE_FOLDER_FORMAT, saveFile), true);

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void OnGameLoad() {
        EnsureCoreFoldersExist();

        for (int i = 0; i < 3; i++) {
            if (!SaveFileExists(i)) {
                Debug.Log($"No save file found for file ID {i}, ignoring...");
                validSaveFiles[i] = false;
                continue;
            }

            string error;

            if (!EnsureSaveFileIsValid(i, out error)) {
                Debug.LogError($"File {i} is invalid: {error}");
                validSaveFiles[i] = false;
                continue;
            }
        }

        fileDatas = new FileInfoData[3];

        for (int i = 0; i < fileDatas.Length; i++) {
            if (!validSaveFiles[i]) {
                continue;
            }

            fileDatas[i] = GetFileInfo(i);
        }
    }

    //DEAR GOD REMOVE THIS BEFORE WE SHIP THE GAME
    static byte[] EncryptBytes(byte[] input) {
        //Debug.LogError("YOU FUCKER YOU STILL HAVENT REMOVED THIS FROM THE PROJECT YET? GET FUCKED FUCK YOU YOURE STUPID REMOVE THIS DUMBASS");
        byte[] fixedInputBytes = new byte[input.Length + (input.Length % 2 == 0 ? 0 : 1)];

        for (int i = 0; i < input.Length; i++) {
            fixedInputBytes[i] = input[i];
        }

        if (fixedInputBytes[^1] == 0) {
            fixedInputBytes[^1] = (byte)'\n';
        }

        byte[] output = new byte[fixedInputBytes.Length];
        for (int i = 0; i < fixedInputBytes.Length; i += 2) {
            output[i / 2] = fixedInputBytes[i];
        }

        for (int i = 1; i < fixedInputBytes.Length; i += 2) {
            output[(i / 2) + (fixedInputBytes.Length / 2)] = fixedInputBytes[i];
        }

        return output;
    }

    static byte[] DecryptBytes(byte[] input) {
        //Debug.LogError("YOU FUCKER YOU STILL HAVENT REMOVED THIS FROM THE PROJECT YET? GET FUCKED FUCK YOU YOURE STUPID REMOVE THIS DUMBASS");
        byte[] output = new byte[input.Length];

        byte[] evenBytes = new byte[input.Length / 2];
        byte[] oddBytes = new byte[input.Length / 2];

        for (int i = 0; i < input.Length / 2; i++) {
            evenBytes[i] = input[i];
            oddBytes[i] = input[i + (input.Length / 2)];
        }

        for (int i = 0; i < input.Length; i++) {
            output[i] = (i % 2 == 0 ? evenBytes : oddBytes)[i / 2];
        }

        return output;
    }

    static string CreateCheckSum(FileData data, int saveFile, out Dictionary<string, string> saveData) {

        //Convert to JSON and encrypt before saving
        string totaljson = "";
        Dictionary<string, string> dataJson = new Dictionary<string, string>();

        string json = JsonUtility.ToJson(data.FileInfo, true);
        dataJson.Add(GetSaveFilePath(FILE_INFO_PATH, saveFile), json);
        totaljson += json;

        json = JsonUtility.ToJson(data.Weapons, true);
        dataJson.Add(GetSaveFilePath(WEAPONS_PATH, saveFile), json);
        totaljson += json;

        json = JsonUtility.ToJson(data.Planets, true);
        dataJson.Add(GetSaveFilePath(WORLD_PLANET_PATH, saveFile), json);
        totaljson += json;

        json = JsonUtility.ToJson(data.Stars, true);
        dataJson.Add(GetSaveFilePath(WORLD_STAR_PATH, saveFile), json);
        totaljson += json;

        json = JsonUtility.ToJson(data.Story, true);
        dataJson.Add(GetSaveFilePath(STORY_DATA_PATH, saveFile), json);
        totaljson += json;

        using (var md5 = MD5.Create()) {
            //Calculate checksum and set that to the new info json
            data.FileInfo.Checksum = BitConverter.ToString(md5.ComputeHash(Encoding.ASCII.GetBytes(totaljson)));
            dataJson[GetSaveFilePath(FILE_INFO_PATH, saveFile)] = JsonUtility.ToJson(data.FileInfo, true);
        }

        saveData = dataJson;
        return data.FileInfo.Checksum;
    }
}


public class SaveFileData {
    public string FilePath;
    public object Data;
}