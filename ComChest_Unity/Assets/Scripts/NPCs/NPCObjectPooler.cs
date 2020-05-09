using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NPCObjectPooler 
{
    [SerializeField] GameObject m_NPCPrefab;
    [SerializeField] int m_PoolNumber = 5;
    [SerializeField] int m_MaxNpcs = 10;

    private List<NPC> m_NPCList = new List<NPC>();

    public void Init()
    {
        CreateNPCs(m_PoolNumber);
    }

    public NPC GetNPC()
    {
        foreach (NPC npc in m_NPCList)
        {
            if (npc.gameObject.activeSelf)
                continue;

            return npc;
        }

        //if npc exceeds limit
        if (m_NPCList.Count >= m_MaxNpcs)
            return null;

        CreateNPCs(m_PoolNumber);
    
        return GetNPC();
    }

    public void CreateNPCs(int number)
    {
        for (int i = 0; i < number; ++i)
        {
            GameObject newNPC = GameObject.Instantiate(m_NPCPrefab);
            newNPC.SetActive(false);

            NPC npc = newNPC.GetComponent<NPC>();
            if (npc != null)
                m_NPCList.Add(npc);
        }
    }

    public List<NPC> GetNPCList()
    {
        return m_NPCList;
    }
}
