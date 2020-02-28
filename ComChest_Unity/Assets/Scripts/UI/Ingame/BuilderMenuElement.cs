using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
/// <summary>
/// Class that stores the building info in a button
/// When pressed, it should call the building description info
/// </summary>
public class BuilderMenuElement : MonoBehaviour
{
    [SerializeField] Image Thumbnail;
    [SerializeField] TextMeshProUGUI buildingName;
    BuildingData m_buildingData;

    public void Initialize(BuildingData _buildingData)
    {
        m_buildingData = _buildingData;
        Thumbnail.sprite = m_buildingData.GetBuildingSprite();
        buildingName.text = m_buildingData.GetBuildingName();
    }

    public void OnClick()
    {
        BuilderMenu.instance.buildingScribe.DescribeBuilding(m_buildingData);
        BuilderMenu.instance.BuildingElementClicked(GetComponent<Button>());
    }
}
