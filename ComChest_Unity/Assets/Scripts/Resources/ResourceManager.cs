﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    // All possible resources
    public enum RESOURCES        
    {
        R_MONEY,
        R_HAPPINESS,

        R_NONE
    }
    // To hold all of the resources
    Dictionary<RESOURCES, int> m_DictOfResources = new Dictionary<RESOURCES, int>();
    // Resources Changed Action
    public delegate void ResourceChangedAction();
    public static event ResourceChangedAction OnResourceChanged;

    static ResourceManager m_Instance = null;
    public static ResourceManager GetInstance() { return m_Instance; }
    private void Awake()
    {
        if (m_Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        m_Instance = this;
        DontDestroyOnLoad(gameObject);
        // Allocate Dict space
        for(int i = 0; i < (int)RESOURCES.R_NONE; ++i)
        {
            m_DictOfResources.Add((RESOURCES)i, 0);
        } 
    }

    public void SetResource(RESOURCES _id, int _value)
    {
        m_DictOfResources[_id] = _value;
        // Fire resource changed event
        OnResourceChanged();
    }
    public int GetResource(RESOURCES _id) { return m_DictOfResources[_id]; }
}