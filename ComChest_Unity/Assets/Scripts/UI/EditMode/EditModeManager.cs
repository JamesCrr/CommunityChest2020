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
    [SerializeField] Image[] m_ButtonModesImage;
    [SerializeField] Color m_GreyedOutColor;

    EDITMODES m_CurrentMode = EDITMODES.MOVE_OBJECTS;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnEnable()
    {
        //activate default mode
        ChangeMode((int)m_DefaultMode);
    }

    public void ChangeMode(int modes)
    {
        m_CurrentMode = (EDITMODES)modes;

        //activate proper mode in mapmanager
        if (m_CurrentMode == EDITMODES.DESTROY)
        {
            OpenMoveBuildingsBrush(false);
            OpenDeleteBrush(true);
        }
        else if (m_CurrentMode == EDITMODES.MOVE_OBJECTS)
        {
            OpenDeleteBrush(false);
            OpenMoveBuildingsBrush(true);
        }

        //make the button for the other mode dulled out
        //make button for current mode proper
        UpdateModeChangeUI();
    }

    //when players want to confirm their changes
    public void ConfirmChangeOpenPopup()
    {
        //check current mode
        //show correct popup to confirm
        //activate
        //TEMP
        if (m_CurrentMode == EDITMODES.DESTROY)
        {
            //remove the buildings
            MapManager.GetInstance().RemoveBuildingsFromMapUnderList();
        }
        else if (m_CurrentMode == EDITMODES.MOVE_OBJECTS)
        {
            if (MapManager.GetInstance().GetBuildingToMove() == null)
                return;
            if (!MapManager.GetInstance().CanPlaceBuildingToMove())
                return;
            MapManager.GetInstance().ConfirmRemovementOfBuilding();
        }
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
        for (int i = 0; i < m_ButtonModesImage.Length; ++i)
        {
            if (i != (int)m_CurrentMode)
                m_ButtonModesImage[i].color = m_GreyedOutColor;
            else
                m_ButtonModesImage[i].color = Color.white;
        }
    }

    #region Delete Mode
    public void OpenDeleteBrush(bool open)
    {
        MapManager.GetInstance().SetRemovalBrush(open);
    }
    #endregion
    #region Move Buildings Mode
    public void OpenMoveBuildingsBrush(bool open)
    {
        MapManager.GetInstance().SetMovementBrush(open);
    }
    #endregion
}
