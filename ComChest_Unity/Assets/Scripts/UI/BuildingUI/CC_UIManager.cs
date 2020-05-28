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
        IngameUIManager.instance.CloseBuildingUI();
    }

    public void OpenFacebook()
    {
        Application.OpenURL("https://www.facebook.com/comchest/");
    }

    public void OpenInstagram()
    {
        Application.OpenURL("https://www.instagram.com/comchestsg/");
    }

    public void OpenTwitter()
    {
        Application.OpenURL("https://twitter.com/comchestsg");
    }
}
