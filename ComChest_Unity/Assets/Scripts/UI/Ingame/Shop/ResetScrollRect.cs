using UnityEngine;
using UnityEngine.UI;

public class ResetScrollRect : MonoBehaviour
{
    ScrollRect m_ScrollRect;

    public void Awake()
    {
        m_ScrollRect = GetComponent<ScrollRect>();
    }

    public void OnEnable()
    {
        if (m_ScrollRect != null)
            m_ScrollRect.normalizedPosition = Vector2.zero;
    }
}
