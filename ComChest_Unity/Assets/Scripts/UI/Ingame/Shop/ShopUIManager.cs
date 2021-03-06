﻿using System.Collections.Generic;
using UnityEngine;

public enum ShopItemType
{
    BUILDINGS,
    DECORATIONS,
    ROAD,
    TOTAL_TYPE,
}

[System.Serializable]
public class ShopUIManager
{
    [SerializeField] Transform[] m_ShopCategoriesParents; //where to spawn the pages
    [SerializeField] GameObject[] m_ShopPages; //the gameobject sections
    [SerializeField] GameObject m_ItemCardUIPrefab;
    [Tooltip("The first shop page the player will always go to")]
    [SerializeField] ShopItemType m_DefaultShopCategory = ShopItemType.BUILDINGS;
    [SerializeField] ShopButton[] m_ShopButtons;

    public void Active()
    {
        InitCategoryShown(m_DefaultShopCategory);
    }

    // Start is called before the first frame update
    public void Init()
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
                GameObject itemCard = GameObject.Instantiate(m_ItemCardUIPrefab, m_ShopCategoriesParents[i]);

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
        InitCategoryShown(m_DefaultShopCategory);
    }

    //shows the default page 
    public void InitCategoryShown(ShopItemType category)
    {
        //show the correct category
        for (int i = 0; i < m_ShopPages.Length; ++i)
        {
            if (m_ShopPages[i] == null)
                continue;

            m_ShopPages[i].SetActive(i == (int)category);
        }

        //show the buttons properly
        for (int i = 0; i < m_ShopButtons.Length; ++i)
        {
            if (m_ShopButtons[i] == null)
                continue;

            m_ShopButtons[i].PutBackground(i == (int)category);
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
