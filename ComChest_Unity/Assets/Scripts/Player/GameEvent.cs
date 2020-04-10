using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Events")]
public class GameEvent : ScriptableObject
{
    [Header("Event Data")]
    [SerializeField]
    string m_EventName = "Unnamed Event";
    

}
