using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Building")]
public class BuildingData : ScriptableObject
{
    [Header("Building Data")]
    [SerializeField]
    Vector2Int m_BuildingSize = Vector2Int.zero;
    [SerializeField]
    Sprite m_BuildingSprite = null;
    [SerializeField]
    BuildingDataBase.BUILDINGS m_BuildingID = BuildingDataBase.BUILDINGS.B_TOTAL;
    [Header("Collision")]
    Vector2 m_SpriteGO_OffsetPosition = Vector2.zero;
    Vector2 m_BottomLeftCorner_OffsetPosition = Vector2.zero;

    private void OnEnable()
    {
        m_SpriteGO_OffsetPosition = Vector2.zero;
        m_BottomLeftCorner_OffsetPosition = Vector2.zero;

        // Calculate the Offset Positions for Collision
        Vector2Int halfBuildingSize = m_BuildingSize / 2;
        // Find Bottom left Corner
        m_BottomLeftCorner_OffsetPosition.x = -halfBuildingSize.x;
        m_BottomLeftCorner_OffsetPosition.y = -halfBuildingSize.y;

        // is building Width Even sized
        if (m_BuildingSize.x % 2 == 0)
        {
            m_SpriteGO_OffsetPosition.x = 0.5f;
            m_BottomLeftCorner_OffsetPosition.x += 1.0f;
        }
        // is building Height Even sized
        if (m_BuildingSize.y % 2 == 0)
        {
            m_SpriteGO_OffsetPosition.y = 0.5f;
            m_BottomLeftCorner_OffsetPosition.y += 1.0f;
        }

    }

    public BuildingDataBase.BUILDINGS GetBuildingID() { return m_BuildingID; }
    public Sprite GetBuildingSprite() { return m_BuildingSprite; }
    public Vector2Int GetBuildingSize() { return m_BuildingSize; }
    public Vector2 GetSpriteGO_PositionOffset() { return m_SpriteGO_OffsetPosition; }
    public Vector2 GetBottomLeftCorner_PositionOffset() { return m_BottomLeftCorner_OffsetPosition; }
}
