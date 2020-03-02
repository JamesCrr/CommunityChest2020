using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public static class SaveSystem
{
    static string m_BuildingsFilePath = Path.Combine(Application.persistentDataPath, "buildings.save");

    public static void SaveBuildingsOnMap(List<BaseBuildingsClass> buildingsOnMap)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(m_BuildingsFilePath, FileMode.Create);
        // Get Data
        Save_BuildingsOnMap data = new Save_BuildingsOnMap();
        data.buildingType = new int[buildingsOnMap.Count];
        data.worldPosX = new float[buildingsOnMap.Count];
        data.worldPosY = new float[buildingsOnMap.Count];
        for (int i =0; i < buildingsOnMap.Count; ++i)
        {
            data.buildingType[i] = (int)(buildingsOnMap[i].GetBuildingType());
            data.worldPosX[i] = buildingsOnMap[i].transform.position.x;
            data.worldPosY[i] = buildingsOnMap[i].transform.position.y;
        }
        // Save
        formatter.Serialize(stream, data);
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
        Save_BuildingsOnMap data = formatter.Deserialize(stream) as Save_BuildingsOnMap;
        stream.Close();
        // Do smth with Data
        MapManager.GetInstance().SaveFileWasLoaded(data);

        Debug.Log("LOADED: Buildings On Map!");
    }

}


[System.Serializable]
public class Save_BuildingsOnMap
{
    public float[] worldPosX;
    public float[] worldPosY;
    public int[] buildingType;
}
