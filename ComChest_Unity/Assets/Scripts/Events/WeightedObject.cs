using System;
using System.Collections.Generic;

// Template class from
// https://gamedev.stackexchange.com/questions/162976/how-do-i-create-a-weighted-collection-and-then-pick-a-random-element-from-it


class WeightedObject<T>
{
    private struct Entry
    {
        public double accumulatedWeight;
        public T item;
    }

    private List<Entry> entries = new List<Entry>();
    private double accumulatedWeight;
    private Random rand = new Random();
    
    // Adds a weighted object to the list
    public void AddEntry(T item, double weight)
    {
        accumulatedWeight += weight;
        entries.Add(new Entry { item = item, accumulatedWeight = accumulatedWeight });
    }

    // To get random event
    public T GetRandom()
    {
        // Generate a random number between the sums of all the weight
        double r = rand.NextDouble() * accumulatedWeight;

        // Loop through all the events in the list
        foreach (Entry entry in entries)
        {
            // if r is more than any of those events, they trigger
            if (entry.accumulatedWeight >= r )
            {
                return entry.item;
            }
        }
        return default(T); // Only if there are no entries
    }

    // To wipe the list
    // Needed for game event as the random chances might change
    public void ClearList()
    {
        entries.Clear();
    }

}