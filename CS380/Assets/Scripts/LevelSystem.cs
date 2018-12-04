﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum DummyEnum
{
  HazardA,
  HazardB,
  HazardC
}

public class LevelSystem : MonoBehaviour {
  [Header("Necessary Objects")]
  public GameObject StartRoom;
  public GameObject GoalRoom;
  public GameObject[] Rooms;
  public GameObject Wall;

  [Header("Room Variables")]
  public Vector3 m_RoomBaseSize;

  [Header("System Variables")]
  public bool m_GenerateOnStart;
  public int m_StartHeight;
  public int m_StartWidth;

  [Header("System Restrictions")]
  public int m_MinWidth;
  public int m_MinHeight;
  public int m_MinGoalOffset;

  [Header("Debug")]
  public List<Vector3> m_RoomPositions;
  public List<GameObject> m_SpawnedRooms;

  //Privates
  List<GameObject> p_SortedRooms;   //Possible Usage to sort by heuristic cost


	// Use this for initialization
	void Start () {
    p_SortedRooms = new List<GameObject>();
    m_SpawnedRooms = new List<GameObject>();

    if (m_GenerateOnStart)
      GenerateRooms(m_StartHeight, m_StartWidth);
	}
	
	// Update is called once per frame
	void Update () {
    }

  //Returns success or failure
  public bool GenerateRooms(int height, int width, bool random = false)
  {
    Debug.Log("[LVGEN] Generating Rooms");

    if (SetSeed != true)
    {
       seed = (int)(Random.value * 100000);
    }
    Random.InitState(seed++);

    //Reset Lists
    m_RoomPositions.Clear();
    for (int i = 0; i < m_SpawnedRooms.Count; ++i)
      Destroy(m_SpawnedRooms[i]);
    m_SpawnedRooms.Clear();

    //Check that given parameters are valid
    if (height < m_MinHeight)
      height = m_MinHeight;
    if (width < m_MinWidth)
      width = m_MinWidth;

    //Sort Rooms
    SortRooms();

    //Find lowest left corner
    Vector3 anchor = Vector3.zero;
    anchor.x = -m_RoomBaseSize.x * width;
    anchor.z = -m_RoomBaseSize.z * height;

    //Set a start and goal
    Vector2 startIndex = 
      new Vector2(Random.Range(0, width - 1), Random.Range(0, height - 1));
    Vector2 goalIndex =
      new Vector2(Random.Range(0, width - 1), Random.Range(0, height - 1));
    int goalOffset =
      IndexDistance(startIndex, goalIndex);
    
    //Make sure start and goal are a minimum # of rooms apart
    while (goalOffset < m_MinGoalOffset)
    {
      goalIndex =
        new Vector2(Random.Range(0, width - 1), Random.Range(0, height - 1));
      goalOffset =
        IndexDistance(startIndex, goalIndex);
    }

    //Find maximum index distance between the goal and any index
    float maxIndexDistance = 0;
    if (Mathf.Abs((int)goalIndex.x - width) > Mathf.Abs((int)goalIndex.x - 0))
      maxIndexDistance += Mathf.Abs((int)goalIndex.x - width);
    else
      maxIndexDistance += Mathf.Abs((int)goalIndex.x - 0);
    if (Mathf.Abs((int)goalIndex.y - height) > Mathf.Abs((int)goalIndex.y - 0))
      maxIndexDistance += Mathf.Abs((int)goalIndex.y - height);
    else
      maxIndexDistance += Mathf.Abs((int)goalIndex.x - 0);

    Debug.Log("[LVGEN] Max Index Distance from goal is: " + maxIndexDistance);
    
    //Create rooms
    for (int i = 0; i < height; ++i)
    {
      for (int j = 0; j < width; ++j)
      {
        GameObject spawnedRoom;
        if ((j == 0 && i == 0) || (j == width - 1 && i == height - 1))//if it's the spawn or the goal then make it a default room
        {
          spawnedRoom = Instantiate(Rooms[0]);
        }
        else//if it's neither the goal nore the spawn then select another room
        {
          spawnedRoom = Instantiate(Rooms[Random.Range(0, Rooms.Length)]);
        }
        Vector3 spawnPos = anchor;
        spawnPos.x += m_RoomBaseSize.x * j;
        spawnPos.z += m_RoomBaseSize.z * i;
        spawnedRoom.transform.position = spawnPos;

        if(spawn == true)
        {
           //if it is the startroom, then move the player and path agent there
           stats.gameObject.transform.position = spawnPos;
           stats.spawn = spawnedRoom;
           path.gameObject.GetComponent<NavMeshAgent>().Warp(spawnPos);
        }
        else if(goal == true)
        {
           path.goal = spawnPos;
        }

        //Push into lists
        m_RoomPositions.Add(spawnPos);
        m_SpawnedRooms.Add(spawnedRoom);
      }
    }


    //Create border walls
    float zRot = 90.0f;
    anchor.y += m_RoomBaseSize.y * .5f;
    for (int i = 0; i < height; ++i)
    {
      GameObject spawnedWall;
      Vector3 spawnPos = anchor;
      spawnPos.x -= m_RoomBaseSize.x * .5f;
      spawnPos.z += m_RoomBaseSize.z * i;
      spawnedWall = Instantiate(Wall);
      spawnedWall.transform.position = spawnPos;
      spawnedWall.transform.eulerAngles =
        new Vector3(spawnedWall.transform.eulerAngles.x,
                    spawnedWall.transform.eulerAngles.y,
                    zRot);

      spawnPos = anchor;
      spawnPos.x += m_RoomBaseSize.x * width;
      spawnPos.x -= m_RoomBaseSize.x * .5f;
      spawnPos.z += m_RoomBaseSize.z * i;
      spawnedWall = Instantiate(Wall);
      spawnedWall.transform.position = spawnPos;
      spawnedWall.transform.eulerAngles =
        new Vector3(spawnedWall.transform.eulerAngles.x,
                    spawnedWall.transform.eulerAngles.y,
                    zRot);
    }
    zRot = 0;
    for (int i = 0; i < width; ++i)
    {
      GameObject spawnedWall;
      Vector3 spawnPos = anchor;
      spawnPos.x += m_RoomBaseSize.x * i;
      spawnPos.z -= m_RoomBaseSize.z * .5f;
      spawnedWall = Instantiate(Wall);
      spawnedWall.transform.position = spawnPos;
      spawnedWall.transform.eulerAngles =
        new Vector3(spawnedWall.transform.eulerAngles.x,
                    spawnedWall.transform.eulerAngles.y,
                    zRot);

      spawnPos = anchor;
      spawnPos.x += m_RoomBaseSize.x * i;
      spawnPos.z += m_RoomBaseSize.z * height;
      spawnPos.z -= m_RoomBaseSize.z * .5f;
      spawnedWall = Instantiate(Wall);
      spawnedWall.transform.position = spawnPos;
      spawnedWall.transform.eulerAngles =
        new Vector3(spawnedWall.transform.eulerAngles.x,
                    spawnedWall.transform.eulerAngles.y,
                    zRot);
    }

    //Create inner walls


    return true;
  }

  void SortRooms()
  {
    if (p_SortedRooms.Count != 0)
      p_SortedRooms.Clear();

    Debug.Log("[LVGEN] Sorting Rooms");

    //Do nothing atm
    //Place rooms into Sorted
    //int j = 0;
    int len = Rooms.Length;
    int[] roomSort = new int[len];
    int[] oldRooms = new int[len];

    for(int i = 0; i < len; ++ i)
    {  oldRooms[i] = i; }

    for(int i = 0; i < len; ++i)
    {
        float highestWeight = 0;
        int highestJ= 0;
        int j = 0;
        while ( j < len)
        {
            if (oldRooms[j] != -1 && highestWeight < NavMesh.GetAreaCost(oldRooms[j]+ (int)Room.ItemRoomA))
            {
               highestWeight = NavMesh.GetAreaCost(oldRooms[j] + (int)Room.ItemRoomA);
               highestJ = j;
            }
            ++j;
        }
       roomSort[i] = highestJ;
       oldRooms[highestJ] = -1;
    }

    for (int i = 0; i < len; ++i)
    {
       p_SortedRooms.Add(Rooms[roomSort[i]]);
    }

    int count = p_SortedRooms.Count;
    int added = 0;
    for(int i = 0; i < count-1; ++i)
    {
      for(int j = 0; j <= i; ++j)
      {
        p_SortedRooms.Insert(added, p_SortedRooms[(added++) + j]);
      }
    }
        added = 0;
}

  int IndexDistance(Vector2 index1, Vector2 index2)
  {
    return (int)(Mathf.Abs(index1.x - index2.x) + Mathf.Abs(index1.y - index2.y));
  }
}

  [Header("Stats")]
  public Player_Stat_Traker stats;
  public Target path;
  public bool SetSeed;
  public int seed;
    //Privates
    List<GameObject> p_SortedRooms;   //Possible Usage to sort by heuristic cost

    private void Awake()
    {
        p_SortedRooms = new List<GameObject>();
        m_SpawnedRooms = new List<GameObject>();

        if (m_GenerateOnStart)
            GenerateRooms(m_StartHeight, m_StartWidth);

        GetComponent<NavMeshSurface>().BuildNavMesh();
    }
    // Use this for initialization
    void Start () {
        
        GameObject spawnedRoom;
        int mask = 0;                //The number of indexes to mask based on distance
        //Set mask value
        float indexDistance = 
          IndexDistance(new Vector2(j, i), startIndex);
        float distanceRatio = indexDistance / maxIndexDistance;
        //Lock mask so it can only mask up to 75% of the total vector
        mask +=
          (int)(((float)p_SortedRooms.Count * .66f) * distanceRatio);
        Debug.Log("[LVGEN] Mask set to: " + mask +
                  " at Distance Ratio: " + distanceRatio + 
                  " from index: " + new Vector2(j, i) +
                  " to goal index: " + goalIndex);

       bool spawn = false;
       bool goal = false;
                //Check if start or goal
                if (i == startIndex.y && j == startIndex.x)
                {
                    spawnedRoom = Instantiate(StartRoom);
                    spawn = true;
                }
                else if (i == goalIndex.y && j == goalIndex.x)
                {
                    spawnedRoom = Instantiate(GoalRoom);
                    goal = true;
                }
                else
                    spawnedRoom =
                      Instantiate(p_SortedRooms[Random.Range(0, p_SortedRooms.Count - mask)]);


  public void BuildNextLevel()
  {
        //GenerateRooms(m_StartHeight, m_StartWidth);

        //GetComponent<NavMeshSurface>().BuildNavMesh();
        //stats.Restart();
    }