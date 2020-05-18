using System.Collections;
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
    Dictionary<RESOURCES, int> m_DictOfResources = new Dictionary<RESOURCES, int>();

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
    }

    void SetResource(RESOURCES _id, int _value)
    {
        m_DictOfResources[_id] = _value;
    }
    int GetResource(RESOURCES _id) { return m_DictOfResources[_id]; }
}
