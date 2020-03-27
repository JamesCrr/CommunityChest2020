using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Building")]]
public class MuseumData : ScriptableObject
{
    [Header("UI Building Data")]
    [SerializeField] string m_ContentName = "Name of UI Screen";

    [Tooltip("Key in the text that the building UI description should have")]
    [TextArea(10, 10)]
    public string m_ContentDescription = "";

    [Tooltip("Image for the museum stories")]
    [SerializeField] Sprite StoryImage;


    #region Getters
    
    public string GetName()
    {
        return m_ContentName;
    }

    public string GetDescription()
    {
        return m_ContentDescription;
    }

    public Sprite GetStorySprite()
    {
        return StoryImage;
    }
    
    #endregion
}
