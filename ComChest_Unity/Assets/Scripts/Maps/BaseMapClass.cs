using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BaseMapClass : MonoBehaviour
{
    [Header("Critical Map Data")]
    Vector2Int m_MapDimensions = Vector2Int.zero;
    Tilemap m_TileMap = null;
    [Header("Map Details")]
    [SerializeField]
    List<MapStartingBuildings> m_ListOfBuildingsToBuildAtSpawn = new List<MapStartingBuildings>();
    [Header("Debug")]
    [SerializeField]
    bool m_DrawDebug = false;
    [SerializeField]
    Color m_DebugColor = Color.blue;

    private void Awake()
    {
        // Get TileMap
        m_TileMap = transform.GetChild(0).GetComponent<Tilemap>();
        m_TileMap.CompressBounds();
        m_MapDimensions = (Vector2Int)m_TileMap.size;
        Debug.Log("Generated Map Object Size: " + m_TileMap.size);
        // Offset TileMap to make bottomLeft Corner be at (0,0) of Grid
        m_TileMap.transform.position = Vector3.zero;
    }
    private void Start()
    {
        // Do anything related to this map 
        // When this map is generated
        MapWasGenerated();
    }

    protected virtual void MapWasGenerated()
    {
        foreach (MapStartingBuildings building in m_ListOfBuildingsToBuildAtSpawn)
        {
            MapManager.GetInstance().PlaceNewBuildingIntoMap(building.spawnGridPosition, building.buildingID);
        }
    }

    #region Getters
    public Vector2Int GetTileMapSize() { return m_MapDimensions; }
    public Vector2Int GetTileMapHalfSize() { return m_MapDimensions / 2; }
    public Tilemap GetTileMapCom() { return m_TileMap; }
    #endregion

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if (!m_DrawDebug)
            return;
        // Map Dimensions
        Vector3 targetPos = transform.position;
        Vector3 startPos = transform.position;
        Gizmos.color = m_DebugColor;
        // Bottom Across
        targetPos.x += m_MapDimensions.x;
        Gizmos.DrawLine(startPos, targetPos);
        // Right Up
        startPos = targetPos;
        targetPos.y += m_MapDimensions.y;
        Gizmos.DrawLine(startPos, targetPos);
        // Top Across
        startPos = targetPos;
        targetPos.x -= m_MapDimensions.x;
        Gizmos.DrawLine(startPos, targetPos);
        // Left Down
        startPos = targetPos;
        targetPos.y -= m_MapDimensions.y;
        Gizmos.DrawLine(startPos, targetPos);

        // Starting Building Positions
        foreach (MapStartingBuildings building in m_ListOfBuildingsToBuildAtSpawn)
        {
            Vector2 half = building.spawnGridPosition;
            half.x += 0.5f;
            half.y += 0.5f;
            Gizmos.color = building.debugColor;
            Gizmos.DrawWireSphere((Vector2)transform.position + half, 0.5f);
        }

    }
#endif
}

[System.Serializable]
public class MapStartingBuildings
{
    public BuildingDataBase.BUILDINGS buildingID = BuildingDataBase.BUILDINGS.B_TOTAL;
    public Vector2Int spawnGridPosition = Vector2Int.zero;
    public Color debugColor = Color.red;
}