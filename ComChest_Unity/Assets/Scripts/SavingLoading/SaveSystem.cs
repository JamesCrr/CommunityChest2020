using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public static class SaveSystem
{
    static string buildingFileHeader = "buildings";
    static string resourceFileHeader = "resources";
    static string fileType = ".save";

    public static void SaveToFile(string mapName = "")
    {
        // Buildings on Map
        SaveBuildings(mapName, MapManager.GetInstance().GetBuildingsOnMap(), MapManager.GetInstance().GetRoadManager().GetSavedRoads());
        // Resources 
        SaveResources(mapName);

    }
    public static void LoadFromFile(string mapName = "")
    {
        // Buildings on Map
        LoadSavedBuildingsToMap(mapName);
        // Resources
        LoadResources(mapName);

    }
    public static void DeleteSaveFile(string mapName = "")
    {
        string newFilePath;
        // Buildings
        newFilePath = Path.Combine(Application.persistentDataPath, buildingFileHeader + mapName + fileType);
        DeleteAFile(newFilePath);
        // Resources
        newFilePath = Path.Combine(Application.persistentDataPath, resourceFileHeader + mapName + fileType);
        DeleteAFile(newFilePath);
    }
    public static bool DoesSaveFileExist(string mapName = "")
    {
        string newFilePath;
        newFilePath = Path.Combine(Application.persistentDataPath, buildingFileHeader + mapName + fileType);
        if (!File.Exists(newFilePath))
        {
            Debug.LogError("Buildings Data NOT Found: No such Path!!\n" + newFilePath);
            return false;
        }
        newFilePath = Path.Combine(Application.persistentDataPath, resourceFileHeader + mapName + fileType);
        if (!File.Exists(newFilePath))
        {
            Debug.LogError("Resources Data NOT Found: No such Path!!\n" + newFilePath);
            return false;
        }

        return true;
    }

    #region Map Data
    static void SaveBuildings(string mapName, List<BaseBuildingsClass> buildingsOnMap, RoadsOnMap[] roadsOnMapData)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string newFilePath = Path.Combine(Application.persistentDataPath, buildingFileHeader + mapName + fileType);
        FileStream stream = new FileStream(newFilePath, FileMode.Create);
        // Get Data
        BuildingsOnMap[] buildingsSavedata = new BuildingsOnMap[buildingsOnMap.Count];
        for (int i =0; i < buildingsOnMap.Count; ++i) //store and init the data
        {
            buildingsSavedata[i] = new BuildingsOnMap(buildingsOnMap[i].transform.position.x,
                buildingsOnMap[i].transform.position.y,
                (int)(buildingsOnMap[i].GetBuildingType()));
        }

        //store all the info here
        Save_AllMapData saveMapData = new Save_AllMapData();
        saveMapData.saved_BuildingsOnMap = buildingsSavedata;
        saveMapData.saved_RoadsOnMap = roadsOnMapData;

        // Save
        formatter.Serialize(stream, saveMapData);
        stream.Close();
        Debug.Log("SAVED: Buildings Data!\nPath: " + newFilePath);
    }
    static void LoadSavedBuildingsToMap(string mapName)
    {
        string newFilePath = Path.Combine(Application.persistentDataPath, buildingFileHeader + mapName + fileType);
        if (!File.Exists(newFilePath))
        {
            Debug.LogError("Loading Building Saved Data: No such Path!!\n" + newFilePath);
            return;
        }
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(newFilePath, FileMode.Open);
        // Load Data
        Save_AllMapData mapData = formatter.Deserialize(stream) as Save_AllMapData;
        stream.Close();
        // Do smth with Data
        MapManager.GetInstance().SaveFileWasLoaded(mapData.saved_BuildingsOnMap, mapData.saved_RoadsOnMap);

        Debug.Log("LOADED: Map Data!");
    }
    #endregion

    #region Resource Data
    static void SaveResources(string mapName)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string newFilePath = Path.Combine(Application.persistentDataPath, resourceFileHeader + mapName + fileType);
        FileStream stream = new FileStream(newFilePath, FileMode.Create);
        // Create the Save Structure
        Save_AllResourcesData saveResourceData = new Save_AllResourcesData();
        saveResourceData.saved_Resources = new Resources[(int)ResourceManager.RESOURCES.R_NONE];
        // Store and Init the data
        for (int i = 0; i < saveResourceData.saved_Resources.Length; ++i)
        {
            saveResourceData.saved_Resources[i] = new Resources(i, ResourceManager.GetInstance().GetResource((ResourceManager.RESOURCES)i));
        }
        // Save
        formatter.Serialize(stream, saveResourceData);
        stream.Close();
        Debug.Log("SAVED: Resources Data!\nPath: " + newFilePath);
    }
    static void LoadResources(string mapName)
    {
        string newFilePath = Path.Combine(Application.persistentDataPath, resourceFileHeader + mapName + fileType);
        if (!File.Exists(newFilePath))
        {
            Debug.LogError("Loading Resources Saved Data: No such Path!!\n" + newFilePath);
            return;
        }
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(newFilePath, FileMode.Open);
        // Load Data
        Save_AllResourcesData resourceData = formatter.Deserialize(stream) as Save_AllResourcesData;
        stream.Close();
        // Set the Resources
        for (int i = 0; i < resourceData.saved_Resources.Length; ++i)
        {
            ResourceManager.GetInstance().SetResource((ResourceManager.RESOURCES)resourceData.saved_Resources[i].resourceID, resourceData.saved_Resources[i].amount);
        }

        Debug.Log("LOADED: Resource Data!");
    }
    #endregion

    #region Misc
    static void DeleteAFile(string filePath)
    {
        try
        {
            File.Delete(filePath);
        }
        catch (IOException ex)
        {
            Debug.LogException(ex);
        }
    }
    #endregion
}

[System.Serializable]
public class Save_AllMapData
{
    public BuildingsOnMap[] saved_BuildingsOnMap;
    public RoadsOnMap[] saved_RoadsOnMap;
}
[System.Serializable]
public class BuildingsOnMap
{
    public float worldPosX;
    public float worldPosY;
    public int buildingType;

    public BuildingsOnMap()
    {
        worldPosX = 0.0f;
        worldPosY = 0.0f;
        buildingType = 0;
    }

    public BuildingsOnMap(float posX, float posY, int buildingType)
    {
        this.worldPosX = posX;
        this.worldPosY = posY;
        this.buildingType = buildingType;
    }
}
[System.Serializable]
public class RoadsOnMap
{
    public float worldPosX;
    public float worldPosY;
    public int roadType;
}


[System.Serializable]
public class Save_AllResourcesData
{
    public Resources[] saved_Resources;
}
[System.Serializable]
public class Resources
{
    public int resourceID;
    public int amount;

    public Resources(int newResourceID, int newAmount)
    {
        resourceID = newResourceID;
        amount = newAmount;
    }
}
