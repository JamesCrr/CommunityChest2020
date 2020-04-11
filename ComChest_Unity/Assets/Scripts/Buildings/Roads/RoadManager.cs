﻿using System.Collections;
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


    Dictionary<int, RoadTypeList> m_RoadMap = new Dictionary<int, RoadTypeList>();
    Dictionary<Vector2Int, BaseBuildingsClass> m_RoadSpriteRendererMap = new Dictionary<Vector2Int, BaseBuildingsClass>();

    const int MAX_CHECKS = 2;
    Vector2Int UP_VECTOR = new Vector2Int(0, 1);
    Vector2Int DOWN_VECTOR = new Vector2Int(0, -1);
    Vector2Int RIGHT_VECTOR = new Vector2Int(1, 0);
    Vector2Int LEFT_VECTOR = new Vector2Int(-1, 0);



    public bool CheckMapAvailability(int mapIndex) //check if space is taken by the road
    {
        return m_RoadMap.ContainsKey(mapIndex);
    }

    public bool CheckMapAvailability(Vector2Int key) //check if space is taken by the road
    {
        return m_RoadSpriteRendererMap.ContainsKey(key);
    }

    public void PlaceRoads(Vector2Int key, int indexConverted, ref BaseBuildingsClass roadInfo)
    {
        m_RoadSpriteRendererMap.Add(key, roadInfo);

        //change sprites accordingly, do check
        CheckAndChangeRoadDirection(key);


        //TODO, STORE THE NEW DIRECTIONS ACCORDINGLY
        m_RoadMap.Add(indexConverted, RoadTypeList.NO_CONNECTION); 
    }

    public void CheckAndChangeRoadDirection(Vector2Int key, int loop = 0)
    {
        if (loop >= MAX_CHECKS)
            return;

        if (!m_RoadSpriteRendererMap.ContainsKey(key))
            Debug.LogError("ROAD KEY DOESNT EXIST, WAS NOT STORED PROPERLY !");

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
                Debug.Log("ZERO ROAD ARD");
                break;
            case 1:
                OneRoadAround(key, directionChecks, loop);
                Debug.Log("ONE ROAD ARD");
                break;
            case 2:
                TwoRoadsAround(key, directionChecks, loop);
                Debug.Log("TWO ROAD ARD");
                break;
            case 3:
                ThreeRoadsAround(key, directionChecks, loop);
                Debug.Log("Three ROAD ARD");

                break;
            case 4:
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

        //inverse and check if theres anything diagonal of the road
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

        //change the current one
        SetRoadSprite(key, currentRoadType);

        foreach (Vector2Int offset in offsets)
        {
            CheckAndChangeRoadDirection(new Vector2Int(key.x + offset.x, key.y + offset.y), loop + 1);
        }
    }

    public void SetRoadSprite(Vector2Int key, RoadTypeList type)
    {
        BaseBuildingsClass road = m_RoadSpriteRendererMap[key];
        road.SetSprite(BuildingDataBase.GetInstance().GetRoadSprite(type));
    }

    RoadTypeList CheckRoadDirection()
    {
        return RoadTypeList.NO_CONNECTION;
    }
}
