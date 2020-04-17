using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCManager : SingletonBase<NPCManager>
{
    [Header("NPC Spawn Map details")]
    [SerializeField] GameObject m_NPCPrefab;
    [SerializeField] int m_MaxNpcsOnMap = 10;

    Dictionary<Vector2Int, bool> m_BuildingEntrances = new Dictionary<Vector2Int, bool>();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //if roads are removed, send to all NPCs to check if the next road or current road they are on are gone
    //if yes, remove those NPCs, set them inactive
    public void RoadsRemoved()
    {

    }

    //TODO:: IF BUILDING DELEATED MUST REMOVE THE ENTRANCE

    public void AddBuildingEntrance(Vector2Int gridPos)
    {
        if (!m_BuildingEntrances.ContainsKey(gridPos))
            m_BuildingEntrances.Add(gridPos, true);
    }

    //checks whether its on the building's 'door'
    public bool CheckInFrontOfBuilding(Vector2Int gridPos)
    {
        return m_BuildingEntrances.ContainsKey(gridPos);
    }
}
