using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseBuildingsClass : MonoBehaviour
{
    [Header("Building GO")]
    [SerializeField]
    protected SpriteRenderer m_BuildingSpriteRenderer = null;
    [SerializeField]
    protected BoxCollider2D m_Collider = null;
    protected bool m_BuildingPlaced = false;    // Is building placed on Map
    protected bool m_CanBeRemoved = true;       // Can building be Removed from Map
    protected bool m_CanBeMoved = true;         // Can building be Moved around the Map
    [Header("Debug")]
    [SerializeField]
    protected bool m_DrawDebug = false;
    [SerializeField]
    protected Color m_DebugColor = Color.blue;
    [Header("From Scriptable Object")]
    [SerializeField]
    protected BuildingDataBase.BUILDINGS m_BuildingType = BuildingDataBase.BUILDINGS.B_TOTAL;
    [SerializeField]
    protected Vector2Int m_BuildingSize = Vector2Int.zero;
    protected Vector2 m_BottomLeftCornerOffset = Vector2.zero;
    protected Vector2Int m_RoadOffset = Vector2Int.zero;


    public virtual void SetNewBuildingType(BuildingData buildingData)
    {
        // Other Data
        m_BuildingType = buildingData.GetBuildingType();
        m_BuildingSize = buildingData.GetBuildingSizeOnMap();
        // Sprite
        m_BuildingSpriteRenderer.gameObject.transform.localPosition = buildingData.GetSpriteGO_PositionOffset();
        m_BuildingSpriteRenderer.sprite = buildingData.GetBuildingSprite();
        // Collider
        m_Collider.size = buildingData.GetBuildingSpriteSize();
        m_BottomLeftCornerOffset = buildingData.GetBottomLeftCorner_PositionOffset();
        m_RoadOffset = buildingData.GetRoadOffset;
    }
    public virtual void SetBuildingPlayerOptions(bool canBeMoved = true, bool canBeRemoved = true)
    {
        m_CanBeMoved = canBeMoved;
        m_CanBeRemoved = canBeRemoved;
    }

    // Sprite Renderer
    public GameObject GetSpriteGO() { return m_BuildingSpriteRenderer.gameObject; }
    public SpriteRenderer GetSpriteRenderer() { return m_BuildingSpriteRenderer; }
    public void SetSprite(Sprite sprite) { m_BuildingSpriteRenderer.sprite = sprite; }
    public void SetSpriteObjectLayer(int newLayer) { m_BuildingSpriteRenderer.gameObject.layer = newLayer; }
    public void SetSpriteObjectColor(Color newColor) { m_BuildingSpriteRenderer.color = newColor; }
    // Building Details
    public Vector2Int GetBuildingSizeOnMap() { return m_BuildingSize; }
    public BuildingDataBase.BUILDINGS GetBuildingType() { return m_BuildingType; }
    public Vector2 GetBottomLeftGridPosition() { return (Vector2)transform.position + m_BottomLeftCornerOffset; }
    public bool GetIsBuildingPlacedOnMap() { return m_BuildingPlaced; }
    public bool GetCanBeRemoved() { return m_CanBeRemoved; }
    public bool GetCanBeMoved() { return m_CanBeMoved; }

    // Called when you place a building down and remove the building
    public virtual void BuildingPlaced()
    {
        m_BuildingPlaced = true;
    }
    public virtual void BuildingRemoved()
    {
        m_BuildingPlaced = false;
    }

    public virtual void BuildingMoved()
    {
        return;
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
