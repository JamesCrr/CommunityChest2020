using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapDataBase : MonoBehaviour
{
    public enum MAPS        // To reference all possible Maps
    {
        M_GRASS,

        M_TOTAL
    }
    [SerializeField]        // Temporary storage of Maps (ONLY In the Editor)
    List<MapDataEntry> listOfMaps = new List<MapDataEntry>();
    // To Store all possible Maps when running
    Dictionary<MAPS, BaseMapClass> dictOfMaps = new Dictionary<MAPS, BaseMapClass>();

    static MapDataBase m_Instance = null;
    public static MapDataBase GetInstance() { return m_Instance; }
    private void Awake()
    {
        if (m_Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        m_Instance = this;
        DontDestroyOnLoad(gameObject);

        // Check for duplicates
        int counter;
        for(int i = 0; i < (int)MAPS.M_TOTAL; ++i)
        {
            counter = 0;
            foreach (MapDataEntry dataEntry in listOfMaps)
            {
                if (dataEntry.GetMapID() == (MAPS)i)
                    counter++;
            }
            if (counter > 1)
            {
                Debug.LogError("Duplicate Enum ID in MAP Database!!");
                return;
            }
        }
        // If no duplicates, convert to Dictionary
        foreach (MapDataEntry dataEntry in listOfMaps)
        {
            dictOfMaps[dataEntry.GetMapID()] = dataEntry.GetMapObject();
        }
        listOfMaps.Clear();
    }

    public GameObject GetMapGO(MapDataBase.MAPS mapType) { return dictOfMaps[mapType].gameObject; }
    public BaseMapClass GetMapCom(MapDataBase.MAPS mapType) { return dictOfMaps[mapType]; }

}

[System.Serializable]
public class MapDataEntry
{
    [SerializeField]
    MapDataBase.MAPS mapID = MapDataBase.MAPS.M_GRASS;
    [SerializeField]
    BaseMapClass mapObject = null;

    public MapDataBase.MAPS GetMapID() { return mapID; }
    public BaseMapClass GetMapObject() { return mapObject; }
}
