using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : MonoBehaviour
{
    [SerializeField]
    [Tooltip("The max distance it needs to be from the grid tile center to be considered it reached that grid")]
    float m_OffsetFromMiddle = 0.001f;

    float m_Speed = 1.0f;
    Vector2 m_Dir = Vector2Int.zero;

    //for finding the next route to go to
    Dictionary<Vector2Int, bool> m_VistedRoads = new Dictionary<Vector2Int, bool>();
    List<Vector2Int> m_RoadsQueue = new List<Vector2Int>();
    Vector2Int m_CurrentTile = Vector2Int.zero; //the current tile position theyre in
    Vector2Int m_NextTile = Vector2Int.zero; //the next tile position they need to go
    

    public void OnEnable()
    {
        Init(1.0f, new Vector2Int(9, 4));
    }

    public void Init(float speed, Vector2Int currentTilePos)
    {
        m_Speed = speed;
        m_CurrentTile = currentTilePos;

        //register the transform position based on current tile pos
        transform.position = MapManager.GetInstance().GetCellCentrePosToWorld(currentTilePos);

        m_Dir = Vector2Int.zero;
        m_VistedRoads.Clear();

        DFSNextRoad(m_CurrentTile); //get the next tile to go to
    }

    // Update is called once per frame
    void Update()
    {
        //reach the block then DFS next block
        if (m_NextTile == MapManager.GetInstance().GetWorldPosToCellPos(transform.position))
        {
            //need to check if NPC at centre of tile, since worldpostocellpos checks bottom left of grid tile instead of centre
            Vector2 nextTileCentrePos = MapManager.GetInstance().GetCellCentrePosToWorld(m_NextTile);
            if (Vector2.SqrMagnitude(nextTileCentrePos - (Vector2)transform.position) <= m_OffsetFromMiddle + Mathf.Epsilon)
            {
                m_CurrentTile = m_NextTile;
                transform.position = MapManager.GetInstance().GetCellCentrePosToWorld(m_CurrentTile); //snap to grid

                DFSNextRoad(m_CurrentTile);
            }
        }
    }

    private void FixedUpdate()
    {
        //update position
        transform.position = (Vector2)transform.position + m_Dir * m_Speed * Time.fixedDeltaTime;
    }

    void DFSNextRoad(Vector2Int currTile) //get the next block to go
    {
        RoadManager roadManager = MapManager.GetInstance().GetRoadManager();
        if (roadManager == null)
            return;

        List<Vector2Int> neighbourRoads = new List<Vector2Int>();

        //check up first, if never visited and theres a tile there add it
        Vector2Int newGridoffsetPos = currTile + new Vector2Int(0, 1);
        if (roadManager.CheckMapAvailability(newGridoffsetPos) && !m_VistedRoads.ContainsKey(newGridoffsetPos)) 
        {
            neighbourRoads.Add(newGridoffsetPos);
        }

        //check down
        newGridoffsetPos = currTile + new Vector2Int(0, -1);
        if (roadManager.CheckMapAvailability(newGridoffsetPos) && !m_VistedRoads.ContainsKey(newGridoffsetPos))
        {
            neighbourRoads.Add(newGridoffsetPos);
        }

        //check right
        newGridoffsetPos = currTile + new Vector2Int(1, 0);
        if (roadManager.CheckMapAvailability(newGridoffsetPos) && !m_VistedRoads.ContainsKey(newGridoffsetPos))
        {
            neighbourRoads.Add(newGridoffsetPos);
        }

        //check left
        newGridoffsetPos = currTile + new Vector2Int(-1, 0);
        if (roadManager.CheckMapAvailability(newGridoffsetPos) && !m_VistedRoads.ContainsKey(newGridoffsetPos))
        {
            neighbourRoads.Add(newGridoffsetPos);
        }

        if (neighbourRoads.Count == 0) //if theres nothing
        {
            //TODO
            //check if theres anything around, if no start despawning
            //if (m_VistedRoads.Count == 0) 
            //{
            //    m_Dir = Vector2Int.zero;
            //    return;
            //}

            //look into the queue to see if theres any other previous roads
            //if queue is empty, reset the position to NPC current tile and reset the visited list
            if (m_RoadsQueue.Count == 0)
            {
                m_VistedRoads.Clear();
                DFSNextRoad(m_CurrentTile);
            }
            else
            {
                int lastAddedIndex = m_RoadsQueue.Count - 1;
                Vector2Int prev = m_RoadsQueue[lastAddedIndex];
                m_RoadsQueue.RemoveAt(lastAddedIndex); //remove the last element added

                InitNextTile(prev);
            }
        }
        else //go to next neighbouring tile
        {
            m_RoadsQueue.Add(m_CurrentTile);

            int randomNeighbourIndex = Random.Range(0, neighbourRoads.Count - 1);
            InitNextTile(neighbourRoads[randomNeighbourIndex]);
        }
    }

    public void InitNextTile(Vector2Int nextTile)
    {
        m_NextTile = nextTile;

        if (!m_VistedRoads.ContainsKey(m_CurrentTile))
            m_VistedRoads.Add(m_CurrentTile, true); //road is 'visited'

        m_Dir = m_NextTile - m_CurrentTile;
        m_Dir.Normalize();

        //TODO edit the animation here?
    }
}
