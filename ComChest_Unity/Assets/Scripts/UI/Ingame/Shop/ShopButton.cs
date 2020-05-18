using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ShopButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    Image m_BackgroundImage;

    public void Awake()
    {
        m_BackgroundImage = GetComponent<Image>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("Pressing down");
        //scale up the size through animations
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Debug.Log("up");
        //scale back the size through animations
    }

    public void Clicked()
    {
        Debug.Log("clicked");

        //scale up and down the size through animations
        //make the background active
        //call the main manager to handle the swapping 
        if (m_BackgroundImage != null)
        {
            Color color = m_BackgroundImage.color;
            color.a = 0.0f;
            m_BackgroundImage.color = color;
        }
    }
}
