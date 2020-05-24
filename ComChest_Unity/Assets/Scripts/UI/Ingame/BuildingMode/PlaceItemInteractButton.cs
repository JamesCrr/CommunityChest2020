public class PlaceItemInteractButton : InteractableObjBase
{
    //place building when clicked
    public override void OnInteract()
    {
        if (MapManager.GetInstance() == null)
            return;

        // Place the Template Building
        if (MapManager.GetInstance().GetMovementBrushActive())
        {
            if (MapManager.GetInstance().GetBuildingToMove() == null)
                return;
            if (!MapManager.GetInstance().CanPlaceBuildingToMove())
                return;

            MapManager.GetInstance().ConfirmRemovementOfBuilding();
            IngameUIManager.instance.BuildModeUIClose(true);
        }
        else if (MapManager.GetInstance().GetPlacementBrushActive())
        {
            // Can place there?
            if (!MapManager.GetInstance().CanPlaceTemplateBuilding())
                return;

            MapManager.GetInstance().PlaceTemplateBuilding();
            IngameUIManager.instance.SetInGameMenuActive(true);
            //close the UI for placing the items and stuff
            IngameUIManager.instance.PlayerInBuildModeUI(false);
        }
    }
}
