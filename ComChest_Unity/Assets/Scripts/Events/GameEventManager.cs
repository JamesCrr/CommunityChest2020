using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEventManager : SingletonBase<GameEventManager>
{
    // TODO: Decide whether or not event chance should increase over time or kept constant
    
    // Keeps track of what resource the event is affecting
    public enum EventResource
    {
        NOTHING, // No event happens
        HAPPINESS,
        WATER,
        POPULATION,
        TOTALEVENTS
    }

    [Tooltip("List of possible events")]
    [SerializeField] List<GameEvent> ListOfEvents = new List<GameEvent>();

    [Header("Events Configuration")]

    // jk shifted event chance into the scripted object
    // Leaving this here incase
    /*
    // Represents the chances of an event
    // Keep the number low because events shouldn't happen often
    // The longer there isn't an event the higher the chance will increase
    // This will represent the base chance of an event ( ideally should be 0.05 or smth)
    //[Tooltip("Chance of Event")]
    //[Range(0, 1)]
    //[SerializeField] float m_EventChance;*
     */


    // Represents when the manager will check if an event will trigger based on event chance
    // The higher the number, the lesser the number of times the manager will check
    // Keep the number high as events are not suppose to happen often
    [Tooltip("Buffer Time between each check for events")]
    [SerializeField] float m_EventBufferTime;

    float elapsedTime = 0.0f;

    // Internal list of game events
    // Different from ListOfEvents as they are a storage for the scripted objects
    WeightedObject<GameEvent> gameEvents = new WeightedObject<GameEvent>();

    // Created a random chance "algorithm" based on this solution
    // https://gamedev.stackexchange.com/questions/162976/how-do-i-create-a-weighted-collection-and-then-pick-a-random-element-from-it

    // Start is called before the first frame update
    void Start()
    {
        // Prep the gameEvents list
        for (int i = 0; i < ListOfEvents.Count; ++i)
        {
            gameEvents.AddEntry(ListOfEvents[i], ListOfEvents[i].GetSetEventChance);
            //ListOfEvents[i].GetSetEventChance = 5;
        }
    }

    // Update is called once per frame
    void Update()
    {
        elapsedTime += Time.deltaTime;

        Debug.Log("Current Elapsed Time " + elapsedTime);

        // If eleapsed time reaches the buffer event time
        if (elapsedTime >= m_EventBufferTime)
        {
            GameEvent currEvent = gameEvents.GetRandom();

            if (currEvent == null)
            {
                Debug.LogWarning("No event happened");
                // idk update the chances or smth
            }
            else
            {
                // Print the name for now
                Debug.Log(currEvent.GetSetEventName);
                Debug.LogWarning("Event Happened " + currEvent.GetSetEventName);

            }


            elapsedTime = 0.0f;
        }
    }
}
