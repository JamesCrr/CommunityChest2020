using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class IngameUIManager : MonoBehaviour
{
    public static IngameUIManager instance = null;

    #region IngameButtons
    [SerializeField] Button buildMenuButton;
    #endregion

    [Header("UI Pages")]
    [SerializeField] GameObject m_ShopMenu;
    [SerializeField] GameObject m_InGameMenu;

    private void Awake()
    {
        if (instance != null)
            Debug.LogWarning("More than 1 IngameUIManager instance exists!");
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        m_ShopMenu.SetActive(false);
    }

    #region MenuTogglers
    public void SetShopMenuActive(bool active)
    {
        if (m_ShopMenu != null) 
            m_ShopMenu.SetActive(active);
    }

    public void OpenShopMenu(bool open)
    {
        SetShopMenuActive(open);

        //close some of the ingame menus
        if (m_InGameMenu != null)
            m_InGameMenu.SetActive(!open);
    }
    #endregion !MenuTogglers
}
