public class CancelInteract : InteractableObjBase
{
    public override void OnInteract()
    {
        //close the UI for placing the items and stuff
        IngameUIManager.instance.PlayerInBuildModeUI(false);
    }
}
