using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Tilemaps;
using UnityEngine.Tilemaps;

public class TestPlace : MonoBehaviour
{
    BaseBuildingsClass selectedBuilding = null;
    bool brushActive = false;
    bool eraserActive = false;

    // Start is called before the first frame update
    void Start()
    {
        selectedBuilding = Instantiate(BuildingDataBase.GetInstance().GetBuildingGO(BuildingDataBase.BUILDINGS.B_POND), Camera.main.transform.position, Quaternion.identity).GetComponent<BaseBuildingsClass>();
        selectedBuilding.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Q))
        {
            brushActive = !brushActive;
            if(brushActive)
                eraserActive = false;
        }
        else if(Input.GetKeyUp(KeyCode.E))
        {
            eraserActive = !eraserActive;
            if(eraserActive)
                brushActive = false;
        }

        MoveCamera();

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

    void MoveCamera()
    {
        float moveSpeed = 10.0f;
        Vector3 cameraPos = Camera.main.transform.position;
        if (Input.GetKey("left"))
            cameraPos.x -= moveSpeed * Time.deltaTime;
        else if(Input.GetKey("right"))
            cameraPos.x += moveSpeed * Time.deltaTime;
        if (Input.GetKey("up"))
            cameraPos.y += moveSpeed * Time.deltaTime;
        else if (Input.GetKey("down"))
            cameraPos.y -= moveSpeed * Time.deltaTime;
        Camera.main.transform.position = cameraPos;
    }

}
