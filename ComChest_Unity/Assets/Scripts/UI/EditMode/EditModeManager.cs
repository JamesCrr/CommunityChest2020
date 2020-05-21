using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum EDITMODES
{
    MOVE_OBJECTS,
    DESTROY,
}

public class EditModeManager : MonoBehaviour
{
    [SerializeField] EDITMODES m_DefaultMode = EDITMODES.MOVE_OBJECTS;

    [Header("Button Modes UI")]
    [SerializeField] Button[] m_ButtonModes;
    [SerializeField] Color m_GreyedOutColor;

    EDITMODES m_CurrentMode = EDITMODES.MOVE_OBJECTS;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnEnable()
    {
        //activate default mode
        ChangeMode(m_DefaultMode);
    }

    public void ChangeMode(EDITMODES modes)
    {
        m_CurrentMode = modes;

        if (m_CurrentMode == EDITMODES.DESTROY)
        {
            //make the button for the other mode dulled out
            //make button for current mode proper

            //activate proper mode in mapmanager
            OpenDeleteBrush(true);

        }
        else if (m_CurrentMode == EDITMODES.MOVE_OBJECTS)
        {
            OpenDeleteBrush(false);

        }

        UpdateModeChangeUI();
    }

    //when players want to confirm their changes
    public void ConfirmChangeOpenPopup()
    {
        //check current mode
        //show correct popup to confirm
        //activate
        
    }

    //when players want to go back to the previous page and cancel their change
    public void CancelChangeOpenPopup()
    {
        //show popup
    }

    //player in pop up mode, confirm
    public void PopupConfirm()
    {
        //activate changes
    }

    public void PopupCancel()
    {
        //close popup, go back to the edit page
    }

    public void UpdateModeChangeUI()
    {
        //for (int i = 0; i < m_ButtonModes.Length; ++i)
        //{
        //    m_ButtonModes[i].colors = m_GreyedOutColor;
        //}
    }

    #region Delete Mode
    public void OpenDeleteBrush(bool open)
    {
        MapManager.GetInstance().SetRemovalBrush(open);
    }
    #endregion
}
