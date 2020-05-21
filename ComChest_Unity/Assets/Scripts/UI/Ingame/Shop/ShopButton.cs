using UnityEngine;
using UnityEngine.UI;

public class ShopButton : ScaleButtonInteraction
{
    [SerializeField] ShopItemType m_ShopCategory = ShopItemType.BUILDINGS;

    Image m_BackgroundImage;

    public override void Awake()
    {
        base.Awake();
        m_BackgroundImage = GetComponent<Image>();
    }

    public override void Clicked()
    {
        //call the main manager to handle the swapping 
        IngameUIManager.instance.ChangeShopCategory(m_ShopCategory);
    }

    public void PutBackground(bool put)
    {
        //make the background active
        if (m_BackgroundImage != null)
        {
            Color color = m_BackgroundImage.color;

            if (put)
                color.a = 1.0f;
            else
                color.a = 0.0f;

            m_BackgroundImage.color = color;
        }
    }
}
