using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// Mono Behaviour that manages build menu, including descriptions and calling actual building placeholders
/// </summary>
public class BuilderMenu : MonoBehaviour
{
    public static BuilderMenu instance = null; // Somewhat a singleton
    public BuilderMenuDescription buildingScribe;
    [SerializeField] BuildingData[] buildingDatas = new BuildingData[0];

    [SerializeField] GameObject builderElementPrefab;
    GameObject contentGameObject;
    Button currBuilding = null;
    private void Awake()
    {
        if (instance != null)
            Debug.LogWarning("More than 1 BuilderMenu instance exists!");
        instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        contentGameObject = GetComponent<ScrollRect>().content.gameObject;
        foreach (var data in buildingDatas)
        {
            var element = Instantiate(builderElementPrefab, contentGameObject.transform).GetComponent<BuilderMenuElement>();
            element.Initialize(data);
        }
    }

    public void BuildingElementClicked(Button _button)
    {
        if (currBuilding != null)
            currBuilding.interactable = true;
        currBuilding = _button;
        currBuilding.interactable = false;
    }

    public void ResetButtons()
    {
        if (currBuilding != null)
            currBuilding.interactable = true;
        currBuilding = null;
    }
}
