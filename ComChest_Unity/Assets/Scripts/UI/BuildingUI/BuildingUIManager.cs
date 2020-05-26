using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingUIManager : SingletonBase<BuildingUIManager>
{
    // Keep tracks of the current active UI Object
    GameObject currentUIObject = null;
    bool isCreated = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// Not sure if im gonna need it yet
    /// </summary>
    void SetupUIObject()
    {

    }

    public bool DeleteUI()
    {
        // Destroy current UI Object;
       Destroy(currentUIObject);

        currentUIObject = null;

        IngameUIManager.instance.SetInGameMenuActive(true);

        return true;
    }

    public bool CreateUI(BuildingData _buildData)
    {
        // A UI Object is already created
        if (currentUIObject != null)
            return false;
    
        // If it has a UI object to build
        if (_buildData.GetUIObject())
        {
            currentUIObject = Instantiate(_buildData.GetUIObject(), this.transform);

            IngameUIManager.instance.SetInGameMenuActive(false);

            return true;
        }

        return false;
    }
}
