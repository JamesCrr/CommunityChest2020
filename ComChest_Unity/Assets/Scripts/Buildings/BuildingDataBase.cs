using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingDataBase : MonoBehaviour
{
    public enum BUILDINGS        // To reference all possible Buildings
    {
        B_POND,
        B_DIRT,
        B_GRASSHOLE,

        B_TOTAL
    }
    [SerializeField]        // Temporary storage of Buildings (ONLY In the Editor)
    List<BaseBuildingsClass> listOfBuildings = new List<BaseBuildingsClass>();
    // To Store all possible Buildings when running
    Dictionary<BUILDINGS, BaseBuildingsClass> dictOfBuildings = new Dictionary<BUILDINGS, BaseBuildingsClass>();

    static BuildingDataBase m_Instance = null;
    public static BuildingDataBase GetInstance() { return m_Instance; }
    private void Awake()
    {
        if (m_Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        m_Instance = this;
        DontDestroyOnLoad(gameObject);

        // Check for duplicates
        int counter;
        for (int i = 0; i < (int)BUILDINGS.B_TOTAL; ++i)
        {
            counter = 0;
            foreach (BaseBuildingsClass baseBuilding in listOfBuildings)
            {
                if (baseBuilding.GetBuildingID() == (BUILDINGS)i)
                    counter++;
            }
            if (counter > 1)
            {
                Debug.LogError("Duplicate Enum ID in BUILDING Database!!");
                return;
            }
        }
        // If no duplicates, convert to Dictionary
        foreach (BaseBuildingsClass baseBuilding in listOfBuildings)
            dictOfBuildings[baseBuilding.GetBuildingID()] = baseBuilding;
        listOfBuildings.Clear();
    }

    public GameObject GetBuildingGO(BUILDINGS buildingType) { return dictOfBuildings[buildingType].gameObject; }
    public BaseBuildingsClass GetBuildingCom(BUILDINGS buildingType) { return dictOfBuildings[buildingType]; }

}

//[System.Serializable]
//public class BuildingDataEntry
//{
//    [SerializeField]
//    BuildingDataBase.BUILDINGS buildingID = BuildingDataBase.BUILDINGS.B_POND;
//    [SerializeField]
//    BaseBuildingsClass buildingObject = null;

//    public BuildingDataBase.BUILDINGS GetBuildingID() { return buildingID; }
//    public BaseBuildingsClass GetBuildingObject() { return buildingObject; }
//}
