using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    // Camera
    [Header("Camera Related")]
    [SerializeField]
    float m_MinZoomOut = 3;
    [SerializeField]
    float m_MaxZoomOut = 6;
    Vector3 m_TargetCameraPosition;
    // Placment
    [Header("Placement of Buildings")]
    [SerializeField]
    GameObject m_baseBuildingGO = null;
    BaseBuildingsClass m_PlacingBuilding = null;
    Vector2 m_BuildingPlacementOffset = Vector2.zero;
    BuildingDataBase.BUILDINGS buildingSelectID = BuildingDataBase.BUILDINGS.B_POND;
    bool m_BrushActive = true;
    // Miscs
    bool m_MovingPlacementBuilding = false;     // Are we currently moving the Placement Building or the Camera?
    bool m_MovingSomething = false;         // Have we started moving?


    void Start()
    {
        m_PlacingBuilding = Instantiate(m_baseBuildingGO, Camera.main.transform.position, Quaternion.identity).GetComponent<BaseBuildingsClass>();
        m_PlacingBuilding.SetSpriteObjectLayer(LayerMask.NameToLayer("BuildingPlaceRef"));
        TogglePlacmentBrush(false);

        // Subscribe to Map Generated Event
        MapManager.OnMapGenerated += MapWasGenerated;
    }

    void Update()
    {
        // Slerp Camera towards TargetPos
        Camera.main.transform.position = Vector3.Slerp(Camera.main.transform.position, m_TargetCameraPosition, Time.deltaTime * 20.0f);
        SetPlacementBuildingToGridPosition();   // Move Building with Camera

        //Detect Fingers, for mobile input
        DetectFingerInput();

        // Is Brush Active?
        if (m_BrushActive)
        {
            RenderPlacementBuilding();

            PlaceBuildings();

            if (Input.GetKeyUp(KeyCode.R))
                IncrementPlacingBuilding();
        }

        if (Input.GetKeyUp(KeyCode.Q))
            TogglePlacmentBrush(!m_BrushActive, BuildingDataBase.BUILDINGS.B_POND);

        //Grid gridLayout = MapManager.GetInstance().GetGrid();
        //Vector3Int gridPos = gridLayout.WorldToCell(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        //Debug.Log("MOUSE ON: " + gridPos);
    }

    #region Building Placement
    void TogglePlacmentBrush(bool newValue, BuildingDataBase.BUILDINGS selectedBuildingID = BuildingDataBase.BUILDINGS.B_POND)
    {
        m_BrushActive = newValue;
        buildingSelectID = selectedBuildingID;
        if (m_BrushActive)
        {
            m_PlacingBuilding.gameObject.SetActive(true);
            m_PlacingBuilding.SetNewBuildingType(BuildingDataBase.GetInstance().GetBuildingData(buildingSelectID));
        }
        else
        {
            m_PlacingBuilding.gameObject.SetActive(false);
        }
    }
    void PlaceBuildings()
    {
        if (Input.GetKeyUp(KeyCode.Space))
        {
            Grid gridLayout = MapManager.GetInstance().GetGrid();
            Vector3 worldPos = Camera.main.transform.position;//Camera.main.ScreenToWorldPoint(Input.mousePosition);
            worldPos.z = 0.0f;
            Vector3Int gridPos = gridLayout.WorldToCell(worldPos);

            // Can place there?
            if (!MapManager.GetInstance().CanPlaceBuilding(m_PlacingBuilding))
                return;
            // Check if we need to create a Custom Building GO
            if (BuildingDataBase.GetInstance().GetBuildingData(buildingSelectID).GetOwnCustomBuildingObject())
            {
                Destroy(m_PlacingBuilding.gameObject);
                GameObject customBuilding = BuildingDataBase.GetInstance().GetBuildingData(buildingSelectID).GetOwnCustomBuildingObject();
                m_PlacingBuilding = Instantiate(customBuilding, m_PlacingBuilding.transform.position, Quaternion.identity).GetComponent<BaseBuildingsClass>();
                m_PlacingBuilding.SetNewBuildingType(BuildingDataBase.GetInstance().GetBuildingData(buildingSelectID));
            }

            // Place the Building
            MapManager.GetInstance().PlaceBuildingToGrid(m_PlacingBuilding);
            // Change Sprite Layer back to default
            m_PlacingBuilding.SetSpriteObjectLayer(0);
            m_PlacingBuilding.gameObject.name = BuildingDataBase.GetInstance().GetBuildingData(buildingSelectID).GetBuildingName();

            // Success in placing building, create new building for next placment
            BuildingDataBase.BUILDINGS oldID = m_PlacingBuilding.GetBuildingID();
            m_PlacingBuilding = Instantiate(m_baseBuildingGO, Camera.main.transform.position, Quaternion.identity).GetComponent<BaseBuildingsClass>();
            m_PlacingBuilding.SetNewBuildingType(BuildingDataBase.GetInstance().GetBuildingData(oldID));
            m_PlacingBuilding.SetSpriteObjectLayer(LayerMask.NameToLayer("BuildingPlaceRef"));
        }
    }
    void SetPlacementBuildingToGridPosition()
    {
        // Calculate actual Position on Grid
        Grid gridLayout = MapManager.GetInstance().GetGrid();
        Vector3Int gridPos = gridLayout.WorldToCell(Camera.main.transform.position + (Vector3)m_BuildingPlacementOffset);
        Vector3 newPos = gridLayout.CellToWorld(gridPos);
        newPos += gridLayout.cellSize * 0.5f;
        m_PlacingBuilding.transform.position = newPos;
    }
    void RenderPlacementBuilding()
    {
        // Darken if not able to place
        if(MapManager.GetInstance().CanPlaceBuilding(m_PlacingBuilding.GetBottomLeftGridPosition(), m_PlacingBuilding.GetBuildingSize()))
            m_PlacingBuilding.SetSpriteObjectColor(Color.white);
        else
            m_PlacingBuilding.SetSpriteObjectColor(Color.gray);
    }
    void IncrementPlacingBuilding()
    {
        buildingSelectID += 1;
        if (buildingSelectID >= BuildingDataBase.BUILDINGS.B_TOTAL)
            buildingSelectID = BuildingDataBase.BUILDINGS.B_POND;

        m_PlacingBuilding.SetNewBuildingType(BuildingDataBase.GetInstance().GetBuildingData(buildingSelectID));
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

    void DEBUGMoveCameraInput()
    {
        float moveSpeed = 10.0f;
        Vector3 cameraPos = Camera.main.transform.position;
        if (Input.GetKey("left"))
            cameraPos.x -= moveSpeed * Time.deltaTime;
        else if (Input.GetKey("right"))
            cameraPos.x += moveSpeed * Time.deltaTime;
        if (Input.GetKey("up"))
            cameraPos.y += moveSpeed * Time.deltaTime;
        else if (Input.GetKey("down"))
            cameraPos.y -= moveSpeed * Time.deltaTime;
        Camera.main.transform.position = cameraPos;
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
