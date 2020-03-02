﻿using System.Collections.Generic;
using UnityEngine;

public class BuildingDataBase : MonoBehaviour
{
    public enum BUILDINGS        // To reference all possible Buildings
    {
        B_POND,
        B_DIRT,
        B_GRASSHOLE,
        B_DECO,
        B_LAVA,
        B_HOUSE,
        B_SHOP,

        B_TOTAL
    }

    public enum RESOURCES        //possible resources produced
    {
        R_NONE,
        R_MONEY
    }

    [Header("Placement of Buildings")]
    [SerializeField]
    GameObject m_BaseBuildingGO = null;
    [Header("All Avaliable Buildings")]
    [SerializeField]        // Temporary storage of Buildings (ONLY In the Editor)
    List<BuildingData> m_ListOfBuildingSO = new List<BuildingData>();
    // To Store all possible Buildings when running
    Dictionary<BUILDINGS, BuildingData> m_DictOfBuildingSO = new Dictionary<BUILDINGS, BuildingData>();

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
            foreach (BuildingData baseBuilding in m_ListOfBuildingSO)
            {
                if (baseBuilding.GetBuildingType() == (BUILDINGS)i)
                    counter++;
            }
            if (counter > 1)
            {
                Debug.LogError("Duplicate Enum ID in BUILDING Database!!");
                return;
            }
        }
        // If no duplicates, convert to Dictionary
        foreach (BuildingData baseBuilding in m_ListOfBuildingSO)
            m_DictOfBuildingSO[baseBuilding.GetBuildingType()] = baseBuilding;
        m_ListOfBuildingSO.Clear();
    }

    //public GameObject GetBuildingGO(BUILDINGS buildingType) { return m_DictOfBuildingSO[buildingType].gameObject; }
    //public BaseBuildingsClass GetBuildingCom(BUILDINGS buildingType) { return m_DictOfBuildingSO[buildingType]; }

    public BuildingData GetBuildingData(BUILDINGS buildingType) { return m_DictOfBuildingSO[buildingType]; }
    public GameObject GetBaseBuildingGO() { return m_BaseBuildingGO; }
}


