using UnityEngine;

public class InteractableObjBase : MonoBehaviour
{
#if UNITY_EDITOR || UNITY_STANDALONE
    public void OnMouseDown() //for debugging purposes
    {
        OnInteract();
    }
#endif

    public virtual void OnInteract()
    {
        return;
    }
}
