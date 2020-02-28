using UnityEngine;

[CreateAssetMenu(menuName = "Building")]
public class BuildingData : ScriptableObject
{
    [Header("Building Data")]
    [SerializeField]
    string m_BuildingName = "Unnamed_Building";
    [SerializeField] string m_BuildingDescription;
    [SerializeField]
    Vector2Int m_BuildingSize = Vector2Int.zero;
    [SerializeField]
    Sprite m_BuildingSprite = null;
    [SerializeField]
    BuildingDataBase.BUILDINGS m_BuildingID = BuildingDataBase.BUILDINGS.B_TOTAL;
    [SerializeField]
    GameObject m_CustomBuildingObject = null;
    [Header("Collision")]
    Vector2 m_SpriteGO_OffsetPosition = Vector2.zero;
    Vector2 m_BottomLeftCorner_OffsetPosition = Vector2.zero;

    [Header("UI Shiz")]
    [SerializeField]
    bool m_HasUI = false;
    [SerializeField]
    GameObject m_UIObject = null;

    //for resources
    [Header("Resources Produced Data")]
    [SerializeField]
    float m_GenerateResourceTime = 0.0f;
    [SerializeField]
    BuildingDataBase.RESOURCES m_ResourceProduced = BuildingDataBase.RESOURCES.R_NONE;
    [SerializeField]
    int m_AmtResourceGiven = 0;
    Vector2 m_ResourceUIOffset;

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

        //for resources
        m_ResourceUIOffset = new Vector2(0.0f, halfBuildingSize.y);
    }

    public string GetBuildingName() { return m_BuildingName; }
    public string GetBuildingDescription() { return m_BuildingDescription; }
    public BuildingDataBase.BUILDINGS GetBuildingID() { return m_BuildingID; }
    public Sprite GetBuildingSprite() { return m_BuildingSprite; }
    public Vector2Int GetBuildingSize() { return m_BuildingSize; }
    public Vector2 GetSpriteGO_PositionOffset() { return m_SpriteGO_OffsetPosition; }
    public Vector2 GetBottomLeftCorner_PositionOffset() { return m_BottomLeftCorner_OffsetPosition; }
    public GameObject GetOwnCustomBuildingObject() { return m_CustomBuildingObject; }

    public float GetResourceTime
    {
        get { return m_GenerateResourceTime; }
    }

    public int GetAmtResource
    {
        get { return m_AmtResourceGiven; }
    }

    public BuildingDataBase.RESOURCES GetResourceTypeProduced
    {
        get { return m_ResourceProduced; }
    }

    public Vector2 GetResourceUIOffset
    {
        get { return m_ResourceUIOffset; }
    }

    public bool GetUIStatus()
    {
        return m_HasUI;
    }

    public GameObject GetUIObject()
    {
        return m_UIObject;
    }
   

}
