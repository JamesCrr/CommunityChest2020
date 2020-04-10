using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadManager : MonoBehaviour
{
    Dictionary<int, RoadTypeList> m_RoadMap;
    Dictionary<int, SpriteRenderer> m_RoadSpriteRendererMap;

    RoadTypeList CheckRoadDirection()
    {
        return RoadTypeList.NO_CONNECTION;
    }
}
