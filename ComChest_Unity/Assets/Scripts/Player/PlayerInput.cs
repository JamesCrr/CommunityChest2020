using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    // Camera
    [SerializeField]
    float minZoomOut = 3;
    [SerializeField]
    float maxZoomOut = 6;
    Vector3 targetCameraPosition;
    // Placment
    BaseBuildingsClass selectedBuilding = null;
    bool brushActive = true;

    // Start is called before the first frame update
    void Start()
    {
        selectedBuilding = Instantiate(BuildingDataBase.GetInstance().GetBuildingGO(BuildingDataBase.BUILDINGS.B_POND), Camera.main.transform.position, Quaternion.identity).GetComponent<BaseBuildingsClass>();
        selectedBuilding.gameObject.SetActive(false);

        targetCameraPosition = Camera.main.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        DetectInput();

        if (brushActive)
        {
            RenderSelectBuilding();

            PlaceBuildings();

            if (Input.GetKeyUp(KeyCode.R))
                IncrementSelectedBuilding();
        }

        //Grid gridLayout = MapManager.GetInstance().GetGrid();
        //Vector3Int gridPos = gridLayout.WorldToCell(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        //Debug.Log("MOUSE ON: " + gridPos);
    }

    void PlaceBuildings()
    {
        if (Input.GetKeyUp(KeyCode.Space))
        {
            Grid gridLayout = MapManager.GetInstance().GetGrid();
            Vector3 worldPos = Camera.main.transform.position;//Camera.main.ScreenToWorldPoint(Input.mousePosition);
            worldPos.z = 0.0f;
            Vector3Int gridPos = gridLayout.WorldToCell(worldPos);

            //MapManager.GetInstance().PlaceBuilding(BuildingDataBase.BUILDINGS.B_POND, gridPos + (gridLayout.cellSize * 0.5f));

            if(MapManager.GetInstance().PlaceBuilding(selectedBuilding))
            {
                // if success in placing building, create new building to put
                selectedBuilding = Instantiate(BuildingDataBase.GetInstance().GetBuildingGO(selectedBuilding.GetBuildingID()), Camera.main.transform.position, Quaternion.identity).GetComponent<BaseBuildingsClass>();
            }

            //pondBrush.PlaceGO(targetTileMap, targetTileMap.gameObject, gridPos, 0);
            //GameObject newObject = pondBrush.GetGOInCell(targetTileMap, targetTileMap.transform, gridPos);
            //Debug.Log(newObject.name);
        }
    }
    void RenderSelectBuilding()
    {
        selectedBuilding.gameObject.SetActive(true);

        Grid gridLayout = MapManager.GetInstance().GetGrid();
        Vector3Int gridPos = gridLayout.WorldToCell(Camera.main.transform.position);
        Vector3 newPos = gridLayout.CellToWorld(gridPos);
        newPos += gridLayout.cellSize * 0.5f;
        selectedBuilding.transform.position = newPos;

        if(MapManager.GetInstance().CanPlaceBuilding(selectedBuilding.GetBottomLeftRefPosition(), selectedBuilding.GetBuildingSize()))
            selectedBuilding.GetBuildingSpriteRenderer().color = Color.white;
        else
            selectedBuilding.GetBuildingSpriteRenderer().color = Color.gray;
    }
    void IncrementSelectedBuilding()
    {
        BuildingDataBase.BUILDINGS prevID = BuildingDataBase.BUILDINGS.B_POND;
        if (selectedBuilding)
        {
            prevID = selectedBuilding.GetBuildingID();
            Destroy(selectedBuilding.gameObject);
        }
        prevID += 1;
        if (prevID >= BuildingDataBase.BUILDINGS.B_TOTAL)
            prevID = BuildingDataBase.BUILDINGS.B_POND;

        selectedBuilding = Instantiate(BuildingDataBase.GetInstance().GetBuildingGO(prevID), Camera.main.transform.position, Quaternion.identity).GetComponent<BaseBuildingsClass>();
        

    }


    void DetectInput()
    {
        // Check for Input
        if (MobileInput.GetInstance().GetTouchCount() < 1)
            return;
        // Move Camera or Building?
        if (!MobileInput.GetInstance().IsFingerTouching_GO(selectedBuilding.gameObject))
            UpdateCamera();
        else
        {

        }

    }
    void UpdateCamera()
    {
        // Slerp Camera towards TargetPos
        Camera.main.transform.position = Vector3.Slerp(Camera.main.transform.position, targetCameraPosition, Time.deltaTime * 20.0f);

        if (MobileInput.GetInstance().GetTouchCount() == 1)
            MoveCameraInput();
        else
            ZoomCameraInput();
    }
    void MoveCameraInput()
    {
        //float moveSpeed = 10.0f;
        //Vector3 cameraPos = Camera.main.transform.position;
        //if (Input.GetKey("left"))
        //    cameraPos.x -= moveSpeed * Time.deltaTime;
        //else if(Input.GetKey("right"))
        //    cameraPos.x += moveSpeed * Time.deltaTime;
        //if (Input.GetKey("up"))
        //    cameraPos.y += moveSpeed * Time.deltaTime;
        //else if (Input.GetKey("down"))
        //    cameraPos.y -= moveSpeed * Time.deltaTime;
        //Camera.main.transform.position = cameraPos;

        float moveSpeed = 15.0f;
        Vector2 swipeDelta = MobileInput.GetInstance().GetSwipeDelta();
        swipeDelta.Normalize();
        swipeDelta = -swipeDelta * moveSpeed * Time.deltaTime;
        targetCameraPosition += (Vector3)swipeDelta;
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

        Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize - zoomDiff, minZoomOut, maxZoomOut);

    }
}
