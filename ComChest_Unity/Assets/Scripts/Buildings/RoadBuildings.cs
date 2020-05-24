using UnityEngine;

public class RoadBuildings : BaseBuildingsClass
{
    bool m_RoadConnected = false; //checks if the building road is connected
    Vector2Int m_RoadGridPos = Vector2Int.zero;

    void Start()
    {
        //register the invoke function from player input to see if added or removed
        RoadManager roadManager = MapManager.GetInstance().GetRoadManager();
        if (roadManager != null)
        {
            roadManager.OnRoadModifiedAndAddedCallback += CheckRoadConnectionAfterRoadAdded;
            roadManager.OnRoadModifiedAndDeleatedCallback += CheckRoadConnectionAfterRoadRemoval;
        }

        CheckRoadConnectionAfterRoadAdded(); //check if already connected to main road
    }

    public override void SetNewBuildingType(BuildingData buildingData)
    {
        base.SetNewBuildingType(buildingData);

        //calculate where the grid position of the road from the offset 
        m_RoadGridPos = MapManager.GetInstance().GetWorldPosToCellPos(GetBottomLeftGridPosition()) + buildingData.GetRoadOffset;

        CheckRoadConnectionAfterRoadAdded(); //check if there are any roads connected already

        if (NPCManager.Instance != null)
            NPCManager.Instance.AddBuildingEntrance(m_RoadGridPos); //add the entrance to the road
    }

    public override void BuildingRemoved()
    {
        base.BuildingRemoved();

        if (NPCManager.Instance != null)
            NPCManager.Instance.RemoveBuildingEntrance(m_RoadGridPos); //remove the entrance to the road
    }

    public override void BuildingMoved()
    {
        base.BuildingMoved();

        //remove the prev entrance, put the new one
        if (NPCManager.Instance != null)
        {
            NPCManager.Instance.RemoveBuildingEntrance(m_RoadGridPos);

            //check and update road connection
            m_RoadGridPos = MapManager.GetInstance().GetWorldPosToCellPos(GetBottomLeftGridPosition()) + m_RoadOffset;
            m_RoadConnected = false;
            CheckRoadConnectionAfterRoadAdded(); //check if there are any roads connected already
            NPCManager.Instance.AddBuildingEntrance(m_RoadGridPos); //add the entrance to the road
        }
    }

    public void CheckRoadConnectionAfterRoadAdded()
    {
        if (m_RoadConnected)
            return;

        bool roadConnected = MapManager.GetInstance().CheckRoadConnectionToMainRoad(m_RoadGridPos);

        if (!roadConnected) //road is not connected
        {
            UpdateRoadDisconnectedUI();
            return;
        }

        //road is connected
        m_RoadConnected = true;
        UpdateRoadConnectedUI();
    }

    public void CheckRoadConnectionAfterRoadRemoval()
    {
        if (!m_RoadConnected) //if road is never connected ignore
            return;

        //BFS and check if the connection broke
        bool roadConnected = MapManager.GetInstance().CheckRoadConnectionToMainRoad(m_RoadGridPos);

        if (!roadConnected)
        {
            UpdateRoadDisconnectedUI();
            m_RoadConnected = false;
        }
    }

    public void UpdateRoadDisconnectedUI()
    {
        //TODO, TEMP CHANGE SPRITE TO black
        SetSpriteObjectColor(Color.grey);
    }

    public void UpdateRoadConnectedUI()
    {
        //TODO, TEMP CHANGE SPRITE TO NORMAL COLOR
        SetSpriteObjectColor(Color.white);
    }
}
