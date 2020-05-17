using UnityEngine;

public class PlaceItemInteractButton : InteractableObjBase
{
    [SerializeField] Transform m_MainParent;

    public void OnDisable()
    {
        //go to the original parent
        if (m_MainParent != null)
            transform.SetParent(m_MainParent);
    }

    //place building when clicked
    public override void OnInteract()
    {
        if (MapManager.GetInstance() == null)
            return;

        // Can place there?
        if (!MapManager.GetInstance().CanPlaceTemplateBuilding())
            return;

        // Place the Template Building
        MapManager.GetInstance().PlaceTemplateBuilding();

        //close the UI for placing the items and stuff
        IngameUIManager.instance.PlayerInBuildModeUI(false);
    }
}
