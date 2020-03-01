﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseBuildingsClass : MonoBehaviour
{
    [Header("Building GO")]
    [SerializeField]
    SpriteRenderer m_BuildingSpriteRenderer = null;
    [SerializeField]
    BoxCollider2D m_Collider = null;
    bool m_BuildingPlaced = false;
    [Header("Debug")]
    [SerializeField]
    bool m_DrawDebug = false;
    [SerializeField]
    Color m_DebugColor = Color.blue;
    [Header("From Scriptable Object")]
    [SerializeField]
    BuildingDataBase.BUILDINGS m_BuildingID = BuildingDataBase.BUILDINGS.B_TOTAL;
    [SerializeField]
    Vector2Int m_BuildingSize = Vector2Int.zero;
    Vector2 m_BottomLeftCornerOffset = Vector2.zero;
    //[Header("For UI")]


    public virtual void SetNewBuildingType(BuildingData buildingData)
    {
        // Other Data
        m_BuildingID = buildingData.GetBuildingID();
        m_BuildingSize = buildingData.GetBuildingSizeOnMap();
        // Sprite
        m_BuildingSpriteRenderer.gameObject.transform.localPosition = buildingData.GetSpriteGO_PositionOffset();
        m_BuildingSpriteRenderer.sprite = buildingData.GetBuildingSprite();
        // Collider
        m_Collider.size = buildingData.GetBuildingSpriteSize();
        m_BottomLeftCornerOffset = buildingData.GetBottomLeftCorner_PositionOffset();
    }

    // Sprite Renderer
    public GameObject GetSpriteGO() { return m_BuildingSpriteRenderer.gameObject; }
    public void SetSpriteObjectLayer(int newLayer) { m_BuildingSpriteRenderer.gameObject.layer = newLayer; }
    public void SetSpriteObjectColor(Color newColor) { m_BuildingSpriteRenderer.color = newColor; }
    // Building Details
    public Vector2Int GetBuildingSizeOnMap() { return m_BuildingSize; }
    public BuildingDataBase.BUILDINGS GetBuildingID() { return m_BuildingID; }
    public Vector2 GetBottomLeftGridPosition() { return (Vector2)transform.position + m_BottomLeftCornerOffset; }

    // Called when you place a building down and remove the building
    public virtual void BuildingPlaced()
    {
        m_BuildingPlaced = true;
    }
    public virtual void BuildingRemoved()
    {
        m_BuildingPlaced = false;
    }

    private void OnDrawGizmos()
    {
        if (!m_DrawDebug)
            return;

        // Building Size
        Vector3 targetPos = GetBottomLeftGridPosition();
        targetPos.x -= 0.5f;
        targetPos.y -= 0.5f;
        Vector3 startPos = targetPos;
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

        // Bottom Left Position
        Gizmos.DrawWireSphere(GetBottomLeftGridPosition(), 0.5f);
    }
}
