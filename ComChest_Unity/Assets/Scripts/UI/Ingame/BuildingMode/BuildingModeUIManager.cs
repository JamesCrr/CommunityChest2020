using UnityEngine;

[System.Serializable]
public class BuildingModeUIManager 
{
    [SerializeField] GameObject m_PlacementButtons;
    [SerializeField] GameObject m_BuildingGridHelper; //shows a grid around the build size of the item
    [SerializeField] GameObject m_MainParent;

    public void AttachToBuilding(Transform building, Vector2 offset, Vector2Int buildingSizeOnMap)
    {
        if (building == null)
            return;

        m_MainParent.SetActive(true);

        //attach the 'buttons' to the object
        SetParents(building, offset, buildingSizeOnMap);
    }

    public void Detach()
    {
        m_PlacementButtons.transform.SetParent(m_MainParent.transform);
        m_BuildingGridHelper.transform.SetParent(m_MainParent.transform);

        m_MainParent.SetActive(false);
    }

    public void SetParents(Transform parent)
    {
        if (m_PlacementButtons != null)
            m_PlacementButtons.transform.SetParent(parent);
    }

    public void SetParents(Transform parent, Vector2 offset, Vector2Int buildingSizeOnMap)
    {
        if (m_PlacementButtons != null)
        {
            m_PlacementButtons.transform.SetParent(parent);
            m_PlacementButtons.transform.localPosition = new Vector3(offset.x, offset.y, 0.0f);
        }

        if (m_BuildingGridHelper != null)
        {
            m_BuildingGridHelper.transform.SetParent(parent);
            m_BuildingGridHelper.transform.localPosition = Vector3.zero;
            m_BuildingGridHelper.transform.localScale = new Vector3(buildingSizeOnMap.x, buildingSizeOnMap.y, 1.0f);
        }
    }
}
