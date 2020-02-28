using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Linq;

public class MobileInput : MonoBehaviour
{
    // Public Static Instance
    static MobileInput m_Instance = null;

    // UI Graphics Raycasting
    GraphicRaycaster m_Raycaster = null;
    PointerEventData m_PointerEventData = null;
    [SerializeField]
    EventSystem m_EventSystem = null;

    // Lists of Touches that are currently active
    List<MobileTouch> m_listOfActiveTouches = new List<MobileTouch>();

    [Header("Mobile UI Parent")]
    [SerializeField]
    GameObject m_mobileUIParent = null;

    public static MobileInput GetInstance() { return m_Instance; }
    private void Awake()
    {
        m_mobileUIParent.SetActive(false);

        if (m_Instance != null)       // Make this a public Instance
        {
            Destroy(this.gameObject);
            return;
        }
        m_Instance = this;
        DontDestroyOnLoad(this.gameObject);

        m_Raycaster = m_mobileUIParent.GetComponent<GraphicRaycaster>();

        // Create new MobileTouch and add into list
        MobileTouch newTouch = new MobileTouch();
        MobileTouch newTouch2 = new MobileTouch();
        MobileTouch newTouch3 = new MobileTouch();
        MobileTouch newTouch4 = new MobileTouch();
        m_listOfActiveTouches.Add(newTouch);
        m_listOfActiveTouches.Add(newTouch2);
        m_listOfActiveTouches.Add(newTouch3);
        m_listOfActiveTouches.Add(newTouch4);
    }

/*#if UNITY_ANDROID || UNITY_IOS*/    // playing on Phones
    private void Update()
    { 
        // Any touches on the screen
        if (Input.touchCount < 1)
            return;

        MobileTouch activeTouch = null;
        for (int i = 0; i < Input.touches.Length; ++i)
        {
            activeTouch = GetMobileTouch(Input.touches[i].fingerId);
            
            // React differently to different states
            switch (Input.touches[i].phase)
            {
                case TouchPhase.Began:          // A new Touch just touched the screen
                    {
                        MobileTouch newTouch = GetDeactivatedMobileTouch();
                        // Activate the Touch Object
                        newTouch.ActivateTouch(Input.touches[i].fingerId);
                        newTouch.startTouchPos = Input.touches[i].position;
                        newTouch.lastTouchedPos = newTouch.startTouchPos;

                        //Debug.Log(i + ": ENTERED");
                    }
                    break;
                case TouchPhase.Moved:          // Touch moved on the screen
                    {
                        //CheckCollidedWithGO(activeTouch);

                        activeTouch.lastTouchedPos = Input.touches[i].position;
                        activeTouch.swipeDelta = Input.touches[i].deltaPosition;
                        // Passed Deadzone?
                        if (activeTouch.swipeDelta.sqrMagnitude < (15*15))
                        {
                            activeTouch.swipeDelta = Vector2.zero;
                            activeTouch.isSwiping = false;
                            break;
                        }

                        // Check which direction was the largest
                        activeTouch.isSwiping = true;
                        if (Mathf.Abs(activeTouch.swipeDelta.x) > Mathf.Abs(activeTouch.swipeDelta.y))
                        {
                            // Left or Right
                            if (activeTouch.swipeDelta.x < 0)
                            {
                                activeTouch.swipeLeft = true;
                                //Debug.Log(i + ": LEFTT");
                            }
                            else
                            {
                                activeTouch.swipeRight = true;
                                //Debug.Log(i + ": RIGHTTT");
                            }
                        }
                        else
                        {
                            // Up or Down
                            if (activeTouch.swipeDelta.y < 0)
                            {
                                activeTouch.swipeDown = true;
                                //Debug.Log(i + ": DOOWN");
                            }
                            else
                            {
                                activeTouch.swipeUp = true;
                                //Debug.Log(i + ": UPP");
                            }
                        }

                    }
                    break;
                case TouchPhase.Stationary:     // Still touching the screen, but hasn’t moved since the last frame
                    {
                        activeTouch.lastTouchedPos = Input.touches[i].position;
                        //Debug.Log(Camera.main.ScreenToViewportPoint(activeTouch.lastTouchedPos));
                    }
                    break;
                case TouchPhase.Ended:
                case TouchPhase.Canceled:
                    {
                        activeTouch.DeactivateTouch();
                    }
                    break;
            }


        }

    }
//#endif

    GameObject CheckTouchingUI(MobileTouch newTouch)
    {
        // Reset selected GO
        newTouch.currentSelectedGO = null;
        // Set up the new Pointer Event
        m_PointerEventData = new PointerEventData(m_EventSystem);
        m_PointerEventData.position = newTouch.lastTouchedPos;
        // Raycast using Graphics Raycaster for UI
        List<RaycastResult> results = new List<RaycastResult>();
        m_Raycaster.Raycast(m_PointerEventData, results);

        //For every result returned, output the name of the GameObject on the Canvas hit by the Ray
        foreach (RaycastResult result in results)
        {
            Debug.Log("Hit " + result.gameObject.name);

            // Attach the collided GO
            newTouch.currentSelectedGO = result.gameObject;
            // To make sure only detect the first UI Hit
            break;
        }

        // return result
        return newTouch.currentSelectedGO;
    }
    public GameObject CheckTouchingGO(MobileTouch newTouch, int layerMaskID = 0)
    {
        // Reset selected GO
        newTouch.currentSelectedGO = null;

        // If it hits something...
        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(newTouch.lastTouchedPos), Vector2.zero, 20.0f, 1 << layerMaskID);
        if (hit.collider != null)
        {
            newTouch.currentSelectedGO = hit.transform.gameObject;
            Debug.Log(newTouch.currentSelectedGO.name);
        }

        //RaycastHit hit;
        //Ray ray = Camera.main.ScreenPointToRay(newTouch.lastTouchedPos);
        //if (Physics2D.Raycast(ray, out hit, 200.0f, 1 << layerMaskID))
        //{
           
        //}
        return newTouch.currentSelectedGO;
    }
    public GameObject CheckTouchingGO(int layerMaskID = 0)
    {
        MobileTouch newTouch = m_listOfActiveTouches[Input.touches[0].fingerId];
        // Reset selected GO
        newTouch.currentSelectedGO = null;

        // If it hits something...
        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(newTouch.lastTouchedPos), Vector2.zero, 20.0f, 1 << layerMaskID);
        if (hit.collider != null)
        {
            newTouch.currentSelectedGO = hit.transform.gameObject;
            Debug.Log(newTouch.currentSelectedGO.name);
        }

        //RaycastHit hit;
        //Ray ray = Camera.main.ScreenPointToRay(newTouch.lastTouchedPos);
        //if (Physics2D.Raycast(ray, out hit, 200.0f, 1 << layerMaskID))
        //{

        //}
        return newTouch.currentSelectedGO;
    }
    MobileTouch GetDeactivatedMobileTouch()
    {
        for (int i = 0; i < m_listOfActiveTouches.Count; ++i)
        {
            if (m_listOfActiveTouches[i].activated == false)
                return m_listOfActiveTouches[i];
        }
        return null;
    }
    MobileTouch GetMobileTouch(int newFingerId)
    {
        for(int i = 0; i < m_listOfActiveTouches.Count; ++i)
        {
            if (m_listOfActiveTouches[i].fingerID == newFingerId)
                return m_listOfActiveTouches[i];
        }
        return null;
    }


    /// <summary>
    /// FOR CANVAS GameObject ONLY..
    /// Returns whether any Mobile Touch is currently touching the objToTouch
    /// </summary>
    /// <param name="objToTouch"></param>
    /// <returns></returns>
    public bool IsFingerTouching_UI(GameObject objToTouch)
    {
        for (int i = 0; i < m_listOfActiveTouches.Count; ++i)
        {
            if (!m_listOfActiveTouches[i].activated)
                continue;
            if (CheckTouchingUI(m_listOfActiveTouches[i]) == objToTouch)
                return true;
        }
        return false;
    }
    public bool IsFingerTouching_UI(GameObject objToTouch, int fingerID)
    {
        if (CheckTouchingUI(m_listOfActiveTouches[fingerID]) == objToTouch)
            return true;
        return false;
    }
    /// <summary>
    /// Returns whether any Mobile Touch is currently touching the objToTouch
    /// </summary>
    /// <param name="objToTouch"></param>
    /// <returns></returns>
    public bool IsFingerTouching_GO(GameObject objToTouch, int layerMaskID = 0)
    {
        for (int i = 0; i < m_listOfActiveTouches.Count; ++i)
        {
            if (!m_listOfActiveTouches[i].activated)
                continue;
            if (CheckTouchingGO(m_listOfActiveTouches[i], layerMaskID) == objToTouch)
                return true;
        }
        return false;
    }
    public bool IsFingerTouching_GO(GameObject objToTouch, int fingerID, int layerMaskID = 0)
    {
        if (CheckTouchingGO(m_listOfActiveTouches[fingerID], layerMaskID) == objToTouch)
            return true;
        return false;
    }

    /// <summary>
    /// Returns how many fingers are Touching the screen in this Frame.
    /// EVEN IF they are ending or canceling
    /// </summary>
    /// <returns></returns>
    public int GetTouchCount() { return Input.touchCount; }
    /// <summary>
    /// Returns a Touch Object's current Touching Phase
    /// </summary>
    /// <param name="fingerID"></param>
    /// <returns></returns>
    public TouchPhase GetTouchPhase(int fingerID = 0) 
    {
        if (!m_listOfActiveTouches[fingerID].activated)
            return TouchPhase.Began;
        return Input.touches[fingerID].phase;
    }
    /// <summary>
    /// Returns the difference between Starting and Current Pixel position of a Finger touching the screen
    /// </summary>
    /// <param name="fingerID"></param>
    /// <returns></returns>
    public Vector2 GetSwipeDelta(int fingerID = 0)
    {
        if (!m_listOfActiveTouches[fingerID].activated)
            return Vector2.zero;
        return m_listOfActiveTouches[fingerID].swipeDelta;
    }
    public Vector2 GetLastTouchedPosition(int fingerID = 0)
    {
        if (!m_listOfActiveTouches[fingerID].activated)
            return Vector2.zero;
        return m_listOfActiveTouches[fingerID].lastTouchedPos;
    }
    public Vector2 GetStartTouchPosition(int fingerID = 0)
    {
        if (!m_listOfActiveTouches[fingerID].activated)
            return Vector2.zero;
        return m_listOfActiveTouches[fingerID].startTouchPos;
    }

}

public class MobileTouch
{
    // Is this Mobile Touch being used?
    public bool activated = false;

    // Storing the game object we collided with
    public GameObject currentSelectedGO;
    // Identifier
    public int fingerID;
    // Input Related
    public bool swipeLeft, swipeRight, swipeUp, swipeDown;
    public bool isSwiping;
    public Vector2 startTouchPos, lastTouchedPos, swipeDelta;

    public MobileTouch()
    {
        currentSelectedGO = null;
        isSwiping = swipeLeft = swipeRight = swipeUp = swipeDown = false;
        startTouchPos = swipeDelta = lastTouchedPos = Vector2.zero;
    }

    public void ResetSwipe()
    {
        isSwiping = swipeLeft = swipeRight = swipeUp = swipeDown = false;
        swipeDelta = Vector2.zero;
    }

    public void ActivateTouch(int newFingerID)
    {
        fingerID = newFingerID;

        activated = true;
    }
    public void DeactivateTouch()
    {
        fingerID = -1;
        currentSelectedGO = null;
        startTouchPos = lastTouchedPos = Vector2.zero;
        ResetSwipe();

        activated = false;
    }

}