using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableBuilding : InteractableObjBase
{
    public BaseBuildingsClass m_buildClass;
    BuildingData buildingData;
    
    GameObject UIObject = null;
    // Start is called before the first frame update
    void Start()
    {
        if(m_buildClass)
        {
            buildingData = BuildingDataBase.GetInstance().GetBuildingData(m_buildClass.GetBuildingType());
        }
    }

    // Update is called once per frame
    void Update()
    {
        // For Debugging
        if (Input.GetKeyDown(KeyCode.C))
        {
            OnInteract();
        }
    }

    public override void OnInteract()
    {
        base.OnInteract();

        Debug.Log("Tapped");
        // m_buildClass.GetBuildingID();
        if (buildingData.GetUIStatus())
            BuildingUIManager.Instance.CreateUI(buildingData);
    }
    
}
