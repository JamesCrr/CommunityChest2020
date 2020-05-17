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
    [SerializeField] GameObject m_ShopMenu;//the shop menu
    [SerializeField] GameObject m_InGameMenu; //things on the UI like build button etc.
    [SerializeField] GameObject m_PlayerInBuildModeUI;

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
        m_PlayerInBuildModeUI.SetActive(false);
    }

    #region MenuTogglers
    public void SetShopMenuActive(bool active)
    {
        if (m_ShopMenu != null) 
            m_ShopMenu.SetActive(active);
    }

    public void SetInGameMenuActive(bool active)
    {
        if (m_InGameMenu != null)
            m_InGameMenu.SetActive(active);
    }

    public void OpenShopMenu(bool open)
    {
        SetShopMenuActive(open);

        //close some of the ingame menus
        SetInGameMenuActive(!open);
    }

    //when player is placing down deco or buildings
    public void PlayerInBuildModeUI(bool buildModeActive, BuildingDataBase.BUILDINGS buildingType = BuildingDataBase.BUILDINGS.B_LAVA)
    {
        //set up the build mode UI
        if (m_PlayerInBuildModeUI != null)
            m_PlayerInBuildModeUI.SetActive(buildModeActive);

        //set up the original UI
        SetInGameMenuActive(!buildModeActive);

        //open the brush mode
        if (MapManager.GetInstance() != null)
            MapManager.GetInstance().SetPlacementBrush(buildModeActive, buildingType);
    }
    #endregion !MenuTogglers
}
