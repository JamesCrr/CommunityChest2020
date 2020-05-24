using UnityEngine;

[System.Serializable]
public class BuildingModeUIManager 
{
    [SerializeField] GameObject m_PlacementButtons;
    [SerializeField] GameObject m_BuildingGridHelper; //shows a grid around the build size of the item
    [SerializeField] GameObject m_MainParent; //for all the things we need to attach to the buildings when building them

    [Header("Grid Background")]
    [SerializeField] GameObject m_Grid;
    [SerializeField] Material m_GridMaterial;    

    public void Init()
    {
        //set the building grid background accordingly
        BaseMapClass map = MapManager.GetInstance().GetCurrentMap();
        if (map != null)
        {
            Vector2Int size = map.GetTileMapSize();

            if (m_Grid != null)
                m_Grid.transform.localScale = new Vector3(size.x, size.y, 1.0f);

            if (m_GridMaterial != null)
                m_GridMaterial.SetVector("_Tiling", new Vector4(size.x, size.y, 1.0f,1.0f));
        }

        m_Grid.SetActive(false);
    }

    public void AttachToBuilding(Transform building, Vector2 offset, Vector2Int buildingSizeOnMap, Vector2Int buildingSpriteSize)
    {
        if (building == null)
            return;

        m_MainParent.SetActive(true);
        m_Grid.SetActive(true);

        //attach the 'buttons' to the object
        SetParents(building, offset, buildingSizeOnMap, buildingSpriteSize);
    }

    public void Detach(bool setGridActive = false)
    {
        m_PlacementButtons.transform.SetParent(m_MainParent.transform);
        m_BuildingGridHelper.transform.SetParent(m_MainParent.transform);

        m_MainParent.SetActive(false);
        m_Grid.SetActive(setGridActive);
    }

    public void SetParents(Transform parent)
    {
        if (m_PlacementButtons != null)
            m_PlacementButtons.transform.SetParent(parent);
    }

    public void SetParents(Transform parent, Vector2 offset, Vector2Int buildingSizeOnMap, Vector2Int buildingSpriteSize)
    {
        if (m_PlacementButtons != null)
        {
            m_PlacementButtons.transform.SetParent(parent);
            m_PlacementButtons.transform.localPosition = new Vector3(offset.x, offset.y, 0.0f);
        }

        if (m_BuildingGridHelper != null)
        {
            m_BuildingGridHelper.transform.SetParent(parent);

            //get grid size * by row/col to get mid pt of x and y
            Vector2 sizeOfGrid = MapManager.GetInstance().GetCellSize();
            Vector2 midPtOfBuildingSizeOnMap = new Vector2((buildingSizeOnMap.x * sizeOfGrid.x) / 2.0f, (buildingSizeOnMap.y * sizeOfGrid.y) / 2.0f);
            Vector2 midPtOfBuildingActual = new Vector2((buildingSpriteSize.x * sizeOfGrid.x) / 2.0f, (buildingSpriteSize.y * sizeOfGrid.y) / 2.0f);

            m_BuildingGridHelper.transform.localPosition = midPtOfBuildingSizeOnMap - midPtOfBuildingActual;

            m_BuildingGridHelper.transform.localScale = new Vector3(buildingSizeOnMap.x, buildingSizeOnMap.y, 1.0f);
        }
    }

    public void ShowBackgroundGridOnly()
    {
        if (m_Grid != null)
            m_Grid.SetActive(true);

        m_MainParent.SetActive(false);
    }
}
