using System.Collections.Generic;
using UnityEngine;

public class InGame_PlayerInput : MonoBehaviour
{
    // Camera
    [Header("Camera Related")]
    [SerializeField]
    float m_MinZoomOut = 3;
    [SerializeField]
    float m_MaxZoomOut = 6;
    Vector3 m_TargetCameraPosition;
    // Placment Buildings
    [Header("Placement of Buildings")]
    BaseBuildingsClass m_PlacingBuilding = null;
    Vector2 m_BuildingPlacementOffset = Vector2.zero;
    BuildingDataBase.BUILDINGS m_BuildingSelectID = BuildingDataBase.BUILDINGS.B_POND;
    bool m_PlacmentBrushActive = true;
    bool m_RemovalBrushActive = false;
    // Removal Buildings
    [Header("Removal of Buildings")]
    List<BaseBuildingsClass> m_ListOfBuildingsToRemove = null;
    // Miscs
    bool m_MovingPlacementBuilding = false;     // Are we currently moving the Placement Building or the Camera?
    bool m_MovingSomething = false;         // Have we started moving?

    // Instance
    static InGame_PlayerInput m_Instance = null;
    public static InGame_PlayerInput GetInstance() { return m_Instance; }
    private void Awake()
    {
        if (m_Instance != null)       // Make this a Singleton Instance
        {
            Destroy(gameObject);
            return;
        }
        m_Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    void Start()
    {
        m_PlacingBuilding = Instantiate(BuildingDataBase.GetInstance().GetBaseBuildingGO(), Camera.main.transform.position, Quaternion.identity).GetComponent<BaseBuildingsClass>();
        m_PlacingBuilding.SetSpriteObjectLayer(LayerMask.NameToLayer("BuildingPlaceRef"));
        m_ListOfBuildingsToRemove = new List<BaseBuildingsClass>();
        TogglePlacmentBrush(false);
        ToggleRemovalBrush(false);

        // Subscribe to Map Generated Event
        MapManager.OnMapGenerated += MapWasGenerated;
    }

    void Update()
    {
        // Slerp Camera towards TargetPos
        Camera.main.transform.position = Vector3.Slerp(Camera.main.transform.position, m_TargetCameraPosition, Time.deltaTime * 20.0f);
        SetPlacementBuildingToGridPosition();   // Move Building with Camera

        // Detect Fingers, for mobile input
        DetectFingerInput();

#if UNITY_EDITOR || UNITY_STANDALONE
        DEBUG_MoveCameraInput();

        if (Input.GetKeyUp(KeyCode.Q))
            TogglePlacmentBrush(!m_PlacmentBrushActive, BuildingDataBase.BUILDINGS.B_POND);
        else if (Input.GetKeyUp(KeyCode.W))
            ToggleRemovalBrush(!m_RemovalBrushActive);


        // Is Placement Brush Active?
        if (m_PlacmentBrushActive)
        {
            RenderPlacementBuilding();

            if (Input.GetKeyUp(KeyCode.Space))
                PlaceBuildings();

            if (Input.GetKeyUp(KeyCode.R))
                IncrementPlacingBuilding();
        }
        // Is Removal Brush Active?
        if (m_RemovalBrushActive)
        {
            if (Input.GetMouseButtonUp(0))
            {
                RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, 20.0f, 1 << 0);
                if (hit.collider == null)
                    return;
                // Debug.Log("Raycast2D hit: " + hit.transform.gameObject.name);

                hit.transform.gameObject.GetComponent<SpriteRenderer>().color = Color.red;
                m_ListOfBuildingsToRemove.Add(hit.transform.parent.gameObject.GetComponent<BaseBuildingsClass>());
            }
            else if (Input.GetKeyUp(KeyCode.E))
            {
                for (int i = 0; i < m_ListOfBuildingsToRemove.Count; ++i)
                {
                    MapManager.GetInstance().RemoveBuildingFromGrid(m_ListOfBuildingsToRemove[i]);
                    Destroy(m_ListOfBuildingsToRemove[i].transform.gameObject);
                }
                m_ListOfBuildingsToRemove.Clear();
            }
        }

        //if (Input.GetKeyUp(KeyCode.Z))
        //    SaveSystem.SaveBuildingsOnMap(MapManager.GetInstance().GetBuildingsOnMap());
        //else if (Input.GetKeyUp(KeyCode.X))
        //    SaveSystem.LoadSavedBuildingsToMap();

#endif


    }

    #region Building Placement
    public void TogglePlacmentBrush(bool newValue, BuildingDataBase.BUILDINGS selectedBuildingID = BuildingDataBase.BUILDINGS.B_POND)
    {
        m_PlacmentBrushActive = newValue;
        m_BuildingSelectID = selectedBuildingID;
        if (m_PlacmentBrushActive)
        {
            ToggleRemovalBrush(false);
            m_PlacingBuilding.gameObject.SetActive(true);
            m_PlacingBuilding.SetNewBuildingType(BuildingDataBase.GetInstance().GetBuildingData(m_BuildingSelectID));
        }
        else
        {
            m_PlacingBuilding.gameObject.SetActive(false);

            MapManager.GetInstance().PlayerCloseAddEditorMode();
        }
    }
    void PlaceBuildings()
    {
        // Can place there?
        if (!MapManager.GetInstance().CanPlaceBuilding(m_PlacingBuilding))
            return;
        // Check if we need to create a Custom Building GO
        if (BuildingDataBase.GetInstance().GetBuildingData(m_BuildingSelectID).GetOwnCustomBuildingObject())
        {
            Destroy(m_PlacingBuilding.gameObject);
            GameObject customBuilding = BuildingDataBase.GetInstance().GetBuildingData(m_BuildingSelectID).GetOwnCustomBuildingObject();
            m_PlacingBuilding = Instantiate(customBuilding, m_PlacingBuilding.transform.position, Quaternion.identity).GetComponent<BaseBuildingsClass>();
            m_PlacingBuilding.SetNewBuildingType(BuildingDataBase.GetInstance().GetBuildingData(m_BuildingSelectID));
        }

        // Place the Building
        MapManager.GetInstance().PlaceBuildingToGrid(m_PlacingBuilding);
        // Change Sprite Layer back to default
        m_PlacingBuilding.SetSpriteObjectLayer(0);
        m_PlacingBuilding.gameObject.name = BuildingDataBase.GetInstance().GetBuildingData(m_BuildingSelectID).GetBuildingName();

        // Success in placing building, create new building for next placment
        BuildingDataBase.BUILDINGS oldID = m_PlacingBuilding.GetBuildingType();
        m_PlacingBuilding = Instantiate(BuildingDataBase.GetInstance().GetBaseBuildingGO(), Camera.main.transform.position, Quaternion.identity).GetComponent<BaseBuildingsClass>();
        m_PlacingBuilding.SetNewBuildingType(BuildingDataBase.GetInstance().GetBuildingData(oldID));
        m_PlacingBuilding.SetSpriteObjectLayer(LayerMask.NameToLayer("BuildingPlaceRef"));

    }
    void SetPlacementBuildingToGridPosition()
    {
        // Calculate actual Position on Grid
        BaseMapClass gridLayout = MapManager.GetInstance().GetCurrentMap();
        Vector3Int gridPos = gridLayout.GetTileMapCom().WorldToCell(Camera.main.transform.position + (Vector3)m_BuildingPlacementOffset);
        Vector3 newPos = gridLayout.GetTileMapCom().CellToWorld(gridPos);
        newPos += gridLayout.GetTileMapCom().cellSize * 0.5f;
        m_PlacingBuilding.transform.position = newPos;
    }
    void RenderPlacementBuilding()
    {
        // Darken if not able to place
        if(MapManager.GetInstance().CanPlaceBuilding(m_PlacingBuilding.GetBottomLeftGridPosition(), m_PlacingBuilding.GetBuildingSizeOnMap()))
            m_PlacingBuilding.SetSpriteObjectColor(Color.white);
        else
            m_PlacingBuilding.SetSpriteObjectColor(Color.gray);
    }
    void IncrementPlacingBuilding()
    {
        m_BuildingSelectID += 1;
        if (m_BuildingSelectID >= BuildingDataBase.BUILDINGS.B_TOTAL)
            m_BuildingSelectID = BuildingDataBase.BUILDINGS.B_POND;

        m_PlacingBuilding.SetNewBuildingType(BuildingDataBase.GetInstance().GetBuildingData(m_BuildingSelectID));
    }
    #endregion

    #region Building Removal
    public void ToggleRemovalBrush(bool newValue)
    {
        if(newValue)
            TogglePlacmentBrush(false);
        else
        {
            for (int i = 0; i < m_ListOfBuildingsToRemove.Count; ++i)
            {
                m_ListOfBuildingsToRemove[i].transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().color = Color.white;
            }

            MapManager.GetInstance().PlayerCloseRemovalEditorModeStop();
        }
        m_ListOfBuildingsToRemove.Clear();
        m_RemovalBrushActive = newValue;
    }
    #endregion

    #region Movement
    void DetectFingerInput()
    {
        // Check for Input
        if (MobileInput.GetInstance().GetTouchCount() == 1 && 
            MobileInput.GetInstance().GetTouchPhase() != TouchPhase.Began)     // Only one Finger
        {
            // Decide on what to move
            if(!m_MovingSomething)
            {
                // Move Camera or Building?
                if (!MobileInput.GetInstance().IsFingerTouching_GO(m_PlacingBuilding.GetSpriteGO(), 0, LayerMask.NameToLayer("BuildingPlaceRef")))
                    m_MovingPlacementBuilding = false;
                else
                    m_MovingPlacementBuilding = true;
                // Keep on moving that Object, DO NOT need to check again on next frame
                m_MovingSomething = true;
            }

            GameObject gameObj = MobileInput.GetInstance().IsFingerTouching_GO();
            if (gameObj != null)
            {
                InteractableObjBase interactableObj = gameObj.GetComponent<InteractableObjBase>();
                if (interactableObj != null)
                {
                    interactableObj.OnInteract();
                }
            }

            // Move that Object
            if (!m_MovingPlacementBuilding)
                MoveCameraInput();
            else
                MovePlacementBuildingInput();
        } 
        else if(MobileInput.GetInstance().GetTouchCount() > 1)  // More than one finger
            ZoomCameraInput();
        else    // No Fingers detected
        {
            m_MovingSomething = false;
        }

    }
    void MoveCameraInput()
    {
        float moveSpeed = 15.0f;
        Vector2 swipeDelta = MobileInput.GetInstance().GetSwipeDelta();
        swipeDelta.Normalize();
        swipeDelta = -swipeDelta * moveSpeed * Time.deltaTime;
        m_TargetCameraPosition += (Vector3)swipeDelta;
    }
    void ZoomCameraInput()
    {
        Vector2 curZeroPos = MobileInput.GetInstance().GetLastTouchedPosition();
        Vector2 swipeDeltaZero = MobileInput.GetInstance().GetSwipeDelta();
        Vector2 prevZeroPos = curZeroPos - swipeDeltaZero;

        Vector2 curOnePos = MobileInput.GetInstance().GetLastTouchedPosition(1);
        Vector2 swipeDeltaOne = MobileInput.GetInstance().GetSwipeDelta(1);
        Vector2 prevOnePos = curOnePos - swipeDeltaOne;

        float prevDist = (prevZeroPos - prevOnePos).sqrMagnitude;
        float curDist = (curZeroPos - curOnePos).sqrMagnitude;
        float zoomDiff = curDist - prevDist;
        zoomDiff *= 0.00001f;

        Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize - zoomDiff, m_MinZoomOut, m_MaxZoomOut);
    }
    void MovePlacementBuildingInput()
    {
        Vector2 lastTouched_WorldPos = Camera.main.ScreenToWorldPoint(MobileInput.GetInstance().GetLastTouchedPosition());
        Vector2 startTouch_WorldPos = Camera.main.transform.position;//Camera.main.ScreenToWorldPoint(MobileInput.GetInstance().GetStartTouchPosition());
        m_BuildingPlacementOffset = lastTouched_WorldPos - startTouch_WorldPos;
    }

    void DEBUG_MoveCameraInput()
    {
        float moveSpeed = 10.0f;
        if (Input.GetKey("left"))
            m_TargetCameraPosition.x -= moveSpeed * Time.deltaTime;
        else if (Input.GetKey("right"))
            m_TargetCameraPosition.x += moveSpeed * Time.deltaTime;
        if (Input.GetKey("up"))
            m_TargetCameraPosition.y += moveSpeed * Time.deltaTime;
        else if (Input.GetKey("down"))
            m_TargetCameraPosition.y -= moveSpeed * Time.deltaTime;
    }
    #endregion

    /// <summary>
    /// When the MapManager calls the OnMapGenerated Event
    /// </summary>
    void MapWasGenerated()
    {
        m_TargetCameraPosition = Camera.main.transform.position;
    }
}
