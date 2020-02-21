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

    // Main Camera
    Camera m_mainCamera = null;
    [Header("Mobile UI Parent")]
    [SerializeField]
    GameObject m_mobileUIParent = null;

    public static MobileInput GetInstance() { return m_Instance; }
    private void Awake()
    {
#if UNITY_STANDALONE    // If we are playing in PC, disable mobileInput's Canvas
        m_mobileUIParent.SetActive(false);
#elif UNITY_ANDROID || UNITY_IOS    // playing on Phones

        if (m_Instance != null)       // Make this a public Instance
        {
            Destroy(this.gameObject);
            return;
        }
        m_Instance = this;
        DontDestroyOnLoad(this.gameObject);

        m_Raycaster = m_mobileUIParent.GetComponent<GraphicRaycaster>();
        m_mainCamera = Camera.main;

        // Create new MobileTouch and add into list
        MobileTouch newTouch = new MobileTouch();
        MobileTouch newTouch2 = new MobileTouch();
        MobileTouch newTouch3 = new MobileTouch();
        m_listOfActiveTouches.Add(newTouch);
        m_listOfActiveTouches.Add(newTouch2);
        m_listOfActiveTouches.Add(newTouch3);
#endif
    }

#if UNITY_ANDROID || UNITY_IOS    // playing on Phones
    private void Update()
    { 
        // Any touches on the screen
        if (Input.touches.Length < 1)
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
                        newTouch.ActivateTouch(Input.touches[i].fingerId, Input.touches[i]);
                        newTouch.startTouch = Input.touches[i].position;
                        newTouch.lastTouchedPos = newTouch.startTouch;
                        // Get Collided GO
                        CheckCollidedWithUI(newTouch);

                        Debug.Log(i + ": ENTERED");
                    }
                    break;
                case TouchPhase.Moved:          // Touch moved on the screen
                    {
                        activeTouch.lastTouchedPos = Input.touches[i].position;
                        activeTouch.swipeDelta = activeTouch.lastTouchedPos - activeTouch.startTouch;
                        // Passed Deadzone?
                        if (activeTouch.swipeDelta.sqrMagnitude < (35*35))
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
                                Debug.Log(i + ": LEFTT");
                            }
                            else
                            {
                                activeTouch.swipeRight = true;
                                Debug.Log(i + ": RIGHTTT");
                            }
                        }
                        else
                        {
                            // Up or Down
                            if (activeTouch.swipeDelta.y < 0)
                            {
                                activeTouch.swipeDown = true;
                                Debug.Log(i + ": DOOWN");
                            }
                            else
                            {
                                activeTouch.swipeUp = true;
                                Debug.Log(i + ": UPP");
                            }
                        }

                    }
                    break;
                case TouchPhase.Stationary:     // Still touching the screen, but hasn’t moved since the last frame
                    {
                        // No longer swiping
                        //activeTouch.ResetSwipe();
                        activeTouch.lastTouchedPos = Input.touches[i].position;
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

        #region Swiping
        //// Calculate the Distance
        //swipeDelta = Vector2.zero;
        //if(isDragging)
        //{
        //    if (Input.touches.Length > 0)
        //        swipeDelta = Input.touches[0].position - startTouch;
        //    else if (Input.GetMouseButton(0))
        //        swipeDelta = (Vector2)Input.mousePosition - startTouch;
        //}

        //// Check if we crossed the deadZone
        //if(swipeDelta.magnitude > 50)
        //{
        //    // Check which direction was the swipe
        //    float x = swipeDelta.x;
        //    float y = swipeDelta.y;

        //    // Check which direction was the largest
        //    if (Mathf.Abs(x) > Mathf.Abs(y))
        //    {
        //        // Left or Right
        //        if (x < 0)
        //        {
        //            swipeLeft = true;
        //            //Debug.Log("LEFTT");
        //        }
        //        else
        //        {
        //            swipeRight = true;
        //            //Debug.Log("RIGHTTT");
        //        }
        //    }
        //    else
        //    {
        //        // Up or Down
        //        if (y < 0)
        //        {
        //            swipeDown = true;
        //            //Debug.Log("DOOWN");
        //        }
        //        else
        //        {
        //            swipeUp = true;
        //            //Debug.Log("UPP");
        //        }
        //    }

        //    Reset();
        //}
        #endregion
    }
#endif

    GameObject CheckCollidedWithUI(MobileTouch newTouch)
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
    void RemoveMobileTouch(int newFingerID)
    {
        for (int i = 0; i < m_listOfActiveTouches.Count; ++i)
        {
            if (m_listOfActiveTouches[i].fingerID == newFingerID)
                m_listOfActiveTouches[i].DeactivateTouch();
            return;
        }
    }

    /// <summary>
    /// Returns the Mobile Touch that is currently touching the objectTouching
    /// </summary>
    /// <param name="objectTouching"></param>
    /// <returns></returns>
    public MobileTouch GetMobileTouch_TouchingMe(GameObject objectTouching)
    {
        for (int i = 0; i < m_listOfActiveTouches.Count; ++i)
        {
            if (m_listOfActiveTouches[i].currentSelectedGO == objectTouching)
                return m_listOfActiveTouches[i];
        }
        return null;
    }
}

public class MobileTouch
{
    // Is this Mobile Touch being used?
    public bool activated = false;

    // Storing the game object we collided with
    public GameObject currentSelectedGO;
    public Touch touchObject;
    // Identifier
    public int fingerID;
    // Input Related
    public bool swipeLeft, swipeRight, swipeUp, swipeDown;
    public bool isSwiping;
    public Vector2 startTouch, swipeDelta, lastTouchedPos;

    public MobileTouch()
    {
        currentSelectedGO = null;
        isSwiping = swipeLeft = swipeRight = swipeUp = swipeDown = false;
        startTouch = swipeDelta = lastTouchedPos = Vector2.zero;
    }

    public void ResetSwipe()
    {
        isSwiping = swipeLeft = swipeRight = swipeUp = swipeDown = false;
        swipeDelta = Vector2.zero;
    }

    public void ActivateTouch(int newFingerID, Touch newTouchObject)
    {
        fingerID = newFingerID;
        touchObject = newTouchObject;

        activated = true;
    }
    public void DeactivateTouch()
    {
        fingerID = -1;
        currentSelectedGO = null;
        startTouch = lastTouchedPos = Vector2.zero;
        ResetSwipe();

        activated = false;
    }

}