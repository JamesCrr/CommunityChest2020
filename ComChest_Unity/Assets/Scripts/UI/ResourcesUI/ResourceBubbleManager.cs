using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public struct ResourceSprites
{
    public ResourceManager.RESOURCES m_ResourceType;
    public Sprite m_ResourceImage;
}

public class ResourceBubbleManager : SingletonBase<ResourceBubbleManager>
{
    public List<ResourceSprites> m_ResourceSpriteData;
    public GameObject m_ResourceBubblePrefab;
    private Dictionary<ResourceManager.RESOURCES, Sprite> m_ResourceSpriteStorage = new Dictionary<ResourceManager.RESOURCES, Sprite>();

    List<GameObject> m_ResourceUIList = new List<GameObject>();

    public override void Awake()
    {
        foreach (ResourceSprites resourceData in m_ResourceSpriteData)
        {
            m_ResourceSpriteStorage[resourceData.m_ResourceType] = resourceData.m_ResourceImage;
        }
    }

    public void InitResourceBubbleUI(ResourceManager.RESOURCES type, int amt, Vector2 pos, ResourceBuildings resourceBuildingObj)
    {
        ResourceBubbleUI resourceBubble = GetBubbleUI(type);
        if (resourceBubble == null)
            return;

        if (m_ResourceSpriteStorage.ContainsKey(type))
            resourceBubble.Init(type, amt, pos, m_ResourceSpriteStorage[type], resourceBuildingObj);
    }

    public ResourceBubbleUI GetBubbleUI(ResourceManager.RESOURCES type)
    {
        foreach (GameObject resourceUI in m_ResourceUIList)
        {
            if (resourceUI.activeSelf)
                continue;

            ResourceBubbleUI resourceBubbleUI = resourceUI.GetComponent<ResourceBubbleUI>();
            if (resourceBubbleUI == null)
                continue;

            return resourceBubbleUI;
        }

        if (m_ResourceBubblePrefab == null)
            return null;

        for (int i = 0; i < 5; ++i)
        {
            GameObject newResourceBubbleObj = Instantiate(m_ResourceBubblePrefab);
            newResourceBubbleObj.SetActive(false);
            m_ResourceUIList.Add(newResourceBubbleObj);
        }

        return GetBubbleUI(type);
    }
}
