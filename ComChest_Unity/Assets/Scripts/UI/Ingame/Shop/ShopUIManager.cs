using System.Collections.Generic;
using UnityEngine;

public enum ShopItemType
{
    BUILDINGS,
    DECORATIONS,
    TOTAL_TYPE,
}

public class ShopUIManager : MonoBehaviour
{
    [SerializeField] Transform[] m_ShopCategoriesParents; //get the parents of the different categories
    [SerializeField] GameObject m_ItemCardUIPrefab;
    [Tooltip("The first shop page the player will always go to")]
    [SerializeField] ShopItemType m_MainShopCategory = ShopItemType.BUILDINGS;

    public void OnEnable()
    {
        InitCategoryShown();
    }

    // Start is called before the first frame update
    void Start()
    {
        //get from the building database the different buildings
        //make an array of list to store the different categories
        List<BuildingData>[] tempBuildingDataStorage = new List<BuildingData>[(int)ShopItemType.TOTAL_TYPE];
        for (int i =0; i < (int)ShopItemType.TOTAL_TYPE; ++i)
        {
            tempBuildingDataStorage[i] = new List<BuildingData>();
        }

        //add the buildings to the correct category
        for (int i=0; i < (int)BuildingDataBase.BUILDINGS.B_TOTAL; ++i)
        {
            BuildingData buildingData = BuildingDataBase.GetInstance().GetBuildingData((BuildingDataBase.BUILDINGS)(i));
            if (buildingData == null)
                return;

            //item not suppose to be in shop
            if (!buildingData.GetIsSoldInShop()) 
                continue;

            //add the building into the correct category
            tempBuildingDataStorage[(int)buildingData.GetShopItemType()].Add(buildingData);
        }

        for (int i = 0; i < (int)ShopItemType.TOTAL_TYPE; ++i)
        {
            //sort the buildings
            SortBuildings sortBuildings = new SortBuildings();
            tempBuildingDataStorage[i].Sort(sortBuildings);

            //make sure parent exist first
            if (i >= m_ShopCategoriesParents.Length)
                break;

            //spawn the prefabs and add to the correct parent
            foreach (BuildingData buildingData in tempBuildingDataStorage[i])
            {
                GameObject itemCard = Instantiate(m_ItemCardUIPrefab, m_ShopCategoriesParents[i]);

                //init the UI with the building info accordingly
                ShopItemCardUI shopItemCardUI = itemCard.GetComponent<ShopItemCardUI>();
                if (shopItemCardUI == null)
                    continue;

                shopItemCardUI.Init(buildingData);
            }

            //clear the list
            tempBuildingDataStorage[i].Clear();
        }

        //set the shop category inactive and active accordingly
        InitCategoryShown();
    }

    //shows the default page 
    public void InitCategoryShown()
    {
        for (int i = 0; i < m_ShopCategoriesParents.Length; ++i)
        {
            if (m_ShopCategoriesParents[i] == null)
                continue;

            m_ShopCategoriesParents[i].gameObject.SetActive(i == (int)m_MainShopCategory);
        }
    }

    //class to help compare and sort the buildings in ascending order
    class SortBuildings : IComparer<BuildingData>
    {
        public int Compare(BuildingData building1, BuildingData building2)
        {
            if (building1 == null || building2 == null)
                return 0;

            return building1.GetPrice().CompareTo(building2.GetPrice());
        }
    }
}
