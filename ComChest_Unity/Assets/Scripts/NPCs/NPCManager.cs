using System.Collections.Generic;
using UnityEngine;

public class NPCManager : SingletonBase<NPCManager>
{
    [Header("NPC Spawn Map details")]
    [SerializeField] NPCObjectPooler m_NPCObjectPooler = new NPCObjectPooler();
    [SerializeField] List<AnimatorOverrideController> m_NPCAnimationList = new List<AnimatorOverrideController>(); //store the different npc animations
    [Tooltip("Time taken to spawn one NPC")]
    [SerializeField] float m_SpawnRate = 5.0f;

    Dictionary<Vector2Int, bool> m_BuildingEntrances = new Dictionary<Vector2Int, bool>();
    float m_SpawnTimer = 0.0f;
    bool m_EditorModeActive = false;

    // Start is called before the first frame update
    void Start()
    {
        m_NPCObjectPooler.Init();
        m_SpawnTimer = 0.0f;
        m_EditorModeActive = false;

        RoadManager roadManager = MapManager.GetInstance().GetRoadManager();
        if (roadManager != null)
        {
            roadManager.OnRoadModifiedAndDeleatedCallback += RoadsRemoved;
        }
    }

    //TODO:: spawn timer is temp, create algorithm to change spawn rate depending on population and building number
    //TODO:: Make sure max spawn amount is also dependant on population

    // Update is called once per frame
    void Update()
    {
        //dont need spawn any NPC if player is in brush mode
        if (m_EditorModeActive)
            return;

        //when theres an entrance for them to spawn
        if (m_BuildingEntrances.Count == 0)
            return;

        //spawn a NPC every few seconds
        m_SpawnTimer += Time.fixedDeltaTime;
        if (m_SpawnTimer > m_SpawnRate)
        {
            m_SpawnTimer = 0.0f;

            NPC npc = m_NPCObjectPooler.GetNPC();
            if (npc == null)
                return;

            //randomize the animator and spawn position
            AnimatorOverrideController newAnimator = null;
            if (m_NPCAnimationList.Count > 0)
                newAnimator = m_NPCAnimationList[Random.Range(0, m_NPCAnimationList.Count)];

            Vector2Int spawnLocation = Vector2Int.zero;
            if (m_BuildingEntrances.Count > 0)
            {
                List<Vector2Int> entranceList = new List<Vector2Int>(m_BuildingEntrances.Keys);
                spawnLocation = entranceList[Random.Range(0, entranceList.Count)];
            }

            npc.gameObject.SetActive(true);
            npc.Init(spawnLocation, newAnimator);
        }
    }

    //if roads are removed, send to all NPCs to check if the next road or current road they are on are gone
    //if yes, remove those NPCs, set them inactive
    public void RoadsRemoved()
    {
        if (m_NPCObjectPooler == null)
            return;

        List<NPC> npcList = m_NPCObjectPooler.GetNPCList();
        foreach (NPC npc in npcList)
        {
            npc.PlayerRemoveRoads();
        }
    }

    public void AddBuildingEntrance(Vector2Int gridPos)
    {
        if (!m_BuildingEntrances.ContainsKey(gridPos))
            m_BuildingEntrances.Add(gridPos, true);
    }

    //IF BUILDING DELEATED MUST REMOVE THE ENTRANC
    public void RemoveBuildingEntrance(Vector2Int gridPos)
    {
        if (m_BuildingEntrances.ContainsKey(gridPos))
            m_BuildingEntrances.Remove(gridPos);
    }

    //when player is in brush mode and editing the map
    public void PlayerEditorModeActive(bool active)
    {
        if (m_NPCObjectPooler == null)
            return;

        Transform npcParent = m_NPCObjectPooler.GetNPCParent();
        if (npcParent != null)
            npcParent.gameObject.SetActive(!active);

        m_EditorModeActive = active;
    }

    //checks whether its on the building's 'door'
    public bool CheckInFrontOfBuilding(Vector2Int gridPos)
    {
        return m_BuildingEntrances.ContainsKey(gridPos);
    }
}
