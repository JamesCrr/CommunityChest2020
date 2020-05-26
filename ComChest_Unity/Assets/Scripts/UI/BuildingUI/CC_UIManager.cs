using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CC_UIManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CloseUI()
    {
        BuildingUIManager.Instance.DeleteUI();
    }
}
