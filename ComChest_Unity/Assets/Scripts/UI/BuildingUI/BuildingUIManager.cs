using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// How many unique buildings are there with UI information
public enum BuildingType
{
    COMMUNITYCENTER,
    MUSEUM,
    TOTAL_TYPE,
}

public class BuildingUIManager
{
    // Keep tracks of the current active UI Object

    IngameUIManager UIManager;

    bool isCreated = false;

    // Start is called before the first frame update
    public void Init()
    {
        UIManager = IngameUIManager.instance;
    }

    /// <summary>
    /// Not sure if im gonna need it yet
    /// </summary>
    public bool InitBuiilingUI(BuildingData building)
    {
        if (UIManager.m_BuildingUI != null)
            return false;

        if (building.GetUIObject())
        {
            IngameUIManager.instance.CreateUI(building);
           // isCreated = true;

            return true;
        }


        return false;
    }

   

    //public bool DeleteUI()
    //{
    //    // Destroy current UI Object;
    //   Destroy(currentUIObject);

    //    currentUIObject = null;

    //    IngameUIManager.instance.SetInGameMenuActive(true);

    //    return true;
    //}

    //public bool CreateUI(BuildingData _buildData)
    //{
    //    // A UI Object is already created
    //    if (currentUIObject != null)
    //        return false;
    
    //    // If it has a UI object to build
    //    if (_buildData.GetUIObject())
    //    {
    //        //currentUIObject = Instantiate(_buildData.GetUIObject(), this.transform);

    //        IngameUIManager.instance.SetInGameMenuActive(false);

    //        return true;
    //    }

    //    return false;
    //}
}
