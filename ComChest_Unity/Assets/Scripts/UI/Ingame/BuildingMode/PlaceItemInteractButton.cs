public class PlaceItemInteractButton : InteractableObjBase
{
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
