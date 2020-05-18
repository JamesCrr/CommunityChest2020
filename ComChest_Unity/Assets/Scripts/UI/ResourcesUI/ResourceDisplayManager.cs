using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ResourceDisplayManager : MonoBehaviour
{
    [Header("Money UI")]
    [SerializeField]
    GameObject m_MoneyDisplayUI = null;
    TextMeshProUGUI m_MoneyDisplayText = null;
    [Header("Happiness UI")]
    [SerializeField]
    GameObject m_HappyDisplayUI = null;
    TextMeshProUGUI m_HappyDisplayText = null;

    private void Awake()
    {
        // Get Components
        m_MoneyDisplayText = m_MoneyDisplayUI.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        m_HappyDisplayText = m_HappyDisplayUI.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        // Subscribe to onResource Change events
        ResourceManager.OnResourceChanged += ResourceWasChanged;
    }

    /// <summary>
    /// Updates UI with values from ResourceManager
    /// </summary>
    void FetchResourcesValueToUI()
    {
        m_MoneyDisplayText.text = ResourceManager.GetInstance().GetResource(ResourceManager.RESOURCES.R_MONEY).ToString();
        m_HappyDisplayText.text = ResourceManager.GetInstance().GetResource(ResourceManager.RESOURCES.R_HAPPINESS).ToString() + "%";
    }

    /// <summary>
    /// When the ResourceManager calls the OnResourceChanged Event
    /// </summary>
    void ResourceWasChanged()
    {
        FetchResourcesValueToUI();
    }
}
