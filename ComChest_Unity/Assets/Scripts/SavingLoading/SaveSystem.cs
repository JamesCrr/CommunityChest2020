using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public static class SaveSystem
{
    static string m_BuildingsFilePath = Path.Combine(Application.persistentDataPath, "buildings.save");

    public static void SaveMap(List<BaseBuildingsClass> buildingsOnMap, Save_RoadsOnMap[] roadsOnMapData)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(m_BuildingsFilePath, FileMode.Create);
        // Get Data
        Save_BuildingsOnMap[] buildingsSavedata = new Save_BuildingsOnMap[buildingsOnMap.Count];
        for (int i =0; i < buildingsOnMap.Count; ++i) //store and init the data
        {
            buildingsSavedata[i] = new Save_BuildingsOnMap(buildingsOnMap[i].transform.position.x,
                buildingsOnMap[i].transform.position.y,
                (int)(buildingsOnMap[i].GetBuildingType()));
        }

        //store all the info here
        Save_Data saveData = new Save_Data();
        saveData.saved_BuildingsOnMap = buildingsSavedata;
        saveData.saved_RoadsOnMap = roadsOnMapData;

        // Save
        formatter.Serialize(stream, saveData);
        stream.Close();
        Debug.Log("SAVED: Buildings On Map!");
    }
    public static void LoadSavedBuildingsToMap()
    {
        if (!File.Exists(m_BuildingsFilePath))
        {
            Debug.LogError("Loading Building Save Data: No such Path!!");
            return;
        }
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(m_BuildingsFilePath, FileMode.Open);
        // Load Data
        Save_Data data = formatter.Deserialize(stream) as Save_Data;
        stream.Close();
        // Do smth with Data
        MapManager.GetInstance().SaveFileWasLoaded(data.saved_BuildingsOnMap, data.saved_RoadsOnMap);

        Debug.Log("LOADED: Buildings On Map!");
    }

}

[System.Serializable]
public class Save_Data
{
    public Save_BuildingsOnMap[] saved_BuildingsOnMap;
    public Save_RoadsOnMap[] saved_RoadsOnMap;
}

[System.Serializable]
public class Save_BuildingsOnMap
{
    public float worldPosX;
    public float worldPosY;
    public int buildingType;

    public Save_BuildingsOnMap()
    {
        worldPosX = 0.0f;
        worldPosY = 0.0f;
        buildingType = 0;
    }

    public Save_BuildingsOnMap(float posX, float posY, int buildingType)
    {
        this.worldPosX = posX;
        this.worldPosY = posY;
        this.buildingType = buildingType;
    }
}

[System.Serializable]
public class Save_RoadsOnMap
{
    public float worldPosX;
    public float worldPosY;
    public int roadType;
}

