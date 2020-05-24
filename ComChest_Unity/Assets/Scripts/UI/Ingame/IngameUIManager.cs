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
    [SerializeField] GameObject m_EditModeMenu;
    EditModeManager m_EditModeManager;
    [SerializeField] BuildingModeUIManager m_BuildingModeUIManager = new BuildingModeUIManager();

    [Header("UI Managers")]
    [SerializeField] ShopUIManager m_ShopUIManager = new ShopUIManager();

    private void Awake()
    {
        if (instance != null)
            Debug.LogWarning("More than 1 IngameUIManager instance exists!");
        instance = this;

        m_EditModeManager = m_EditModeMenu.GetComponent<EditModeManager>();
    }

    // Start is called before the first frame update
    void Start()
    {
        if (m_ShopUIManager != null)
            m_ShopUIManager.Init();

        if (m_ShopMenu != null)
            m_ShopMenu.SetActive(false);

        SetEditModeMenuActive(false);
    }

    public void InitMapUI()
    {
        if (m_BuildingModeUIManager != null)
            m_BuildingModeUIManager.Init();
    }

    public BuildingModeUIManager GetBuildingModeUIManager()
    {
        return m_BuildingModeUIManager;
    }

    #region MenuTogglers
    public void SetShopMenuActive(bool active)
    {
        if (m_ShopMenu != null) 
            m_ShopMenu.SetActive(active);

        if (active)
        {
            if (m_ShopUIManager != null)
                m_ShopUIManager.Active(); //init default category
        }
    }

    public void ChangeShopCategory(ShopItemType shopMenuCategory)
    {
        if (m_ShopUIManager != null)
            m_ShopUIManager.InitCategoryShown(shopMenuCategory);
    }

    public void SetInGameMenuActive(bool active)
    {
        if (m_InGameMenu != null)
            m_InGameMenu.SetActive(active);
    }

    public void SetEditModeMenuActive(bool active)
    {
        if (m_EditModeMenu != null)
            m_EditModeMenu.SetActive(active);

        // Disable EditMenu Brushes if disabling Edit Menu
        if (!active)
        {
            m_EditModeManager.OpenDeleteBrush(false);
            m_EditModeManager.OpenMoveBuildingsBrush(false);
        }
    }

    public void OpenShopMenu(bool open)
    {
        SetShopMenuActive(open);

        //close some of the ingame menus
        SetInGameMenuActive(!open);
    }

    public void OpenEditMode(bool open)
    {
        SetEditModeMenuActive(open);
        SetInGameMenuActive(!open);

        if (!open)
            BuildModeUIClose();
    }

    //when player is placing down deco or buildings
    public void PlayerInBuildModeUI(bool buildModeActive, BuildingDataBase.BUILDINGS buildingType = BuildingDataBase.BUILDINGS.B_LAVA)
    {
        //open the brush mode
        if (MapManager.GetInstance() != null)
            MapManager.GetInstance().SetPlacementBrush(buildModeActive, buildingType);
    }

    //when player is placing down deco or buildings
    public void BuildModeUIOpen(Transform building, Vector2 offset, Vector2Int buildingSizeOnMap)
    {
        //set up the build mode UI
        if (m_BuildingModeUIManager == null)
            return;

        m_BuildingModeUIManager.AttachToBuilding(building, offset, buildingSizeOnMap);

        //close up the original UI
        SetInGameMenuActive(false);
    }

    public void BuildModeUIClose(bool gridActive = false)
    {
        if (m_BuildingModeUIManager == null)
            return;

        m_BuildingModeUIManager.Detach(gridActive);
    }
    #endregion !MenuTogglers
}
