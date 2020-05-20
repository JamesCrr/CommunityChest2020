using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    // All possible resources
    public enum RESOURCES        
    {
        R_MONEY,
        R_HAPPINESS,

        R_NONE
    }
    // To hold all of the resources
    Dictionary<RESOURCES, int> m_DictOfResources = new Dictionary<RESOURCES, int>();
    // Resources Changed Action
    public delegate void ResourceChangedAction();
    public static event ResourceChangedAction OnResourceChanged;

    static ResourceManager m_Instance = null;
    public static ResourceManager GetInstance() { return m_Instance; }
    private void Awake()
    {
        if (m_Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        m_Instance = this;
        DontDestroyOnLoad(gameObject);
        // Allocate Dict space
        for(int i = 0; i < (int)RESOURCES.R_NONE; ++i)
        {
            m_DictOfResources.Add((RESOURCES)i, 0);
        } 
    }

    public void SetResource(RESOURCES _id, int _value)
    {
        m_DictOfResources[_id] = _value;
        // Fire resource changed event
        OnResourceChanged();
    }
    public void ModifyResource(RESOURCES _id, int valueToAdd)
    {
        m_DictOfResources[_id] += valueToAdd;
        // Fire resource changed event
        OnResourceChanged();
    }
    public int GetResource(RESOURCES _id) { return m_DictOfResources[_id]; }

    /// <summary>
    /// Returns if you have enough resources to place the building
    /// </summary>
    /// <param name="buildingID">What type of building to check against</param>
    /// <returns></returns>
    public bool EnoughResourcesForBuilding(BuildingDataBase.BUILDINGS buildingID)
    {
        foreach(ResourcesNeeded resourceNeeded in BuildingDataBase.GetInstance().GetBuildingData(buildingID).GetResourcesNeeded())
        {
            if (GetResource(resourceNeeded.resourceID) < resourceNeeded.amount)
                return false;
        }
        return true;
    }
    /// <summary>
    /// Deducts your current resources against the resourcesNeeded for the buildingType
    /// </summary>
    /// <param name="buildingID"></param>
    public void DeductResourcesFromBuildingData(BuildingDataBase.BUILDINGS buildingID)
    {
        foreach (ResourcesNeeded resourceNeeded in BuildingDataBase.GetInstance().GetBuildingData(buildingID).GetResourcesNeeded())
        {
            if (!resourceNeeded.toDeduct)
                continue;
            SetResource(resourceNeeded.resourceID, GetResource(resourceNeeded.resourceID) - resourceNeeded.amount);
        }
    }
}
