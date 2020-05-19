using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ShopButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] ShopItemType m_ShopCategory = ShopItemType.BUILDINGS;

    Image m_BackgroundImage;
    Animator m_Animator;

    public void Awake()
    {
        m_BackgroundImage = GetComponent<Image>();
        m_Animator = GetComponent<Animator>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        //Debug.Log("Pressing down");
        //scale up the size through animations
        m_Animator.SetTrigger("ScaleDown");
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        //Debug.Log("up");
        //scale back the size through animations
        m_Animator.SetTrigger("ScaleBack");
    }

    public void Clicked()
    {
        //Debug.Log("clicked");

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
