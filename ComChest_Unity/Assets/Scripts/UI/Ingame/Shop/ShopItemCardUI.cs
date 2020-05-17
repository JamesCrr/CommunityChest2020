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
    bool m_Unlocked = false;

    // Start is called before the first frame update
    void Start()
    {
        //check whether player unlocked this or not
    }

    private void OnEnable()
    {
        //check whether player is able to purchase this item
        //update the UI accordingly
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

        //close shop and activate UI for placement
        IngameUIManager.instance.SetShopMenuActive(false);
        IngameUIManager.instance.PlayerInBuildModeUI(true, m_BuildingType);
    }
}
