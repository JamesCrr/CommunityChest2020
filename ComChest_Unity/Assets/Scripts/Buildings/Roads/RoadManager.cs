using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadManager 
{
    enum Direction
    {
        UP,
        DOWN,
        LEFT,
        RIGHT,
        TOTAL_DIRECTIONS
    }

    enum DiagonalDirection
    {
        TOP_RIGHT,
        TOP_LEFT,
        BOTTOM_RIGHT,
        BOTTOM_LEFT,
        TOTAL_DIA_DIRECTIONS
    }

    Dictionary<int, RoadTypeList> m_RoadMap = new Dictionary<int, RoadTypeList>();
    Dictionary<Vector2Int, BaseBuildingsClass> m_RoadSpriteRendererMap = new Dictionary<Vector2Int, BaseBuildingsClass>();

    Vector2Int m_MapSize = Vector2Int.zero;
    Vector2Int m_MainRoadGridPos = Vector2Int.zero;

    const int MAX_CHECKS = 2;
    Vector2Int UP_VECTOR = new Vector2Int(0, 1);
    Vector2Int DOWN_VECTOR = new Vector2Int(0, -1);
    Vector2Int RIGHT_VECTOR = new Vector2Int(1, 0);
    Vector2Int LEFT_VECTOR = new Vector2Int(-1, 0);

    Vector2Int TOP_RIGHT_VECTOR = new Vector2Int(1, 1);
    Vector2Int TOP_LEFT_VECTOR = new Vector2Int(-1, 1);
    Vector2Int BOTTOM_RIGHT_VECTOR = new Vector2Int(1, -1);
    Vector2Int BOTTOM_LEFT_VECTOR = new Vector2Int(-1, -1);

    Queue<Vector2Int> m_BFSQueue = new Queue<Vector2Int>();

    //delegate functions for when player is in editing mode
    public delegate void OnRoadModifiedAndAdded(); //if players edited the road by adding to it
    public OnRoadModifiedAndAdded OnRoadModifiedAndAddedCallback;

    public delegate void OnRoadModifiedAndDeleated(); //if players edited the road by deleating some
    public OnRoadModifiedAndDeleated OnRoadModifiedAndDeleatedCallback;

    bool m_RoadAddedInSession = false;
    bool m_RoadRemovedInSession = false;

    public void Init(Vector2Int mapSize, Vector2Int mainRoadGridPos)
    {
        m_MapSize = mapSize;
        m_MainRoadGridPos = mainRoadGridPos;
    }

    int Convert2DToIntIndex(Vector2Int v2Index)
    {
        if (v2Index.x < 0 || v2Index.y < 0)
            return -1;
        return (v2Index.y * m_MapSize.x) + v2Index.x;
    }

    public bool CheckMapAvailability(int mapIndex) //check if space is taken by the road
    {
        return m_RoadMap.ContainsKey(mapIndex);
    }

    public bool CheckMapAvailability(Vector2Int key) //check if space is taken by the road
    {
        return m_RoadSpriteRendererMap.ContainsKey(key);
    }

    public void PlaceRoads(Vector2Int key, ref BaseBuildingsClass roadInfo)
    {
        if (m_RoadSpriteRendererMap.ContainsKey(key))
            return;

        //add them in uey
        m_RoadSpriteRendererMap.Add(key, roadInfo);

        int indexConverted = Convert2DToIntIndex(key);
        m_RoadMap.Add(indexConverted, RoadTypeList.NO_CONNECTION);

        //change sprites accordingly, do check
        CheckAndChangeRoadDirection(key);

        //road was added during session
        m_RoadAddedInSession = true;
    }

#region CheckAndChangeRoadDirection
    public void CheckAndChangeRoadDirection(Vector2Int key, int loop = 0)
    {
        if (loop >= MAX_CHECKS)
            return;

        if (!m_RoadSpriteRendererMap.ContainsKey(key))
            return;

        //check right left up down first
        bool[] directionChecks = new bool[(int)Direction.TOTAL_DIRECTIONS];
        for (int i = 0; i < (int)Direction.TOTAL_DIRECTIONS; ++i)
        {
            directionChecks[i] = false;
        }

        if (CheckMapAvailability(new Vector2Int(key.x, key.y + 1)))
            directionChecks[(int)Direction.UP] = true;
        if (CheckMapAvailability(new Vector2Int(key.x, key.y - 1)))
            directionChecks[(int)Direction.DOWN] = true;
        if (CheckMapAvailability(new Vector2Int(key.x + 1, key.y)))
            directionChecks[(int)Direction.RIGHT] = true;
        if (CheckMapAvailability(new Vector2Int(key.x - 1, key.y)))
            directionChecks[(int)Direction.LEFT] = true;

        ChangeRoadDirectionSprite(directionChecks, key, loop);
    }

    public void ChangeRoadDirectionSprite(bool[] directionChecks, Vector2Int key, int loop = 0)
    {
        int roadsArd = 0;
        foreach (bool dirCheck in directionChecks)
        {
            if (dirCheck)
                ++roadsArd;
        }

        switch (roadsArd)
        {
            case 0:
                ZeroRoadAround(key);
                break;
            case 1:
                OneRoadAround(key, directionChecks, loop);
                break;
            case 2:
                TwoRoadsAround(key, directionChecks, loop);
                break;
            case 3:
                ThreeRoadsAround(key, directionChecks, loop);
                break;
            case 4:
                FourRoadsAround(key, loop);
                break;
        }
    }

    public void ZeroRoadAround(Vector2Int key)
    {
        BaseBuildingsClass road = m_RoadSpriteRendererMap[key];
        road.SetSprite(BuildingDataBase.GetInstance().GetRoadSprite(RoadTypeList.NO_CONNECTION));
    }

    public void OneRoadAround(Vector2Int key, bool[] directionChecks, int loop) //only one road around the latest road placed
    {
        Vector2Int offset = Vector2Int.zero;
        RoadTypeList currentRoadType = RoadTypeList.NO_CONNECTION; //the dir for the road player is placing

        //get the offset on where the other road is
        if (directionChecks[(int)Direction.UP])
        {
            offset = UP_VECTOR;
            currentRoadType = RoadTypeList.U_ONLY_CONNECTION;
        }
        else if (directionChecks[(int)Direction.DOWN]) //other road on its left
        {
            offset = DOWN_VECTOR;
            currentRoadType = RoadTypeList.D_ONLY_CONNECTION;
        }
        else if (directionChecks[(int)Direction.RIGHT]) //other road is on its right
        {
            offset = RIGHT_VECTOR;
            currentRoadType = RoadTypeList.R_ONLY_CONNECTION;
        }
        else
        {
            offset = LEFT_VECTOR;
            currentRoadType = RoadTypeList.L_ONLY_CONNECTION;
        }

        //change the current one
        SetRoadSprite(key, currentRoadType);

        //check and change the surrounding roads accordingly
        CheckAndChangeRoadDirection(new Vector2Int(key.x + offset.x, key.y + offset.y), loop + 1);

        //TODO, IF NO CHANGE THAT MEANS EVERYTHING OKAY, EARLY RETURN DONT NEED RECRUSIVE

        return;
    }

    public void TwoRoadsAround(Vector2Int key, bool[] directionChecks, int loop)
    {
        RoadTypeList currentRoadType = RoadTypeList.NO_CONNECTION;
        bool diagonal = false;
        Vector2Int []offsets = new Vector2Int[3];
        for (int i =0; i < 3; ++i)
        {
            offsets[i] = Vector2Int.zero;
        }

        if (directionChecks[(int)Direction.UP])
        {
            offsets[0] = UP_VECTOR;

            if (directionChecks[(int)Direction.DOWN]) //one above, other below
            {
                currentRoadType = RoadTypeList.U_D_ONLY_CONNECTION;
                offsets[1] = DOWN_VECTOR;
            }
            else if (directionChecks[(int)Direction.RIGHT])
            {
                offsets[1] = RIGHT_VECTOR;
                currentRoadType = RoadTypeList.U_R_ONLY_CONNECTION;
            }
            else if (directionChecks[(int)Direction.LEFT])
            {
                currentRoadType = RoadTypeList.U_L_ONLY_CONNECTION;
                offsets[1] = LEFT_VECTOR;
            }
        }
        else if (directionChecks[(int)Direction.DOWN])
        {
            offsets[0] = DOWN_VECTOR;

            if (directionChecks[(int)Direction.RIGHT])
            {
                currentRoadType = RoadTypeList.D_R_ONLY_CONNECTION;
                offsets[1] = RIGHT_VECTOR;
            }
            else if (directionChecks[(int)Direction.LEFT])
            {
                currentRoadType = RoadTypeList.D_L_ONLY_CONNECTION;
                offsets[1] = LEFT_VECTOR;
            }
        }
        else //assume one on the right, other on the left
        {
            currentRoadType = RoadTypeList.R_L_ONLY_CONNECTION;
            offsets[0] = RIGHT_VECTOR;
            offsets[1] = LEFT_VECTOR;
        }

        //check if theres anything diagonal of the road
        offsets[2] = offsets[0] + offsets[1];
        if (CheckMapAvailability(key + offsets[2]) && offsets[2] != Vector2Int.zero)
        {
            diagonal = true;
            currentRoadType = currentRoadType + (RoadTypeList.U_R_DIA_ONLY_CONNECTION - RoadTypeList.U_R_ONLY_CONNECTION);
        }

        //change the current one
        SetRoadSprite(key, currentRoadType);

        CheckAndChangeRoadDirection(new Vector2Int(key.x + offsets[0].x, key.y + offsets[0].y), loop + 1);
        CheckAndChangeRoadDirection(new Vector2Int(key.x + offsets[1].x, key.y + offsets[1].y), loop + 1);

        if (diagonal)
            CheckAndChangeRoadDirection(new Vector2Int(key.x + offsets[2].x, key.y + offsets[2].y), loop + 1);
    }

    public void ThreeRoadsAround(Vector2Int key, bool[] directionChecks, int loop)
    {
        RoadTypeList currentRoadType = RoadTypeList.NO_CONNECTION;
        Vector2Int[] offsets = new Vector2Int[3];

        if (directionChecks[(int)Direction.UP])
        {
            offsets[0] = UP_VECTOR;

            if (directionChecks[(int)Direction.DOWN])
            {
                offsets[1] = DOWN_VECTOR;

                if (directionChecks[(int)Direction.RIGHT]) //up, down and right got tiles 
                {
                    currentRoadType = RoadTypeList.U_D_R_ONLY_CONNECTION;
                    offsets[2] = RIGHT_VECTOR;
                }
                else //up, down and left got tiles 
                {
                    currentRoadType = RoadTypeList.U_D_L_ONLY_CONNECTION;
                    offsets[2] = LEFT_VECTOR;
                }
            }
            else //assume its up, right and left
            {
                offsets[1] = RIGHT_VECTOR;
                offsets[2] = LEFT_VECTOR;
                currentRoadType = RoadTypeList.U_R_L_ONLY_CONNECTION;
            }
        }
        else //assume its down, right and left
        {
            offsets[0] = DOWN_VECTOR;
            offsets[1] = RIGHT_VECTOR;
            offsets[2] = LEFT_VECTOR;
            currentRoadType = RoadTypeList.D_R_L_ONLY_CONNECTION;
        }

        //check diagonal
        Vector2Int diagonalOffset = Vector2Int.zero;
        ThreeRoadsDiagonal(key, ref currentRoadType, loop);

        //change the current one
        SetRoadSprite(key, currentRoadType);

        foreach (Vector2Int offset in offsets)
        {
            CheckAndChangeRoadDirection(new Vector2Int(key.x + offset.x, key.y + offset.y), loop + 1);
        }
    }

    public void ThreeRoadsDiagonal(Vector2Int key, ref RoadTypeList currentRoadType, int loop)
    {
        Vector2Int diagonalOffset = Vector2Int.zero;

        //check possible combination of diagonals and see if theres any roads there
        switch (currentRoadType)
        {
            case RoadTypeList.U_D_L_ONLY_CONNECTION:
            {
                bool bottomLeft, topLeft;
                bottomLeft = topLeft = false;

                diagonalOffset = new Vector2Int(-1, -1); //check bottom left first
                if (CheckMapAvailability(diagonalOffset + key))
                {
                    currentRoadType = RoadTypeList.U_D_L_DIA_BL_ONLY_CONNECTION;
                    bottomLeft = true;
                    CheckAndChangeRoadDirection(key + diagonalOffset, loop + 1);
                }

                diagonalOffset = new Vector2Int(-1, 1); //check top left
                if (CheckMapAvailability(diagonalOffset + key))
                {
                    currentRoadType = RoadTypeList.U_D_L_DIA_TL_ONLY_CONNECTION;
                    topLeft = true;
                    CheckAndChangeRoadDirection(key + diagonalOffset, loop + 1);
                }

                if (topLeft && bottomLeft) //if diagonally both sides have
                   currentRoadType = RoadTypeList.U_D_L_DIA_BL_TL_ONLY_CONNECTION;
            }
            break;
            case RoadTypeList.U_D_R_ONLY_CONNECTION:
            {
                bool bottomRight, topRight;
                bottomRight = topRight = false;

                diagonalOffset = new Vector2Int(1, -1); //check bottom right first
                if (CheckMapAvailability(diagonalOffset + key))
                {
                    currentRoadType = RoadTypeList.U_D_R_DIA_BR_ONLY_CONNECTION;
                    bottomRight = true;
                    CheckAndChangeRoadDirection(key + diagonalOffset, loop + 1);
                }

                diagonalOffset = new Vector2Int(1, 1); //check top right
                if (CheckMapAvailability(diagonalOffset + key))
                {
                    currentRoadType = RoadTypeList.U_D_R_DIA_TR_ONLY_CONNECTION;
                    topRight = true;
                    CheckAndChangeRoadDirection(key + diagonalOffset, loop + 1);
                }

                if (topRight && bottomRight) //if diagonally both sides have
                   currentRoadType = RoadTypeList.U_D_R_DIA_BR_TR_ONLY_CONNECTION;
            }
            break;
            case RoadTypeList.U_R_L_ONLY_CONNECTION:
            {
                bool topLeft, topRight;
                topLeft = topRight = false;

                diagonalOffset = new Vector2Int(1, 1); //check top right first
                if (CheckMapAvailability(diagonalOffset + key))
                {
                    currentRoadType = RoadTypeList.U_R_L_DIA_TR_ONLY_CONNECTION;
                    topRight = true;
                    CheckAndChangeRoadDirection(key + diagonalOffset, loop + 1);
                }

                diagonalOffset = new Vector2Int(-1, 1); //check top left
                if (CheckMapAvailability(diagonalOffset + key))
                {
                    currentRoadType = RoadTypeList.U_R_L_DIA_TL_ONLY_CONNECTION;
                    topLeft = true;
                    CheckAndChangeRoadDirection(key + diagonalOffset, loop + 1);
                }

                if (topRight && topLeft) //if diagonally both sides have
                    currentRoadType = RoadTypeList.U_R_L_DIA_TR_TL_ONLY_CONNECTION;
            }
            break;
            case RoadTypeList.D_R_L_ONLY_CONNECTION:
            {
                bool bottomLeft, bottomRight;
                bottomLeft = bottomRight = false;

                diagonalOffset = new Vector2Int(1, -1); //check bottom right first
                if (CheckMapAvailability(diagonalOffset + key))
                {
                    currentRoadType = RoadTypeList.D_R_L_DIA_BR_ONLY_CONNECTION;
                    bottomRight = true;
                    CheckAndChangeRoadDirection(key + diagonalOffset, loop + 1); //change the corners accordingly
                }

                diagonalOffset = new Vector2Int(-1, -1); //check bottom left
                if (CheckMapAvailability(diagonalOffset + key))
                {
                    currentRoadType = RoadTypeList.D_R_L_DIA_BL_ONLY_CONNECTION;
                    bottomLeft = true;
                    CheckAndChangeRoadDirection(key + diagonalOffset, loop + 1);
                }

                if (bottomLeft && bottomRight) //if diagonally both sides have
                    currentRoadType = RoadTypeList.D_R_L_DIA_BR_BL_ONLY_CONNECTION;
            }
            break;
        }
    }

    public void FourRoadsAround(Vector2Int key, int loop)
    {
        //check diagonal sides
        bool[] diagonalDirectionChecks = new bool[(int)DiagonalDirection.TOTAL_DIA_DIRECTIONS];
        for (int i = 0; i < (int)Direction.TOTAL_DIRECTIONS; ++i)
        {
            diagonalDirectionChecks[i] = false;
        }

        if (CheckMapAvailability(new Vector2Int(key.x + 1, key.y + 1))) //check top right
            diagonalDirectionChecks[(int)DiagonalDirection.TOP_RIGHT] = true;
        if (CheckMapAvailability(new Vector2Int(key.x - 1, key.y + 1))) //check top left
            diagonalDirectionChecks[(int)DiagonalDirection.TOP_LEFT] = true;
        if (CheckMapAvailability(new Vector2Int(key.x + 1, key.y - 1))) //check bottom right
            diagonalDirectionChecks[(int)DiagonalDirection.BOTTOM_RIGHT] = true;
        if (CheckMapAvailability(new Vector2Int(key.x - 1, key.y - 1))) //check bottom left
            diagonalDirectionChecks[(int)DiagonalDirection.BOTTOM_LEFT] = true;

        int diagonalRoadCounter = 0;
        foreach (bool diagonalCheck in diagonalDirectionChecks)
        {
            if (diagonalCheck)
                ++diagonalRoadCounter;
        }

        //change the sprite accordingly to the number of diagonal roads around it
        switch (diagonalRoadCounter)
        {
            case 0:
                ZeroDiagonalAround(key);
                break;
            case 1:
                OneDiagonalAround(key, diagonalDirectionChecks, loop);
                break;
            case 2:
                TwoDiagonalAround(key, diagonalDirectionChecks, loop);
                break;
            case 3:
                ThreeDiagonalAround(key, diagonalDirectionChecks, loop);
                break;
            case 4:
                FourDiagonalAround(key, loop);
                break;
        }

        //check the top, bottom, left and right roads
        CheckAndChangeRoadDirection(key + UP_VECTOR, loop + 1);
        CheckAndChangeRoadDirection(key + DOWN_VECTOR, loop + 1);
        CheckAndChangeRoadDirection(key + RIGHT_VECTOR, loop + 1);
        CheckAndChangeRoadDirection(key + LEFT_VECTOR, loop + 1);
    }

    #region FourRoadDiagonal
    public void ZeroDiagonalAround(Vector2Int key)
    {
        SetRoadSprite(key, RoadTypeList.NO_DIA_CONNECTION);
    }

    public void OneDiagonalAround(Vector2Int key, bool[] diaDirectionCheck, int loop)
    {
        RoadTypeList currentRoadType = RoadTypeList.ALL_DIA_CONNECTION;
        Vector2Int offset = Vector2Int.zero;

        if (diaDirectionCheck[(int)DiagonalDirection.TOP_LEFT])
        {
            currentRoadType = RoadTypeList.DIA_TL_ONLY_CONNECTION;
            offset = TOP_LEFT_VECTOR;
        }
        else if (diaDirectionCheck[(int)DiagonalDirection.TOP_RIGHT])
        {
            currentRoadType = RoadTypeList.DIA_TR_ONLY_CONNECTION;
            offset = TOP_RIGHT_VECTOR;
        }
        else if (diaDirectionCheck[(int)DiagonalDirection.BOTTOM_LEFT])
        {
            currentRoadType = RoadTypeList.DIA_BL_ONLY_CONNECTION;
            offset = BOTTOM_LEFT_VECTOR;
        }
        else //bottom right
        {
            currentRoadType = RoadTypeList.DIA_BR_ONLY_CONNECTION;
            offset = BOTTOM_RIGHT_VECTOR;
        }

        SetRoadSprite(key, currentRoadType);
        CheckAndChangeRoadDirection(key + offset, loop + 1);
    }

    public void TwoDiagonalAround(Vector2Int key, bool[] diaDirectionCheck, int loop)
    {
        RoadTypeList currentRoadType = RoadTypeList.ALL_DIA_CONNECTION;
        Vector2Int[] offsets = new Vector2Int[2];

        if (diaDirectionCheck[(int)DiagonalDirection.TOP_LEFT])
        {
            offsets[0] = TOP_LEFT_VECTOR;

            if (diaDirectionCheck[(int)DiagonalDirection.TOP_RIGHT]) //top left and top right
            {
                offsets[1] = TOP_RIGHT_VECTOR;
                currentRoadType = RoadTypeList.DIA_TL_TR_ONLY_CONNECTION;
            }
            else if (diaDirectionCheck[(int)DiagonalDirection.BOTTOM_LEFT]) //top left and bottom left
            {
                offsets[1] = BOTTOM_LEFT_VECTOR;
                currentRoadType = RoadTypeList.DIA_TL_BL_ONLY_CONNECTION;
            }
            else //assume its top left and bottom right
            {
                offsets[1] = BOTTOM_RIGHT_VECTOR;
                currentRoadType = RoadTypeList.DIA_TL_BR_ONLY_CONNECTION;
            }
        }
        else if (diaDirectionCheck[(int)DiagonalDirection.TOP_RIGHT])
        {
            offsets[0] = TOP_RIGHT_VECTOR;

            if (diaDirectionCheck[(int)DiagonalDirection.BOTTOM_LEFT]) //top right and bottom left
            {
                offsets[1] = BOTTOM_LEFT_VECTOR;
                currentRoadType = RoadTypeList.DIA_TR_BL_ONLY_CONNECTION;
            }
            else //assume its top right and bottom right
            {
                offsets[1] = BOTTOM_RIGHT_VECTOR;
                currentRoadType = RoadTypeList.DIA_TR_BR_ONLY_CONNECTION;
            }
        }
        else //assume its bottom left and bottom right
        {
            offsets[0] = BOTTOM_RIGHT_VECTOR;
            offsets[1] = BOTTOM_LEFT_VECTOR;

            currentRoadType = RoadTypeList.DIA_BL_BR_ONLY_CONNECTION;
        }

        SetRoadSprite(key, currentRoadType);

        //check the 2 diagonal roads and update accordingly
        foreach (Vector2Int offset in offsets)
            CheckAndChangeRoadDirection(key + offset, loop + 1);
    }

    public void ThreeDiagonalAround(Vector2Int key, bool[] diaDirectionCheck, int loop)
    {
        RoadTypeList currentRoadType = RoadTypeList.ALL_DIA_CONNECTION;
        Vector2Int[] offsets = new Vector2Int[3];

        if (diaDirectionCheck[(int)DiagonalDirection.TOP_LEFT])
        {
            offsets[0] = TOP_LEFT_VECTOR;

            if (diaDirectionCheck[(int)DiagonalDirection.TOP_RIGHT])
            {
                offsets[1] = TOP_RIGHT_VECTOR;

                if (diaDirectionCheck[(int)DiagonalDirection.BOTTOM_RIGHT])
                {
                    offsets[2] = BOTTOM_RIGHT_VECTOR;
                    currentRoadType = RoadTypeList.DIA_TL_TR_BR_ONLY_CONNECTION;
                }
                else //assume its bottom left
                {
                    offsets[2] = BOTTOM_LEFT_VECTOR;
                    currentRoadType = RoadTypeList.DIA_TL_TR_BL_ONLY_CONNECTION;
                }
            }
            else //assume diagonally on top left, bottom left, bottom right theres roads
            {
                offsets[1] = BOTTOM_LEFT_VECTOR;
                offsets[2] = BOTTOM_RIGHT_VECTOR;
                currentRoadType = RoadTypeList.DIA_TL_BL_BR_ONLY_CONNECTION;
            }
        }
        else //assume its top right, bottom left, bottom right
        {
            offsets[0] = TOP_RIGHT_VECTOR;
            offsets[1] = BOTTOM_LEFT_VECTOR;
            offsets[2] = BOTTOM_RIGHT_VECTOR;

            currentRoadType = RoadTypeList.DIA_TR_BL_BR_ONLY_CONNECTION;
        }

        SetRoadSprite(key, currentRoadType);

        //check the 3 diagonal roads and update accordingly
        foreach (Vector2Int offset in offsets)
            CheckAndChangeRoadDirection(key + offset, loop + 1);
    }

    public void FourDiagonalAround(Vector2Int key, int loop)
    {
        SetRoadSprite(key, RoadTypeList.ALL_DIA_CONNECTION);

        //check the 4 diagonal roads and update accordingly
        CheckAndChangeRoadDirection(key + TOP_RIGHT_VECTOR, loop + 1);
        CheckAndChangeRoadDirection(key + TOP_LEFT_VECTOR, loop + 1);
        CheckAndChangeRoadDirection(key + BOTTOM_LEFT_VECTOR, loop + 1);
        CheckAndChangeRoadDirection(key + BOTTOM_RIGHT_VECTOR, loop + 1);
    }
    #endregion

#endregion

    public void SetRoadSprite(Vector2Int key, RoadTypeList type)
    {
        BaseBuildingsClass road = m_RoadSpriteRendererMap[key];
        road.SetSprite(BuildingDataBase.GetInstance().GetRoadSprite(type));

        //change and store the road type
        int indexConverted = Convert2DToIntIndex(key);
        if (m_RoadMap.ContainsKey(indexConverted))
            m_RoadMap[indexConverted] = type;
    }

    public void RemoveRoads(Vector2Int key)
    {
        if (!CheckMapAvailability(key)) //if key doesnt exist
            return;

        m_RoadSpriteRendererMap.Remove(key);

        //remove the type of road from the list
        int indexConverted = Convert2DToIntIndex(key);
        if (m_RoadMap.ContainsKey(indexConverted))
            m_RoadMap.Remove(indexConverted);

        //check if theres any road around it and update them accordingly
        CheckAndChangeRoadDirection(key + UP_VECTOR);
        CheckAndChangeRoadDirection(key + DOWN_VECTOR);
        CheckAndChangeRoadDirection(key + RIGHT_VECTOR);
        CheckAndChangeRoadDirection(key + LEFT_VECTOR);

        //road was removed during session
        m_RoadRemovedInSession = true;
    }

#region ForEditingMode
    public void CheckAndInvokeAddingOfRoadsCallback()
    {
        //check if roads were added during the adding edit session
        if (m_RoadAddedInSession)
        {
            if (OnRoadModifiedAndAddedCallback != null)
                OnRoadModifiedAndAddedCallback.Invoke();
        }

        m_RoadAddedInSession = false;
    }

    public void CheckAndInvokeRemovalOfRoadsCallback()
    {
        //check if roads are removed during the removal Edit ssession
        if (m_RoadRemovedInSession)
        {
            if (OnRoadModifiedAndDeleatedCallback != null)
                OnRoadModifiedAndDeleatedCallback.Invoke();
        }

        m_RoadRemovedInSession = false;
    }
#endregion

    #region SavingAndLoading
    public RoadsOnMap[] GetSavedRoads()
    {
        RoadsOnMap[] saveRoadsData = new RoadsOnMap[m_RoadSpriteRendererMap.Count];

        int index = 0;
        foreach (KeyValuePair<Vector2Int, BaseBuildingsClass> road in m_RoadSpriteRendererMap)
        {
            BaseBuildingsClass baseRoad = road.Value;
            if (baseRoad == null)
                continue;

            saveRoadsData[index] = new RoadsOnMap();
            saveRoadsData[index].worldPosX = baseRoad.transform.position.x;
            saveRoadsData[index].worldPosY = baseRoad.transform.position.y;

            //get and store the type
            int indexConverted = Convert2DToIntIndex(road.Key);
            if (m_RoadMap.ContainsKey(indexConverted))
            {
                saveRoadsData[index].roadType = (int)m_RoadMap[indexConverted];
            }

            ++index;
        }

        return saveRoadsData;
    }

    public void StoreLoadedInRoads(Vector2Int key, BaseBuildingsClass roadInfo, RoadTypeList roadType)
    {
        if (CheckMapAvailability(key)) //if key exists, do early return
            return;

        m_RoadSpriteRendererMap.Add(key, roadInfo);

        int indexConverted = Convert2DToIntIndex(key);
        m_RoadMap.Add(indexConverted, roadType);
    }
    #endregion

#region pathFinding
    public bool CheckRoadConnection(Vector2Int startPt, Vector2Int endPt)
    {
        //check if end and start point exists first
        if (!CheckMapAvailability(startPt) || !CheckMapAvailability(endPt))
            return false;

        m_BFSQueue.Enqueue(startPt);
        Dictionary<Vector2Int, bool> visitedNodes = new Dictionary<Vector2Int, bool>();

        //BFS to check
        while (m_BFSQueue.Count != 0) //while queue is not empty
        {
            Vector2Int currNode = m_BFSQueue.Peek();
            m_BFSQueue.Dequeue(); //pop out the current vector

            //'explore' the current node
            if (currNode == endPt)
            {
                return true; //reached the destination we want, stop as path exists
            }

            //check the other nodes
            Vector2Int nextNode = currNode + UP_VECTOR;
            if (nextNode.y < m_MapSize.y) //within map size
            {
                if (CheckMapAvailability(nextNode) && !visitedNodes.ContainsKey(nextNode)) //if road exists and not visited yet
                {
                    m_BFSQueue.Enqueue(nextNode);
                    visitedNodes.Add(nextNode, true);
                }
            }

            nextNode = currNode + DOWN_VECTOR;
            if (nextNode.y > 0)
            {
                if (CheckMapAvailability(nextNode) && !visitedNodes.ContainsKey(nextNode))
                {
                    m_BFSQueue.Enqueue(nextNode);
                    visitedNodes.Add(nextNode, true);
                }
            }

            nextNode = currNode + RIGHT_VECTOR;
            if (nextNode.x < m_MapSize.x)
            {
                if (CheckMapAvailability(nextNode) && !visitedNodes.ContainsKey(nextNode))
                {
                    m_BFSQueue.Enqueue(nextNode);
                    visitedNodes.Add(nextNode, true);
                }
            }

            nextNode = currNode + LEFT_VECTOR;
            if (nextNode.x > 0)
            {
                if (CheckMapAvailability(nextNode) && !visitedNodes.ContainsKey(nextNode))
                {
                    m_BFSQueue.Enqueue(nextNode);
                    visitedNodes.Add(nextNode, true);
                }
            }
        }

        return false;
    }

    public bool CheckRoadConnectionToMainRoad(Vector2Int startPt)
    {
        return CheckRoadConnection(startPt, m_MainRoadGridPos);
    }
#endregion
}
