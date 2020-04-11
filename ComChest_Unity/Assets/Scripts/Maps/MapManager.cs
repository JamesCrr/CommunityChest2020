using System.Collections;
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
    // To store the buildings on the map
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
        int totalSize = m_currentMap.GetMapSize().x * m_currentMap.GetMapSize().y;
        m_GridTakenArray.Clear();
        m_GridTakenArray.Capacity = totalSize;     // Set the capactiy to prevent calling Array.Resize() multiple times
        for (int i = 0; i < totalSize; ++i)
            m_GridTakenArray.Add(false);
        // Center the Camera to the map
        Vector3 centerMapWorldPos = m_GridGO.CellToWorld((Vector3Int)m_currentMap.GetMapSize() / 2);
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
                if (testGridPos.x < 0 || testGridPos.x >= m_currentMap.GetMapSize().x ||
                    testGridPos.y < 0 || testGridPos.y >= m_currentMap.GetMapSize().y)
                    return false;
                // Check if spot is already taken
                arrayIndex = Convert2DToIntIndex(testGridPos);
                if (m_GridTakenArray[arrayIndex] || m_RoadManager.CheckMapAvailability(arrayIndex))
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
            int arrayIndex = Convert2DToIntIndex(key);
            m_RoadManager.PlaceRoads(arrayIndex, ref activeBuildingCom);

        }
        else
        {
            m_DictOfBuildingsOnMap[key] = activeBuildingCom;
        }
    }
    void RemoveBuildingFromTrackingDictionary(BaseBuildingsClass activeBuildingCom)
    {
        Vector2Int key = (Vector2Int)m_GridGO.WorldToCell(activeBuildingCom.GetBottomLeftGridPosition());
        if (!m_DictOfBuildingsOnMap.ContainsKey(key))
            return;
        m_DictOfBuildingsOnMap.Remove(key);
    }


    int Convert2DToIntIndex(Vector2Int v2Index) 
    {
        if (v2Index.x < 0 || v2Index.y < 0)
            return -1;
        return (v2Index.y * m_currentMap.GetMapSize().x) + v2Index.x;
    }
    Vector2Int ConvertIntIndexTo2D(int arrayIndex)
    {
        Vector2Int result = new Vector2Int();
        result.y = arrayIndex / m_currentMap.GetMapSize().x;
        result.x = arrayIndex - (result.y * m_currentMap.GetMapSize().x);
        return result;
    }
    #endregion

    #region Load From File
    public void SaveFileWasLoaded(Save_BuildingsOnMap saveFileData)
    {
        BaseBuildingsClass savedBuilding = null;
        Vector3 savedPosition = Vector3.zero;
        for(int i = 0; i < saveFileData.buildingType.Length; ++i)
        {
            savedPosition.x = saveFileData.worldPosX[i];
            savedPosition.y = saveFileData.worldPosY[i];
            savedBuilding = Instantiate(BuildingDataBase.GetInstance().GetBaseBuildingGO(), savedPosition, Quaternion.identity).GetComponent<BaseBuildingsClass>();
            savedBuilding.SetNewBuildingType(BuildingDataBase.GetInstance().GetBuildingData((BuildingDataBase.BUILDINGS)saveFileData.buildingType[i]));
            PlaceBuildingToGrid(savedBuilding);
        }
    }
    #endregion

    #region Getters
    public Grid GetGrid() { return m_GridGO; }
    public List<BaseBuildingsClass> GetBuildingsOnMap()
    {
        List<BaseBuildingsClass> listOfBuildings = new List<BaseBuildingsClass>();
        foreach (KeyValuePair<Vector2Int, BaseBuildingsClass> entry in m_DictOfBuildingsOnMap)
        {
            listOfBuildings.Add(entry.Value);
        }
        return listOfBuildings;
    }
    #endregion

}
