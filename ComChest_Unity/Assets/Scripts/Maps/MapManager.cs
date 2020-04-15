﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    [Header("Grid and Placements")]
    [SerializeField]        // Where the Tilemaps will be rendered
    Grid m_GridGO = null;
    BaseMapClass m_currentMap;
    // To store which tiles on map are taken by building
    List<bool> m_GridTakenArray;        
    // To store the buildings currently on the map
    Dictionary<Vector2Int, BaseBuildingsClass> m_DictOfBuildingsOnMap = new Dictionary<Vector2Int,BaseBuildingsClass>();
    // Map Generated Action
    public delegate void MapGeneratedAction();      
    public static event MapGeneratedAction OnMapGenerated;

    RoadManager m_RoadManager = new RoadManager();

    static MapManager m_Instance = null;
    public static MapManager GetInstance() { return m_Instance; }
    private void Awake()
    {
        if (m_Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        m_Instance = this;
        DontDestroyOnLoad(gameObject);
        m_GridTakenArray = new List<bool>();

        // Parent the m_GridGO
        m_GridGO.transform.parent = transform;
    }
    private void Start()
    {
        CreateNewMap(MapDataBase.MAPS.M_GRASS);

        m_RoadManager.Init(m_currentMap.GetTileMapSize());
    }

    /// <summary>
    /// Creates a new TileMap 
    /// </summary>
    /// <param name="mapType"></param>
    /// <returns></returns>
    public bool CreateNewMap(MapDataBase.MAPS mapType)
    {
        // Create new Tilemap, parented under and positioned at m_GridGO
        GameObject mapObject = MapDataBase.GetInstance().GetMapGO(mapType);
        mapObject = Instantiate(mapObject, m_GridGO.transform.position, Quaternion.identity, m_GridGO.transform);
        m_currentMap = mapObject.GetComponent<BaseMapClass>();
        // Resize the layout Array
        int totalSize = m_currentMap.GetTileMapSize().x * m_currentMap.GetTileMapSize().y;
        m_GridTakenArray.Clear();
        m_GridTakenArray.Capacity = totalSize;     // Set the capactiy to prevent calling Array.Resize() multiple times
        for (int i = 0; i < totalSize; ++i)
            m_GridTakenArray.Add(false);
        // Center the Camera to the map
        Vector3 centerMapWorldPos = m_GridGO.CellToWorld((Vector3Int)m_currentMap.GetTileMapSize() / 2);
        centerMapWorldPos += m_GridGO.cellSize * 0.5f;
        centerMapWorldPos.z = -10.0f;
        Camera.main.transform.position = centerMapWorldPos;

        // Fire the Map Generated Event
        OnMapGenerated();

        return true;
    }
    /// <summary>
    /// Tries to place a Building into the Grid from it's world Position.
    /// Returns null if Unable to, returns the newly created GO otherwise
    /// </summary>
    /// <param name="activeBuildingCom">The Building GO to place</param>
    /// <param name="doChecking">Are any of the spots taken by another building already?</param>
    /// <returns></returns>
    public GameObject PlaceBuildingToGrid(BaseBuildingsClass activeBuildingCom, bool doChecking = false)
    {
        Vector3 buildingBottomLeftWorldPos = activeBuildingCom.GetBottomLeftGridPosition();
        Vector2Int buildingSize = activeBuildingCom.GetBuildingSizeOnMap();
        //Debug.Log("GRID POS: " + m_GridGO.WorldToCell(buildingWorldPos));
        // Can we place it there?
        if(doChecking)
            if (!CanPlaceBuilding(buildingBottomLeftWorldPos, buildingSize))
                return null;
        // Set all the grids taken by new building to true
        SetGridTakenArray(buildingBottomLeftWorldPos, buildingSize, true);

        AddBuildingIntoTrackingDictionary(activeBuildingCom); //roads and buildings store accordingly
        activeBuildingCom.BuildingPlaced();

        return activeBuildingCom.gameObject;
    }
    /// <summary>
    /// Removes the Building from Grid by setting it's array spots back to untaken, then returns it
    /// </summary>
    /// <param name="activeBuildingCom"></param>
    /// <returns></returns>
    public GameObject RemoveBuildingFromGrid(BaseBuildingsClass activeBuildingCom)
    {
        Vector3 buildingBottomLeftWorldPos = activeBuildingCom.GetBottomLeftGridPosition();
        Vector2Int buildingSize = activeBuildingCom.GetBuildingSizeOnMap();
        // Set all the grids taken by building to false, since no longer there
        SetGridTakenArray(buildingBottomLeftWorldPos, buildingSize, false);
        RemoveBuildingFromTrackingDictionary(activeBuildingCom);
        activeBuildingCom.BuildingRemoved();

        return activeBuildingCom.gameObject;
    }

    private void Update()
    {
        Vector3Int gridPos = m_currentMap.GetTileMapCom().WorldToCell(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        Debug.Log("GridPOs: " + gridPos + "\n" + m_currentMap.GetTileMapCom().HasTile(gridPos));
        //Debug.Log("Tile at : " + gridPos + "\n" + m_currentMap.GetTileMapCom().GetTile(gridPos));
    }

    #region Misc
    /// <summary>
    /// Checks if any of the spots the building will occupy, is already taken
    /// </summary>
    /// <param name="buildingBottomLeftWorldPos">Bottom Left Corner of the Building</param>
    /// <param name="buildingSize">How many grids does this building take</param>
    /// <returns>True if can place Building</returns>
    public bool CanPlaceBuilding(BaseBuildingsClass activeBuildingCom)
    {
        return CanPlaceBuilding(activeBuildingCom.GetBottomLeftGridPosition(), activeBuildingCom.GetBuildingSizeOnMap());
    }
    public bool CanPlaceBuilding(Vector3 buildingBottomLeftWorldPos, Vector2Int buildingSize)
    {
        Vector2Int buildingGridPos = (Vector2Int)m_GridGO.WorldToCell(buildingBottomLeftWorldPos);
        Vector2Int testGridPos = buildingGridPos;
        int arrayIndex;
      
        for (int yAxis = 0; yAxis < buildingSize.y; ++yAxis)
        {
            testGridPos.y += yAxis;
            for (int xAxis = 0; xAxis < buildingSize.x; ++xAxis)
            {
                testGridPos.x = buildingGridPos.x + xAxis;
                // Out of Scope?
                if (testGridPos.x < 0 || testGridPos.x >= m_currentMap.GetTileMapSize().x ||
                    testGridPos.y < 0 || testGridPos.y >= m_currentMap.GetTileMapSize().y)
                    return false;
                // Out of Map?
                if (!m_currentMap.GetTileMapCom().HasTile((Vector3Int)testGridPos))
                    return false;
                //    return false;
                // Check if spot is already taken
                arrayIndex = Convert2DToIntIndex(testGridPos);
                if (m_GridTakenArray[arrayIndex])
                {
                    //Debug.Log("SPOT TAKEN");
                    return false;
                }
            }
            // Reset checking Position
            testGridPos = buildingGridPos;
        }
        return true;
    }
    /// <summary>
    /// Sets the GridTakenArray's values. Starts from GridPos of buildingBottomLeftWorldPos
    /// </summary>
    /// <param name="buildingBottomLeftWorldPos">Bottom Left Corner of the Building</param>
    /// <param name="buildingSize">How many grids does this building take</param>
    /// <param name="valueToSet">What value to Set for all those grids</param>
    void SetGridTakenArray(Vector3 buildingBottomLeftWorldPos, Vector2Int buildingSize, bool valueToSet)
    {
        // Loop through from buildingBottomLeftWorldPos
        Vector2Int buildingGridPos = (Vector2Int)m_GridGO.WorldToCell(buildingBottomLeftWorldPos);
        Vector2Int testGridPos = buildingGridPos;
        int arrayIndex = Convert2DToIntIndex(buildingGridPos);
        // Go through the entire area the building occupies
        for (int yAxis = 0; yAxis < buildingSize.y; ++yAxis)
        {
            testGridPos.y += yAxis;
            for (int xAxis = 0; xAxis < buildingSize.x; ++xAxis)
            {
                testGridPos.x = buildingGridPos.x + xAxis;
                arrayIndex = Convert2DToIntIndex(testGridPos);
                m_GridTakenArray[arrayIndex] = valueToSet;
            }
            // Reset checking Position
            testGridPos = buildingGridPos;
        }
    }
    /// <summary>
    /// Adds a building into the tracking Dictionary
    /// </summary>
    /// <param name="activeBuildingCom"></param>
    void AddBuildingIntoTrackingDictionary(BaseBuildingsClass activeBuildingCom)
    {
        Vector2Int key = (Vector2Int)m_GridGO.WorldToCell(activeBuildingCom.GetBottomLeftGridPosition());
        if(m_DictOfBuildingsOnMap.ContainsKey(key))
        {
            Debug.LogError("Duplicate Key in MapManager Building Storage!!");
            return;
        }

        //check if its roads or not, store accordingly
        if (activeBuildingCom.GetBuildingType() == BuildingDataBase.BUILDINGS.B_ROAD)
        {
            m_RoadManager.PlaceRoads(key, ref activeBuildingCom);
        }
        else
        {
            m_DictOfBuildingsOnMap[key] = activeBuildingCom;
        }
    }
    /// <summary>
    /// Removes a building from the tracking Dictionary
    /// </summary>
    /// <param name="activeBuildingCom"></param>
    void RemoveBuildingFromTrackingDictionary(BaseBuildingsClass activeBuildingCom)
    {
        Vector2Int key = (Vector2Int)m_GridGO.WorldToCell(activeBuildingCom.GetBottomLeftGridPosition());
        if (m_DictOfBuildingsOnMap.ContainsKey(key)) //if have remove
        {
            m_DictOfBuildingsOnMap.Remove(key);
            return;
        }

        //check roads instead if its not in building map
        m_RoadManager.RemoveRoads(key);
    }


    int Convert2DToIntIndex(Vector2Int v2Index) 
    {
        if (v2Index.x < 0 || v2Index.y < 0)
            return -1;
        return (v2Index.y * m_currentMap.GetTileMapSize().x) + v2Index.x;
    }
    Vector2Int ConvertIntIndexTo2D(int arrayIndex)
    {
        Vector2Int result = new Vector2Int();
        result.y = arrayIndex / m_currentMap.GetTileMapSize().x;
        result.x = arrayIndex - (result.y * m_currentMap.GetTileMapSize().x);
        return result;
    }
    #endregion

    #region Load From File
    public void SaveFileWasLoaded(Save_BuildingsOnMap[] saveFileBuildingsData, Save_RoadsOnMap[] saveFileRoadsData)
    {
        BaseBuildingsClass savedBuilding = null;
        Vector3 savedPosition = Vector3.zero;
        for(int i = 0; i < saveFileBuildingsData.Length; ++i)
        {
            savedPosition.x = saveFileBuildingsData[i].worldPosX;
            savedPosition.y = saveFileBuildingsData[i].worldPosY;
            savedBuilding = Instantiate(BuildingDataBase.GetInstance().GetBaseBuildingGO(), savedPosition, Quaternion.identity).GetComponent<BaseBuildingsClass>();
            savedBuilding.SetNewBuildingType(BuildingDataBase.GetInstance().GetBuildingData((BuildingDataBase.BUILDINGS)saveFileBuildingsData[i].buildingType));
            PlaceBuildingToGrid(savedBuilding);
        }

        //do the same for the roads
        for (int i = 0; i < saveFileRoadsData.Length; ++i)
        {
            savedPosition.x = saveFileRoadsData[i].worldPosX;
            savedPosition.y = saveFileRoadsData[i].worldPosY;
            savedBuilding = Instantiate(BuildingDataBase.GetInstance().GetBaseBuildingGO(), savedPosition, Quaternion.identity).GetComponent<BaseBuildingsClass>();
            savedBuilding.SetNewBuildingType(BuildingDataBase.GetInstance().GetBuildingData(BuildingDataBase.BUILDINGS.B_ROAD));

            //set to the correct road direction sprite
            RoadTypeList roadType = (RoadTypeList)saveFileRoadsData[i].roadType;
            savedBuilding.SetSprite(BuildingDataBase.GetInstance().GetRoadSprite(roadType));
            StoreLoadedRoads(savedBuilding, roadType); 
        }
    }

    //to store the loaded roads from the save file into the roadManager
    public void StoreLoadedRoads(BaseBuildingsClass roadBuilding, RoadTypeList roadType) 
    {
        if (m_RoadManager == null)
            return;

        Vector3 buildingBottomLeftWorldPos = roadBuilding.GetBottomLeftGridPosition();
        Vector2Int buildingSize = roadBuilding.GetBuildingSizeOnMap();

        // Set all the grids taken by new building to true
        SetGridTakenArray(buildingBottomLeftWorldPos, buildingSize, true);

        Vector2Int key = (Vector2Int)m_GridGO.WorldToCell(roadBuilding.GetBottomLeftGridPosition());
        roadBuilding.BuildingPlaced();

        m_RoadManager.StoreLoadedInRoads(key, roadBuilding, roadType);
    }
    #endregion

    #region Getters
    //public Grid GetGrid() { return m_GridGO; }
    public BaseMapClass GetCurrentMap() { return m_currentMap;  }
    public List<BaseBuildingsClass> GetBuildingsOnMap()
    {
        List<BaseBuildingsClass> listOfBuildings = new List<BaseBuildingsClass>();
        foreach (KeyValuePair<Vector2Int, BaseBuildingsClass> entry in m_DictOfBuildingsOnMap)
        {
            listOfBuildings.Add(entry.Value);
        }
        return listOfBuildings;
    }
    public Save_RoadsOnMap[] GetSavedRoads()
    { 
        if (m_RoadManager != null)
            return m_RoadManager.GetSavedRoads();

        return null;
    }
    #endregion
}
