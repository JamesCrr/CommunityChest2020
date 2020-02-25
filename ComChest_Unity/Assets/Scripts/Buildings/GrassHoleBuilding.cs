using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrassHoleBuilding : BaseBuildingsClass
{

    private void Start()
    {
        Debug.Log("Custom Grass Building was Created!");
    }

    // How to do this?
    // 1.   Create the Script that your Custom building will run on.
    // 1.1  Make sure the script inherits from BaseBuildingClass.
    // 2.   Attach that script to your Building GO and turn it into a prefab.
    // 3.   In the Scriptable Object, drag the prefab into the CustomBuilding Object space.

    // Do anything here that other buildings are not supposed
    // to do...


}
