using UnityEngine;
using System.Collections.Generic;
using System;

[Serializable]
public struct ResourceSprites
{
    public BuildingDataBase.RESOURCES m_ResourceType;
    public Sprite m_ResourceImage;
}

public class ResourceBubbleUI : InteractableObjBase
{
    private BuildingDataBase.RESOURCES m_ResourceType;
    private int m_AmtGiven;

    public SpriteRenderer m_ResourceSprite;
    public List<ResourceSprites> m_ResourceSpriteData;
    private Dictionary<BuildingDataBase.RESOURCES, Sprite> m_ResourceSpriteStorage = new Dictionary<BuildingDataBase.RESOURCES, Sprite>();

    public void Awake()
    {
        foreach (ResourceSprites resourceData in m_ResourceSpriteData)
        {
            m_ResourceSpriteStorage[resourceData.m_ResourceType] = resourceData.m_ResourceImage;
        }
    }

    public void OnEnable()
    {
        if (m_ResourceSpriteStorage.ContainsKey(m_ResourceType))
            m_ResourceSprite.sprite = m_ResourceSpriteStorage[m_ResourceType];
    }

    public override void OnInteract()
    {
        if (m_ResourceType == BuildingDataBase.RESOURCES.R_MONEY)
            PlayerDataManager.Instance.GetSetTotalMoney = PlayerDataManager.Instance.GetSetTotalMoney + m_AmtGiven;

        gameObject.SetActive(false);
    }
}
