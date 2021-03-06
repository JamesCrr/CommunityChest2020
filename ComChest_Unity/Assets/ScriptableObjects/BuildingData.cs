﻿using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Building")]
public class BuildingData : ScriptableObject
{
    [Header("Building Data")]
    [SerializeField]
    string m_BuildingName = "Unnamed_Building";
    [SerializeField]
    Vector2Int m_BuildingSpriteSize = Vector2Int.zero;
    [SerializeField]
    Vector2Int m_BuildingSizeOnMap = Vector2Int.zero;
    [SerializeField]
    Sprite m_BuildingSprite = null;
    [SerializeField]
    BuildingDataBase.BUILDINGS m_BuildingType = BuildingDataBase.BUILDINGS.B_TOTAL;
    [SerializeField]
    GameObject m_CustomBuildingObject = null;
    [Header("Collision")]
    Vector2 m_SpriteGO_OffsetPosition = Vector2.zero;
    Vector2 m_BottomLeftCorner_OffsetPosition = Vector2.zero;

    [Header("UI Info")]
    [SerializeField]
    bool m_HasUI = false;
    [SerializeField]
    GameObject m_UIObject = null;

    [Header("Shop UI Info")]
    [SerializeField]
    string m_BuildingDescription;
    [SerializeField]
    int m_BuildingPrice = 100;
    [SerializeField]
    bool m_IsSoldInShop = true;
    [SerializeField]
    ShopItemType m_ShopItemType = ShopItemType.BUILDINGS;

    // Resources
    [Header("Resources Needed Data")]
    [SerializeField]
    List<ResourcesNeeded> m_ListOfResourcesNeeded = new List<ResourcesNeeded>();
    [Header("Resources Produced Data")]
    [SerializeField]
    float m_GenerateResourceTime = 0.0f;
    [SerializeField]
    ResourceManager.RESOURCES m_ResourceProduced = ResourceManager.RESOURCES.R_NONE;
    [SerializeField]
    int m_AmtResourceGiven = 0;
    Vector2 m_ResourceUIOffset;

    [Header("Road Info")]
    [Tooltip("For buildings that need a road in front of it, the road offset starting from the building bottom left grid")]
    [SerializeField]
    Vector2Int m_RoadOffset = Vector2Int.zero;

    private void OnEnable()
    {
        m_SpriteGO_OffsetPosition = Vector2.zero;
        m_BottomLeftCorner_OffsetPosition = Vector2.zero;

        // Calculate the Offset Positions for Collision
        Vector2Int halfBuildingSize = m_BuildingSpriteSize / 2;
        // Find Bottom left Corner
        m_BottomLeftCorner_OffsetPosition.x = -halfBuildingSize.x;
        m_BottomLeftCorner_OffsetPosition.y = -halfBuildingSize.y;

        // is building Width Even sized
        if (m_BuildingSpriteSize.x % 2 == 0)
        {
            m_SpriteGO_OffsetPosition.x = 0.5f;
            m_BottomLeftCorner_OffsetPosition.x += 1.0f;
        }
        // is building Height Even sized
        if (m_BuildingSpriteSize.y % 2 == 0)
        {
            m_SpriteGO_OffsetPosition.y = 0.5f;
            m_BottomLeftCorner_OffsetPosition.y += 1.0f;
        }
        // Valid Size on Map?
        if (m_BuildingSizeOnMap.x < 0)
            m_BuildingSizeOnMap.x = 0;
        if (m_BuildingSizeOnMap.y < 0)
            m_BuildingSizeOnMap.y = 0;
        if (m_BuildingSizeOnMap == Vector2Int.zero)
            m_BuildingSizeOnMap = m_BuildingSpriteSize;

        //for resources
        m_ResourceUIOffset = new Vector2(0.0f, halfBuildingSize.y);
    }

    #region Building Getters
    public string GetBuildingName() { return m_BuildingName; }
    public string GetBuildingDescription() { return m_BuildingDescription; }
    public BuildingDataBase.BUILDINGS GetBuildingType() { return m_BuildingType; }
    public Sprite GetBuildingSprite() { return m_BuildingSprite; }
    public Vector2Int GetBuildingSpriteSize() { return m_BuildingSpriteSize; }
    public Vector2Int GetBuildingSizeOnMap() { return m_BuildingSizeOnMap; }
    public Vector2 GetSpriteGO_PositionOffset() { return m_SpriteGO_OffsetPosition; }
    public Vector2 GetBottomLeftCorner_PositionOffset() { return m_BottomLeftCorner_OffsetPosition; }
    public GameObject GetOwnCustomBuildingObject() { return m_CustomBuildingObject; }
    #endregion

    #region Resource Needed Getters
    public List<ResourcesNeeded> GetResourcesNeeded() { return m_ListOfResourcesNeeded; }
    #endregion

    #region Resource Produced Getters
    public float GetResourceTime
    {
        get { return m_GenerateResourceTime; }
    }

    public int GetAmtResource
    {
        get { return m_AmtResourceGiven; }
    }

    public ResourceManager.RESOURCES GetResourceTypeProduced
    {
        get { return m_ResourceProduced; }
    }
    #endregion

    #region UI
    public Vector2 GetResourceUIOffset
    {
        get { return m_ResourceUIOffset; }
    }

    public bool GetUIStatus()
    {
        return m_HasUI;
    }

    public bool GetIsSoldInShop() { return m_IsSoldInShop; }
    public ShopItemType GetShopItemType() { return m_ShopItemType; }
    public int GetPrice() { return m_BuildingPrice; }
    public GameObject GetUIObject() { return m_UIObject; }
    #endregion

    #region roadGetters
    public Vector2Int GetRoadOffset
    {
        get { return m_RoadOffset; }
    }
    #endregion

}

[System.Serializable]
public class ResourcesNeeded
{
    public ResourceManager.RESOURCES resourceID = ResourceManager.RESOURCES.R_MONEY;
    public int amount = 1;
    public bool toDeduct = true;    // To Deduct from the resources Or simply have enough
}
