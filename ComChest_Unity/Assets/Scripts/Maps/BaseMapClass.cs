using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BaseMapClass : MonoBehaviour
{
    [Header("Map Data")]
    Vector2Int m_MapDimensions = Vector2Int.zero;
    Tilemap m_TileMap = null;
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
        Debug.Log("Generated Map Size: " + m_TileMap.size);
        // Offset TileMap to make bottomLeft Corner be at (0,0) of Grid
        m_TileMap.transform.position = Vector3.zero;
    }

    public Vector2Int GetTileMapSize() { return m_MapDimensions; }
    public Tilemap GetTileMapCom() { return m_TileMap; }


#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (!m_DrawDebug)
            return;
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
    }
#endif
}
