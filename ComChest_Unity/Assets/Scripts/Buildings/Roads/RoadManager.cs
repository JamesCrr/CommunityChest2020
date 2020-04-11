using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadManager 
{
    Dictionary<int, RoadTypeList> m_RoadMap = new Dictionary<int, RoadTypeList>();
    Dictionary<int, BaseBuildingsClass> m_RoadSpriteRendererMap;

    public bool CheckMapAvailability(int mapIndex) //check if space is taken by the road
    {
        return m_RoadMap.ContainsKey(mapIndex);
    }

    public void PlaceRoads(int mapIndex, ref BaseBuildingsClass roadInfo)
    {
       // m_RoadSpriteRendererMap.Add(mapIndex, roadInfo.GetSpriteRenderer());
        //change sprites accordingly, do check

        //TODO, STORE THE NEW DIRECTIONS ACCORDINGLY
        m_RoadMap.Add(mapIndex, RoadTypeList.NO_CONNECTION); 
    }

    RoadTypeList CheckRoadDirection()
    {
        return RoadTypeList.NO_CONNECTION;
    }
}
