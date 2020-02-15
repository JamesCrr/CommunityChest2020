using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    [Header("Grid and Placements")]
    [SerializeField]        // Where the Tilemaps will be rendered
    Grid m_GridGO = null;
    BaseMapClass m_currentMap;
    List<bool> m_GridTakenArray;

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

        return true;
    }

    public GameObject PlaceBuilding(BuildingDataBase.BUILDINGS buildingType, Vector3 buildingWorldPos)
    {
        BaseBuildingsClass buildingCom = BuildingDataBase.GetInstance().GetBuildingCom(buildingType);
        Vector3 buildingBottomLeftWorldPos = buildingWorldPos + buildingCom.GetBottomLeftRefPosition();
        Vector2Int buildingGridPos = (Vector2Int)m_GridGO.WorldToCell(buildingBottomLeftWorldPos);
        Vector2Int buildingSize = buildingCom.GetBuildingSize();
        Debug.Log("GRID POS: " + m_GridGO.WorldToCell(buildingWorldPos));
        // Can we place it there?
        if (!CanPlaceBuilding(buildingBottomLeftWorldPos, buildingSize))
            return null;
        // Set all the grids taken by new building to true
        Vector2Int testGridPos = buildingGridPos;
        int arrayIndex = Convert2DToIntIndex(buildingGridPos);
        for (int yAxis = 0; yAxis < buildingSize.y; ++yAxis)
        {
            testGridPos.y += yAxis;
            for (int xAxis = 0; xAxis < buildingSize.x; ++xAxis)
            {
                testGridPos.x = buildingGridPos.x + xAxis;
                arrayIndex = Convert2DToIntIndex(testGridPos);
                m_GridTakenArray[arrayIndex] = true;
            }
            // Reset checking Position
            testGridPos = buildingGridPos;
        }

        return Instantiate(buildingCom.gameObject, buildingWorldPos, Quaternion.identity);
    }

    /// <summary>
    /// Checks if any of the spots the building will occupy, is already taken
    /// </summary>
    /// <param name="buildingBottomLeftWorldPos">The world Position of the Building GO, Origin should be at Bottom Left</param>
    /// <param name="gridDimensions">How many grids does this building take</param>
    /// <returns></returns>
    bool CanPlaceBuilding(Vector3 buildingBottomLeftWorldPos, Vector2Int gridDimensions)
    {
        Vector2Int buildingGridPos = (Vector2Int)m_GridGO.WorldToCell(buildingBottomLeftWorldPos);
        Vector2Int testGridPos = buildingGridPos;
        int arrayIndex;
      
        for (int yAxis = 0; yAxis < gridDimensions.y; ++yAxis)
        {
            testGridPos.y += yAxis;
            for (int xAxis = 0; xAxis < gridDimensions.x; ++xAxis)
            {
                testGridPos.x = buildingGridPos.x + xAxis;
                // Out of Scope?
                if (testGridPos.x < 0 || testGridPos.x >= m_currentMap.GetMapSize().x ||
                    testGridPos.y < 0 || testGridPos.y >= m_currentMap.GetMapSize().y)
                    return false;
                // Check if spot is already taken
                arrayIndex = Convert2DToIntIndex(testGridPos);
                if (m_GridTakenArray[arrayIndex])
                {
                    Debug.Log("SPOT TAKEN");
                    return false;
                }
            }
            // Reset checking Position
            testGridPos = buildingGridPos;
        }
        return true;
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

}
