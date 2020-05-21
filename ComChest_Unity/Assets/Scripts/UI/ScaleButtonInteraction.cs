using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ScaleButtonInteraction : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    protected Animator m_Animator;

    public virtual void Awake()
    {
        m_Animator = GetComponent<Animator>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        //Debug.Log("Pressing down");
        //scale up the size through animations
        if (m_Animator != null)
            m_Animator.SetTrigger("ScaleDown");
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        //Debug.Log("up");
        //scale back the size through animations
        if (m_Animator != null)
            m_Animator.SetTrigger("ScaleBack");
    }

    public virtual void Clicked()
    {
        return;
    }
}
