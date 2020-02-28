using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
/// <summary>
/// Class which describes buildings that are selected
/// </summary>
public class BuilderMenuDescription : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI buildingName;
    [SerializeField] TextMeshProUGUI buildingCost;
    [SerializeField] TextMeshProUGUI buildingDescription;
    [SerializeField] Image buildingThumbnail;
    public void DescribeBuilding(BuildingData _data)
    {
        buildingName.text = _data.name;
        buildingDescription.text = _data.GetBuildingDescription();
        buildingThumbnail.sprite = _data.GetBuildingSprite();
    }
}
