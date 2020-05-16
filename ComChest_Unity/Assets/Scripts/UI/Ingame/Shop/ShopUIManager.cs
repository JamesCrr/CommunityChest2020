using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ShopItemType
{
    BUILDINGS,
    //DECORATIONS,
    TOTAL_TYPE,
}

public class ShopUIManager : MonoBehaviour
{
    [SerializeField] Transform[] m_ShopCategoriesParents; //get the parents of the different categories
    [SerializeField] GameObject m_ItemCardUIPrefab;

    // Start is called before the first frame update
    void Start()
    {
        //get from the building database the different buildings
        //BuildingDataBase.GetInstance().get
        //make an array of list
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

            //spawn the prefabs and add to the correct parent
            foreach (BuildingData buildingData in tempBuildingDataStorage[i])
            {
                //make sure parent exist first
                if (i >= m_ShopCategoriesParents.Length)
                    break;

                GameObject itemCard = Instantiate(m_ItemCardUIPrefab, m_ShopCategoriesParents[i]);

                //init the UI with the building info accordingly
                ShopItemCardUI shopItemCardUI = itemCard.GetComponent<ShopItemCardUI>();
                if (shopItemCardUI == null)
                    continue;

                shopItemCardUI.Init(buildingData);
            }

            //TODO:: clear the list
        }

        //TODO:: set the shop category inactive and active accordingly

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
