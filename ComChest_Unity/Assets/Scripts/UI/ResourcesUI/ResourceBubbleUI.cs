using UnityEngine;

public class ResourceBubbleUI : InteractableObjBase
{
    public SpriteRenderer m_ResourceSprite;

    private ResourceManager.RESOURCES m_ResourceType;
    private int m_AmtGiven;

    private ResourceBuildings m_ResourcedBuildingOwner; //who it belongs to

    public void Init(ResourceManager.RESOURCES type, int amt, Vector2 pos, Sprite resourceSprite, ResourceBuildings resourceBuildingOwner)
    {
        m_ResourceType = type;
        m_AmtGiven = amt;

        gameObject.transform.position = pos;
        gameObject.SetActive(true);

        m_ResourceSprite.sprite = resourceSprite;

        m_ResourcedBuildingOwner = resourceBuildingOwner;
        if (m_ResourcedBuildingOwner != null)
            m_ResourcedBuildingOwner.GetSetResourceCollected = false;
    }

    public override void OnInteract()
    {
        if (m_ResourceType == ResourceManager.RESOURCES.R_MONEY)
            PlayerDataManager.Instance.GetSetTotalMoney = PlayerDataManager.Instance.GetSetTotalMoney + m_AmtGiven;

        if (m_ResourcedBuildingOwner != null)
            m_ResourcedBuildingOwner.GetSetResourceCollected = true;

        gameObject.SetActive(false);
    }
}
