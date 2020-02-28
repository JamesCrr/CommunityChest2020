using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class IngameUIManager : MonoBehaviour
{
    #region IngameButtons
    [SerializeField] Button buildMenuButton;
    #endregion
    [SerializeField] GameObject buildMenu;
    // Start is called before the first frame update
    void Start()
    {
        buildMenu.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

    }

    #region MenuTogglers
    public void BuildMenuPressed()
    {
        buildMenuButton.enabled = false;
        buildMenu.SetActive(true);
    }
    public void CloseBuildMenuPressed()
    {
        buildMenuButton.enabled = true;
        buildMenu.SetActive(false);
    }
    #endregion !MenuTogglers
}
