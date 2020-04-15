using UnityEngine;

public class ResourceBuildings : RoadBuildings
{
    [Header("Resource Info")]
    float m_GenerateResourceTime = 0.0f;
    BuildingDataBase.RESOURCES m_ResourceTypeProduced = BuildingDataBase.RESOURCES.R_NONE;
    int m_AmtResourceProduce = 0;
    Vector2 m_ResourceUIOffset;

    float m_ResourceTimer = 0.0f;
    bool m_ResourceCollected = true;

    public override void SetNewBuildingType(BuildingData buildingData)
    {
        base.SetNewBuildingType(buildingData);

        m_GenerateResourceTime = buildingData.GetResourceTime;
        m_ResourceTypeProduced = buildingData.GetResourceTypeProduced;
        m_AmtResourceProduce = buildingData.GetAmtResource;
        m_ResourceUIOffset = buildingData.GetResourceUIOffset;

        m_ResourceCollected = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (!m_ResourceCollected)
            return;

        m_ResourceTimer += Time.deltaTime;
        if (m_ResourceTimer < m_GenerateResourceTime)
            return;

        m_ResourceTimer = 0.0f;

        //spawn out the icon
        ResourceBubbleManager.Instance.InitResourceBubbleUI(m_ResourceTypeProduced, m_AmtResourceProduce, new Vector2(transform.position.x, transform.position.y) + m_ResourceUIOffset, this);
    }

    public bool GetSetResourceCollected
    {
        get { return m_ResourceCollected; }
        set { m_ResourceCollected = value; }
    }
}
