using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopItemCardUI : MonoBehaviour
{
    [Header("UI Info")]
    [SerializeField] TextMeshProUGUI m_ItemNameText;
    [SerializeField] TextMeshProUGUI m_DescriptionText;
    [SerializeField] TextMeshProUGUI m_PriceText;
    [SerializeField] Image m_ItemImage;

    BuildingDataBase.BUILDINGS m_BuildingType;
    bool m_Buyable = false; //check if player has the resources needed to build this
    Animator m_Animator;

    public void Awake()
    {
        m_Animator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        //check whether player is able to purchase this item
        //update the UI accordingly
        m_Buyable = ResourceManager.GetInstance().EnoughResourcesForBuilding(m_BuildingType);

        if (m_Buyable)
            m_PriceText.color = Color.white;
        else
            m_PriceText.color = Color.red;
    }

    public void Init(BuildingData m_BuildingData)
    {
        if (m_BuildingData == null)
            return;

        //pass the building data type
        m_ItemNameText.SetText(m_BuildingData.GetBuildingName());
        m_DescriptionText.SetText(m_BuildingData.GetBuildingDescription());
        m_PriceText.SetText(m_BuildingData.GetPrice().ToString());

        //TODO:: temp
        m_ItemImage.sprite = m_BuildingData.GetBuildingSprite();

        m_BuildingType = m_BuildingData.GetBuildingType();
    }

    public void OnClicked()
    {
        if (IngameUIManager.instance == null)
            return;

        //check whether player has money, if dont, show a UI and animate
        if (m_Buyable)
        {
            //close shop and activate UI for placement
            IngameUIManager.instance.SetShopMenuActive(false);
            IngameUIManager.instance.PlayerInBuildModeUI(true, m_BuildingType);
        }
        else
        {
            //activate animation
            if (m_Animator != null)
                m_Animator.SetTrigger("Shake");
        }
    }
}
