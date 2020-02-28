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

    public void CreateUI(BuildingData _buildData)
    {

    }
}
