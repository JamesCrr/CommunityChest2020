public class CancelInteract : InteractableObjBase
{
    public override void OnInteract()
    {
        //close the UI for placing the items and stuff
        if (MapManager.GetInstance().GetMovementBrushActive())
        {
            //stop the movement of the building
            MapManager.GetInstance().CancelRemovementOfBuilding();
            IngameUIManager.instance.BuildModeUIClose(true);
        }
        else if (MapManager.GetInstance().GetPlacementBrushActive())
        {
            IngameUIManager.instance.PlayerInBuildModeUI(false);
            IngameUIManager.instance.SetInGameMenuActive(true);
        }
    }
}
