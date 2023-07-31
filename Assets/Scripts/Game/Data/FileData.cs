using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#region Save File Structure
/*
    MAIN FOLDER (FILE-ID ex. "File-0")
    |
    |____FileInfo.json
    |   |
    |   |____Name:String (3 chars)
    |   |____StoryProgress:Float (0-1)
    |   |____WeaponProgress:Float (0-1)
    |   |____Checksum:???
    |
    |____Weapons
    |   |
    |   |____UnlockedWeapons.json
    |   |   |
    |   |   |____Weapon[]:Object
    |   |       |
    |   |       |____ID:Int
    |   |       |____Level:Int
    |   |
    |   |____EquippedWeapons.json
    |       |
    |       |____Weapon[]
    |
    |____World
    |   |
    |   |____Planets.json
    |   |   |
    |   |   |____PlanetData[]
    |   |   |   |
    |   |   |   |____Position:Vector2
    |   |   |   |____Citizens:Int
    |   |
    |   |____Stars.json
    |       |
    |       |____Position[]:Vector2
    |
    |____Story.json (might make this a binary file so people can't read)
    |   |
    |   |____LoopNumber:Int
    |   |____WaveNumber:Int
*/
#endregion

/// <summary>
/// Class for handling all data of a save file
/// </summary>
public class FileData
{
    //Info
    public int FileID;
    public FileInfoData FileInfo;

    //Weapons
    public WeaponFileData Weapons;

    //World
    public PlanetData Planets;
    public StarData Stars;

    //Story
    public StoryData Story;
}

[System.Serializable]
public class FileInfoData
{
    public string Name;
    public float StoryProgress;
    public float WeaponProgress;
    public long Points;
    public string Checksum = "1234567890";

    public override string ToString()
    {
        return $"File Info: Name:{Name}, Story:{StoryProgress}, Weapons:{WeaponProgress}, CheckSum:{Checksum}";
    }
}

[System.Serializable]
public class StoryData
{
    public int Loop;
    public int Wave;
}

[System.Serializable]
public class PlanetData
{
    PlanetInfoData[] Planets;
}

[System.Serializable]
public class PlanetInfoData
{
    public Vector2 Position;
    public int CitizenCount;
}

[System.Serializable]
public class StarData
{
    public Vector2[] StarPositions;
}

[System.Serializable]
public class WeaponFileData {
    public WeaponData[] UnlockedWeapons;
    public WeaponData[] EquippedWeapons;
}

[System.Serializable]
public class WeaponData
{
    public int ID;
    public int Level;
}
