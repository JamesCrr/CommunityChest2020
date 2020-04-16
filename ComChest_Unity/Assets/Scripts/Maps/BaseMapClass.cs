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
    Vector2Int m_MainBuildingStartPosition = Vector2Int.down;
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

    public Vector2Int GetTileMapSize() { return m_MapDimensions; }
    public Vector2Int GetTileMapHalfSize() { return m_MapDimensions / 2; }
    public Tilemap GetTileMapCom() { return m_TileMap; }
    public Vector2Int GetStartingBuildingGridPosition() { return m_MainBuildingStartPosition; }


#if UNITY_EDITOR
    private void OnDrawGizmos()
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

        // Starting Main Building Position
        if(m_MainBuildingStartPosition != Vector2Int.down)
            Gizmos.DrawWireSphere((Vector2)m_MainBuildingStartPosition, 0.5f);
        else if(m_MapDimensions != Vector2Int.zero)
        {
            Vector2 half = GetTileMapHalfSize();
            half.x += 0.5f;
            half.y += 0.5f;
            Gizmos.DrawWireSphere(half, 0.5f);
        }
    }
#endif
}
