using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseBuildingsClass : MonoBehaviour
{
    [Header("Building Data")]
    [SerializeField]
    Vector2Int m_BuildingSize = Vector2Int.zero;
    [Header("Collision")]
    [SerializeField]
    Transform m_BottomLeftRef = null;
    [Header("Debug")]
    [SerializeField]
    bool m_DrawDebug = false;
    [SerializeField]
    Color m_DebugColor = Color.blue;

    public Vector2Int GetBuildingSize() { return m_BuildingSize; }
    public Vector3 GetBottomLeftRefPosition() { return m_BottomLeftRef.localPosition; }

    private void OnDrawGizmos()
    {
        if (!m_DrawDebug)
            return;
        if(m_BottomLeftRef == null)
        {
            Debug.LogWarning(gameObject.name + "'s BottomLeftRef not Attached!");
            return;
        }
        Vector3 targetPos = m_BottomLeftRef.position;
        Vector3 startPos = m_BottomLeftRef.position;
        Gizmos.color = m_DebugColor;
        // Bottom Across
        targetPos.x += m_BuildingSize.x;
        Gizmos.DrawLine(startPos, targetPos);
        // Right Up
        startPos = targetPos;
        targetPos.y += m_BuildingSize.y;
        Gizmos.DrawLine(startPos, targetPos);
        // Top Across
        startPos = targetPos;
        targetPos.x -= m_BuildingSize.x;
        Gizmos.DrawLine(startPos, targetPos);
        // Left Down
        startPos = targetPos;
        targetPos.y -= m_BuildingSize.y;
        Gizmos.DrawLine(startPos, targetPos);
    }
}
