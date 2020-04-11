using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Events")]
public class GameEvent : ScriptableObject
{
    [Header("Event Data")]
    [Tooltip("Name of the event, used for UI event announcement")]
    [SerializeField] string m_EventName = "Unnamed Event";

    [Tooltip("Resource affected by event")]
    [SerializeField] GameEventManager.EventResource m_EventResource;

    // Might increase the longer that event doesnt trigger (Not sure if to do this yet)
    // Because of that, try to set at a lower number
    [Tooltip("Base chance of event happening")]
    [Range(0, 100)]
    [SerializeField] float m_MinimumEventChance;

    [Tooltip("Base chance of event happening")]
    [Range(0, 100)]
    [SerializeField] float m_MaximumEventChance;

    [Tooltip("Base chance of event happening")]
    [Range(0, 100)]
    [SerializeField] float m_EventChance;

    public string GetSetEventName
    {
        get { return m_EventName; }
        set { m_EventName = value; }
    }

    public float GetSetEventChance
    {
        get { return m_EventChance; }
        set { m_EventChance = value; }
    }

    public float GetSetMinChance
    {
        get { return m_MinimumEventChance; }
        set { m_MinimumEventChance = value; }
    }

    public float GetSetMaxChance
    {
        get { return m_MaximumEventChance; }
        set { m_MaximumEventChance = value; }
    }


}
