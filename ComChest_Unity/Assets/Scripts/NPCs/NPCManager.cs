using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCManager : MonoBehaviour
{
    [Header("NPC Spawn Map details")]
    [SerializeField] GameObject m_NPCPrefab;
    [SerializeField] int m_MaxNpcsOnMap = 10;

    [Header("NPC details")]
    [SerializeField] float m_MinSpeed = 0.1f;
    [SerializeField] float m_MaxSpeed = 1.0f;

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
}
