using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseMapClass : MonoBehaviour
{
    [Header("Map Data")]
    [SerializeField]
    Vector2Int m_MapDimensions = Vector2Int.zero;
    [Header("Debug")]
    [SerializeField]
    bool m_DrawDebug = false;
    [SerializeField]
    Color m_DebugColor = Color.blue;

    public Vector2Int GetMapSize() { return m_MapDimensions; }


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
}
