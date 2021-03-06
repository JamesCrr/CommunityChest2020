﻿using System.Collections.Generic;
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
    [Header("Placement of Template Buildings")]    
    Vector2 m_TemplateBuildingPlacementOffset = Vector2.zero;
    // Moving Buildings
    [Header("Movement of Placed Buildings")]
    Vector2 m_MovingPlacedBuildingOffset = Vector2.zero;

    // Miscs
    bool m_MovingTemplateBuilding = false;     // Are we currently moving the Template Building or the Camera?
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
        // Subscribe to Map Generated Event
        MapManager.OnMapGenerated += MapWasGenerated;
    }

    void Update()
    {
        // Slerp Camera towards TargetPos
        Camera.main.transform.position = Vector3.Slerp(Camera.main.transform.position, m_TargetCameraPosition, Time.deltaTime * 20.0f);
        
        // Detect Fingers, for mobile input
        DetectFingerInput();

#if UNITY_EDITOR || UNITY_STANDALONE
        DEBUG_MoveCameraInput();

        // Is Placement Brush Active?
        if (MapManager.GetInstance().GetPlacementBrushActive())
        {
            SetTemplateBuildingToGridPosition();   // Move Building with Camera

            RenderTemplateBuilding();
        }
        // Is Removal Brush Active?
        else if (MapManager.GetInstance().GetRemovalBrushActive())
        {
            // THIS ONLY WORKS ON PC
            if (Input.GetMouseButtonUp(0))
            {
                RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, 20.0f, 1 << 0);
                if (hit.collider == null)
                    return;
                // Debug.Log("Raycast2D hit: " + hit.transform.gameObject.name);
                
                // Should put buildings on seperate layer honestly...
                BaseBuildingsClass hitBuilding = hit.transform.parent.gameObject.GetComponent<BaseBuildingsClass>();
                if (hitBuilding == null)
                    return;
                // Can building be Removed?
                if (!hitBuilding.GetCanBeRemoved()) 
                    return;
                hit.transform.gameObject.GetComponent<SpriteRenderer>().color = Color.red;
                MapManager.GetInstance().AddBuildingToBeRemoved(hit.transform.parent.gameObject.GetComponent<BaseBuildingsClass>());
            }
            //else if (Input.GetKeyUp(KeyCode.E))
            //    MapManager.GetInstance().RemoveBuildingsFromMapUnderList();
        }
        // Is Movement Brush Active?
        else if (MapManager.GetInstance().GetMovementBrushActive())
        {
            if (MapManager.GetInstance().GetBuildingToMove() == null)
            {
                // THIS ONLY WORKS ON PC
                if (Input.GetMouseButtonUp(0))
                {
                    RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, 20.0f, 1 << 0);
                    if (hit.collider == null)
                        return;
                    // Debug.Log("Raycast2D hit: " + hit.transform.gameObject.name);

                    // Should put buildings on seperate layer honestly...
                    BaseBuildingsClass hitBuilding = hit.transform.parent.gameObject.GetComponent<BaseBuildingsClass>();
                    if (hitBuilding == null)
                        return;
                    // Can building be Moved?
                    if (!hitBuilding.GetCanBeMoved())
                        return;
                    // Attach it and calculate offset
                    MapManager.GetInstance().SetNewBuildingToMove(hitBuilding);
                    m_MovingPlacedBuildingOffset = hitBuilding.transform.position - Camera.main.transform.position;
                }
                return;
            }
            // Make building to move follow camera and grid position
            SetMovingBuildingToGridPosition();

            // PC INPUTS
            // Can place?
            if (MapManager.GetInstance().CanPlaceBuildingToMove())
            {
                MapManager.GetInstance().GetBuildingToMove().transform.GetChild(0).GetComponent<SpriteRenderer>().color = Color.cyan;
                //// Confirm
                //if (Input.GetKeyUp(KeyCode.O))
                //{
                //    MapManager.GetInstance().ConfirmRemovementOfBuilding();
                //}
            }
            else
            {
                MapManager.GetInstance().GetBuildingToMove().transform.GetChild(0).GetComponent<SpriteRenderer>().color = Color.gray;
            }
            // Cancel
            if (Input.GetKeyUp(KeyCode.P))
                MapManager.GetInstance().CancelRemovementOfBuilding();
        }

        // Saving and Loading
        //if (Input.GetKeyUp(KeyCode.Z))
        //    SaveSystem.SaveToFile();
        //else if (Input.GetKeyUp(KeyCode.X))
        //    SaveSystem.LoadFromFile();
#endif


    }

    #region Building Placement
    void PlaceBuildings()
    {
        // Can place there?
        if (!MapManager.GetInstance().CanPlaceTemplateBuilding())
            return;
       
        // Place the Template Building
        MapManager.GetInstance().PlaceTemplateBuilding();
    }
    /// <summary>
    /// Snaps the Template to a Grid Position
    /// </summary>
    void SetTemplateBuildingToGridPosition()
    {
        // Calculate actual Position on Grid
        BaseMapClass gridLayout = MapManager.GetInstance().GetCurrentMap();
        // Follow Camera Position
        Vector3Int gridPos = gridLayout.GetTileMapCom().WorldToCell(Camera.main.transform.position + (Vector3)m_TemplateBuildingPlacementOffset);
        Vector3 newPos = gridLayout.GetTileMapCom().CellToWorld(gridPos);
        newPos += gridLayout.GetTileMapCom().cellSize * 0.5f;
        MapManager.GetInstance().GetTemplateBuilding().transform.position = newPos;
    }
    /// <summary>
    /// Darkens the Template Building if unable to place
    /// Modifies the Sorting order of the Building
    /// </summary>
    void RenderTemplateBuilding()
    {
        // Darken if not able to place
        if (MapManager.GetInstance().CanPlaceTemplateBuilding())
            MapManager.GetInstance().GetTemplateBuilding().SetSpriteObjectColor(Color.white);
        else
            MapManager.GetInstance().GetTemplateBuilding().SetSpriteObjectColor(Color.gray);

        // Set Sprite's Sorting Order
        int newOrder = (int)MapManager.GetInstance().GetTemplateBuilding().GetBottomLeftGridPosition().y;
        newOrder = -newOrder;
        MapManager.GetInstance().GetTemplateBuilding().SetSpriteSortingOrder(newOrder);
    }
    #endregion

    #region Moving Placed Buildings
    void SetMovingBuildingToGridPosition()
    {
        // Calculate actual Position on Grid
        BaseMapClass gridLayout = MapManager.GetInstance().GetCurrentMap();
        // Follow Camera Position
        Vector3Int gridPos = gridLayout.GetTileMapCom().WorldToCell(Camera.main.transform.position + (Vector3)m_MovingPlacedBuildingOffset);
        Vector3 newPos = gridLayout.GetTileMapCom().CellToWorld(gridPos);
        newPos += gridLayout.GetTileMapCom().cellSize * 0.5f;
        MapManager.GetInstance().GetBuildingToMove().transform.position = newPos;
    }
    #endregion

    #region Detect Touch Input
    /// <summary>
    /// Detects Player Input and decides what to do with it
    /// </summary>
    void DetectFingerInput()
    {
        // Check for Touch Input
        if (MobileInput.GetInstance().GetTouchCount() == 1 && 
            MobileInput.GetInstance().GetTouchPhase() != TouchPhase.Began)     // Only one Finger
        {
            // Decide on what to move
            if(!m_MovingSomething)
            {
                // Move Camera or Building?
                if (MapManager.GetInstance().GetPlacementBrushActive())
                {
                    if (!MobileInput.GetInstance().IsFingerTouching_GO(MapManager.GetInstance().GetTemplateBuilding().GetSpriteGO(), 0, LayerMask.NameToLayer("BuildingPlaceRef")))
                        m_MovingTemplateBuilding = false;
                    else
                        m_MovingTemplateBuilding = true;
                }
                else if(MapManager.GetInstance().GetMovementBrushActive())
                {

                }
                else
                    m_MovingTemplateBuilding = false;
             
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
                else
                {
                    interactableObj = gameObj.GetComponent<InteractableObjBase>();
                    if (interactableObj)
                        interactableObj.OnInteract();
                }
            }

            // Move that Object
            if (!m_MovingTemplateBuilding)
                MoveCameraInput();
            else
                MovePlacementTemplateBuildingInput();
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
    void MovePlacementTemplateBuildingInput()
    {
        Vector2 lastTouched_WorldPos = Camera.main.ScreenToWorldPoint(MobileInput.GetInstance().GetLastTouchedPosition());
        Vector2 startTouch_WorldPos = Camera.main.transform.position;//Camera.main.ScreenToWorldPoint(MobileInput.GetInstance().GetStartTouchPosition());
        m_TemplateBuildingPlacementOffset = lastTouched_WorldPos - startTouch_WorldPos;
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
