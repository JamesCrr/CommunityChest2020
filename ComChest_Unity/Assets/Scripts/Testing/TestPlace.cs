using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Tilemaps;
using UnityEngine.Tilemaps;

public class TestPlace : MonoBehaviour
{
    public GridLayout gridLayout = null;
    public Tilemap targetTileMap = null;
    public GameObject targetToPaint = null;
    [SerializeField]
    PrefabBrush pondBrush = null;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonUp(0))
        {
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            worldPos.z = 0.0f;
            Vector3Int gridPos = gridLayout.WorldToCell(worldPos);

            MapManager.GetInstance().PlaceBuilding(BuildingDataBase.BUILDINGS.B_POND, gridPos + (gridLayout.cellSize * 0.5f));

            //pondBrush.PlaceGO(targetTileMap, targetTileMap.gameObject, gridPos, 0);
            //GameObject newObject = pondBrush.GetGOInCell(targetTileMap, targetTileMap.transform, gridPos);
            //Debug.Log(newObject.name);
        }

    }
}
