using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    [Header("Grid and TileMap")]
    [SerializeField]        // Where the Tilemaps will be rendered
    Grid m_GridGO = null;
    BaseMapClass m_currentMap;
    List<bool> m_GridTakenArray;    // To store which tiles on map are taken by building        
    // To store the buildings currently on the map
    Dictionary<Vector2Int, BaseBuildingsClass> m_DictOfBuildingsOnMap = new Dictionary<Vector2Int,BaseBuildingsClass>();
    [Header("Placement of Buildings")]      // Handle placement of Buildings into Map
    [SerializeField] Transform m_BuildingParent;
    BaseBuildingsClass m_TemplateBuilding = null;
    BuildingDataBase.BUILDINGS m_TemplateBuildingID = BuildingDataBase.BUILDINGS.B_POND;
    bool m_PlacmentBrushActive = false;
    [Header("Removal of Buildings")]        // Handle removal of Buildings from Map
    List<BaseBuildingsClass> m_ListOfBuildingsToRemove = null;
    bool m_RemovalBrushActive = false;
    // Map Generated Action
    public delegate void MapGeneratedAction();      
    public static event MapGeneratedAction OnMapGenerated;

    [Header("Roads")]
    [SerializeField]
    BaseBuildingsClass m_MainRoad; //the main road, not deleatable
    [SerializeField] Transform m_RoadParent;
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

        // Placement and Removal
        m_TemplateBuilding = Instantiate(BuildingDataBase.GetInstance().GetBaseBuildingGO(), Camera.main.transform.position, Quaternion.identity).GetComponent<BaseBuildingsClass>();
        m_TemplateBuilding.SetSpriteObjectLayer(LayerMask.NameToLayer("BuildingPlaceRef"));
        m_ListOfBuildingsToRemove = new List<BaseBuildingsClass>();

        // Parent the m_GridGO
        m_GridGO.transform.parent = transform;
    }
    private void Start()
    {
        SetPlacementBrush(false);
        SetRemovalBrush(false);

        CreateNewMap(MapDataBase.MAPS.M_GRASS);

        //init road stuff
        Vector2Int mainRoadPos = Vector2Int.zero;
        if (m_MainRoad != null)
            mainRoadPos = (Vector2Int)m_GridGO.WorldToCell(m_MainRoad.GetBottomLeftGridPosition());

        m_RoadManager.Init(m_currentMap.GetTileMapSize(), mainRoadPos);

        if (m_MainRoad != null)
            PlaceBuildingToGrid(ref m_MainRoad);
    }

    #region Map Related
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
    #endregion

    #region Placement and Removal of Buildings into/from Map
    /// <summary>
    /// Places the Template Building into the Map
    /// </summary>
    /// <param name="doChecking">Are any of the spots taken by another building already?</param>
    /// <returns></returns>
    public BaseBuildingsClass PlaceTemplateBuilding(bool doChecking = false)
    {
        BaseBuildingsClass PlacedBuilding = PlaceBuildingToGrid(ref m_TemplateBuilding, doChecking);

        // Success in placing building, create new building for next placment
        BuildingDataBase.BUILDINGS oldID = m_TemplateBuilding.GetBuildingType();
        m_TemplateBuilding = Instantiate(BuildingDataBase.GetInstance().GetBaseBuildingGO(), Camera.main.transform.position, Quaternion.identity).GetComponent<BaseBuildingsClass>();
        m_TemplateBuilding.SetNewBuildingType(BuildingDataBase.GetInstance().GetBuildingData(oldID));
        m_TemplateBuilding.SetSpriteObjectLayer(LayerMask.NameToLayer("BuildingPlaceRef"));

        return PlacedBuilding;
    }
    public BaseBuildingsClass PlaceNewBuildingIntoMap(Vector2 spawnWorldPosition, BuildingDataBase.BUILDINGS buildingID)
    {
        // Convert World to Grid Coordinates
        BaseMapClass gridLayout = GetCurrentMap();
        Vector3Int gridPos = gridLayout.GetTileMapCom().WorldToCell(spawnWorldPosition);
        spawnWorldPosition = gridLayout.GetTileMapCom().CellToWorld(gridPos);
        spawnWorldPosition += (Vector2)gridLayout.GetTileMapCom().cellSize * 0.5f;
        // Check if we can place building Down
        Vector3 buildingBottomLeft = spawnWorldPosition + BuildingDataBase.GetInstance().GetBuildingData(buildingID).GetBottomLeftCorner_PositionOffset();
        Vector2Int buildingSize = BuildingDataBase.GetInstance().GetBuildingData(buildingID).GetBuildingSizeOnMap();
        if (!CanPlaceBuilding(buildingBottomLeft, buildingSize))
        {
            Debug.LogError("Unable to Place Building, Canceling...");
            return null;
        }
        // Create the Building
        return PlaceBuildingToGrid(spawnWorldPosition, buildingID);
    }
    /// <summary>
    /// Adds a Building to the List to be Removed from Map
    /// </summary>
    /// <param name="buildingToRemove"></param>
    public void AddBuildingToBeRemoved(BaseBuildingsClass buildingToRemove)
    {
        m_ListOfBuildingsToRemove.Add(buildingToRemove);
    }
    /// <summary>
    /// Removes all Buildings from Map in List
    /// </summary>
    public void RemoveBuildingsFromMapUnderList()
    {
        for (int i = 0; i < m_ListOfBuildingsToRemove.Count; ++i)
        {
            RemoveBuildingFromGrid(m_ListOfBuildingsToRemove[i]);
            Destroy(m_ListOfBuildingsToRemove[i].transform.gameObject);
        }
        m_ListOfBuildingsToRemove.Clear();
    }
    /// <summary>
    /// Tries to place a Building into the Grid from it's world Position.
    /// Returns null if Unable to, returns the newly created GO otherwise
    /// </summary>
    /// <param name="activeBuildingCom">The Building GO to place</param>
    /// <param name="doChecking">Are any of the spots taken by another building already?</param>
    /// <returns></returns>
    BaseBuildingsClass PlaceBuildingToGrid(ref BaseBuildingsClass activeBuildingCom, bool doChecking = false)
    {
        // Check if we need to create a Custom Building GO
        if (BuildingDataBase.GetInstance().GetBuildingData(activeBuildingCom.GetBuildingType()).GetOwnCustomBuildingObject())
        {
            Vector3 buildingPos = activeBuildingCom.transform.position;
            BuildingDataBase.BUILDINGS buildingID = activeBuildingCom.GetBuildingType();
            Destroy(activeBuildingCom.gameObject);
            GameObject customBuilding = BuildingDataBase.GetInstance().GetBuildingData(buildingID).GetOwnCustomBuildingObject();
            activeBuildingCom = Instantiate(customBuilding, buildingPos, Quaternion.identity).GetComponent<BaseBuildingsClass>();
            activeBuildingCom.SetNewBuildingType(BuildingDataBase.GetInstance().GetBuildingData(buildingID));
        }

        Vector3 buildingBottomLeftWorldPos = activeBuildingCom.GetBottomLeftGridPosition();
        Vector2Int buildingSize = activeBuildingCom.GetBuildingSizeOnMap();
        //Debug.Log("GRID POS: " + m_GridGO.WorldToCell(buildingWorldPos));
        // Can we place it there?
        if (doChecking)
            if (!CanPlaceBuilding(buildingBottomLeftWorldPos, buildingSize))
                return null;
        // Set all the grids taken by new building to true
        SetGridTakenArray(buildingBottomLeftWorldPos, buildingSize, true);

        AddBuildingIntoTrackingDictionary(activeBuildingCom); //roads and buildings store accordingly
        activeBuildingCom.BuildingPlaced();

        // Change Sprite Layer back to building layer
        activeBuildingCom.SetSpriteObjectLayer(1);
        activeBuildingCom.gameObject.name = BuildingDataBase.GetInstance().GetBuildingData(activeBuildingCom.GetBuildingType()).GetBuildingName();

        //set the correct parent
        if (activeBuildingCom.GetBuildingType() == BuildingDataBase.BUILDINGS.B_ROAD)
        {
            activeBuildingCom.gameObject.transform.SetParent(m_RoadParent);
        }
        else
        {
            activeBuildingCom.gameObject.transform.SetParent(m_BuildingParent);
        }

        return activeBuildingCom;
    }
    BaseBuildingsClass PlaceBuildingToGrid(Vector2 spawnWorldPos, BuildingDataBase.BUILDINGS buildingID, bool doChecking = false)
    {
        // Check if we need to create a Custom Building GO
        BaseBuildingsClass newBuilding = null;
        if (BuildingDataBase.GetInstance().GetBuildingData(buildingID).GetOwnCustomBuildingObject())
        {
            GameObject customBuilding = BuildingDataBase.GetInstance().GetBuildingData(buildingID).GetOwnCustomBuildingObject();
            newBuilding = Instantiate(customBuilding, spawnWorldPos, Quaternion.identity).GetComponent<BaseBuildingsClass>();
        }
        else
            newBuilding = Instantiate(BuildingDataBase.GetInstance().GetBaseBuildingGO(), spawnWorldPos, Quaternion.identity).GetComponent<BaseBuildingsClass>();
        newBuilding.SetNewBuildingType(BuildingDataBase.GetInstance().GetBuildingData(buildingID));

        Vector3 buildingBottomLeftWorldPos = newBuilding.GetBottomLeftGridPosition();
        Vector2Int buildingSize = newBuilding.GetBuildingSizeOnMap();
        // Can we place it there?
        if (doChecking)
            if (!CanPlaceBuilding(buildingBottomLeftWorldPos, buildingSize))
                return null;
        // Set all the grids taken by new building to true
        SetGridTakenArray(buildingBottomLeftWorldPos, buildingSize, true);

        AddBuildingIntoTrackingDictionary(newBuilding); //roads and buildings store accordingly
        newBuilding.BuildingPlaced();

        //set the correct parent
        if (newBuilding.GetBuildingType() == BuildingDataBase.BUILDINGS.B_ROAD)
        {
            newBuilding.gameObject.transform.SetParent(m_RoadParent);
        }
        else
        {
            newBuilding.gameObject.transform.SetParent(m_BuildingParent);
        }

        // Change Sprite Layer back to default
        newBuilding.SetSpriteObjectLayer(0);
        newBuilding.gameObject.name = BuildingDataBase.GetInstance().GetBuildingData(buildingID).GetBuildingName();

        return newBuilding;
    }
    /// <summary>
    /// Removes the Building from Grid by setting it's array spots back to untaken, then returns it
    /// </summary>
    /// <param name="activeBuildingCom"></param>
    /// <returns></returns>
    GameObject RemoveBuildingFromGrid(BaseBuildingsClass activeBuildingCom)
    {
        Vector3 buildingBottomLeftWorldPos = activeBuildingCom.GetBottomLeftGridPosition();
        Vector2Int buildingSize = activeBuildingCom.GetBuildingSizeOnMap();
        // Set all the grids taken by building to false, since no longer there
        SetGridTakenArray(buildingBottomLeftWorldPos, buildingSize, false);
        RemoveBuildingFromTrackingDictionary(activeBuildingCom);
        activeBuildingCom.BuildingRemoved();

        return activeBuildingCom.gameObject;
    }
    #endregion

    #region Toggle Placement and Removal of Buildings MODE
    /// <summary>
    /// Enables/Disables Placement Brush to be used for placing down New Buildings in Map.
    /// Auto Disables Removal Brush when enabling
    /// </summary>
    /// <param name="newValue">True=Enable, False=Disable</param>
    /// <param name="selectedBuildingID">What Building Type to Start with</param>
    public void SetPlacementBrush(bool newValue, BuildingDataBase.BUILDINGS selectedBuildingID = BuildingDataBase.BUILDINGS.B_POND)
    {
        m_PlacmentBrushActive = newValue;
        m_TemplateBuildingID = selectedBuildingID;
        if (m_PlacmentBrushActive)
        {
            SetRemovalBrush(false);
            m_TemplateBuilding.gameObject.SetActive(true);
            m_TemplateBuilding.SetNewBuildingType(BuildingDataBase.GetInstance().GetBuildingData(m_TemplateBuildingID));

            PlayerOpenAddEditorMode(m_TemplateBuilding);
        }
        else
        {
            m_TemplateBuilding.gameObject.SetActive(false);
            PlayerCloseAddEditorMode();
        }
    }
    public void IncrementPlacingBuildingID()
    {
        m_TemplateBuildingID += 1;
        if (m_TemplateBuildingID >= BuildingDataBase.BUILDINGS.B_TOTAL)
            m_TemplateBuildingID = BuildingDataBase.BUILDINGS.B_POND;
        m_TemplateBuilding.SetNewBuildingType(BuildingDataBase.GetInstance().GetBuildingData(m_TemplateBuildingID));
    }
    /// <summary>
    /// Enables/Disables Removal Brush to be used for Removing Buildings from Map.
    /// Auto Disables Placement Brush when enabling
    /// </summary>
    /// <param name="newValue">True=Enable, False=Disable</param>
    public void SetRemovalBrush(bool newValue)
    {
        if (newValue)
        {
            SetPlacementBrush(false);
            PlayerOpenRemovalEditorModeStop();
        }
        else
        {
            for (int i = 0; i < m_ListOfBuildingsToRemove.Count; ++i)
            {
                m_ListOfBuildingsToRemove[i].transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().color = Color.white;
            }
            PlayerCloseRemovalEditorModeStop();
        }
        m_ListOfBuildingsToRemove.Clear();
        m_RemovalBrushActive = newValue;
    }

    /// <summary>
    /// When player is out of adding buildings editor mode
    /// </summary>
    public void PlayerCloseAddEditorMode()
    {
        //roadmanager checks if anything is added during the editor session
        if (m_RoadManager != null)
            m_RoadManager.CheckAndInvokeAddingOfRoadsCallback();

        if (NPCManager.Instance != null)
            NPCManager.Instance.PlayerEditorModeActive(false);

        IngameUIManager.instance.BuildModeUIClose();
    }

    public void PlayerOpenAddEditorMode(BaseBuildingsClass building)
    {
        //open NPC player editor mode
        if (NPCManager.Instance != null)
            NPCManager.Instance.PlayerEditorModeActive(true);

        //set UI
        if (building != null)
        {
            SpriteRenderer buildingSprite = building.GetSpriteRenderer();
            if (buildingSprite != null)
            {
                Vector2 uiOffset = new Vector2(0.0f, buildingSprite.bounds.size.y / 2.0f);
                IngameUIManager.instance.BuildModeUIOpen(building.GetSpriteGO().transform, uiOffset, building.GetBuildingSizeOnMap());
            }
        }
    }

    /// <summary>
    /// When player is out of removing editor mode
    /// </summary>
    public void PlayerCloseRemovalEditorModeStop()
    {
        //roadmanager checks if anything is removed during the editor session
        if (m_RoadManager != null)
            m_RoadManager.CheckAndInvokeRemovalOfRoadsCallback();

        //close NPC player editor mode
        if (NPCManager.Instance != null)
            NPCManager.Instance.PlayerEditorModeActive(false);
    }

    public void PlayerOpenRemovalEditorModeStop()
    {
        //open NPC player editor mode
        if (NPCManager.Instance != null)
            NPCManager.Instance.PlayerEditorModeActive(true);
    }

    #endregion

    //private void Update()
    //{
    //    Vector3Int gridPos = m_currentMap.GetTileMapCom().WorldToCell(Camera.main.ScreenToWorldPoint(Input.mousePosition));
    //    Debug.Log("GridPOs: " + gridPos + "\n" + m_currentMap.GetTileMapCom().HasTile(gridPos));
    //    Debug.Log("Tile at : " + gridPos + "\n" + m_currentMap.GetTileMapCom().GetTile(gridPos));
    //}

    #region Misc
    /// <summary>
    /// Checks if any of the spots the building will occupy, is already taken
    /// </summary>
    /// <param name="buildingBottomLeftWorldPos">Bottom Left Corner of the Building</param>
    /// <param name="buildingSize">How many grids does this building take</param>
    /// <returns>True if can place Building</returns>
    public bool CanPlaceTemplateBuilding()
    {
        return CanPlaceBuilding(m_TemplateBuilding.GetBottomLeftGridPosition(), m_TemplateBuilding.GetBuildingSizeOnMap());
    }
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

    #region CheckingRoadConnections
    public bool CheckRoadConnection(Vector2Int startPt, Vector2Int endPt)
    {
        return m_RoadManager.CheckRoadConnection(startPt, endPt);
    }

    public bool CheckRoadConnectionToMainRoad(Vector2Int startPt)
    {
        return m_RoadManager.CheckRoadConnectionToMainRoad(startPt);
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
            PlaceBuildingToGrid(ref savedBuilding);
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
    public Vector2Int GetWorldPosToCellPos(Vector2 pos) { return (Vector2Int)m_GridGO.WorldToCell(pos); }
    public Vector2 GetCellCentrePosToWorld(Vector2Int pos) { return m_GridGO.GetCellCenterWorld((Vector3Int)pos);  }
    public BaseMapClass GetCurrentMap() { return m_currentMap;  }
    public BaseBuildingsClass GetTemplateBuilding() { return m_TemplateBuilding; }
    public bool GetPlacementBrushActive() { return m_PlacmentBrushActive; }
    public bool GetRemovalBrushActive() { return m_RemovalBrushActive; }
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
    public RoadManager GetRoadManager()
    {
        return m_RoadManager;
    }
    #endregion
}
