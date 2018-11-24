using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSystem : MonoBehaviour {
  [Header("Necessary Objects")]
  public GameObject[] Rooms;
  public GameObject Wall;

  [Header("Room Variables")]
  public Vector3 m_RoomBaseSize;

  [Header("System Variables")]
  public bool m_GenerateOnStart;
  public int m_StartHeight;
  public int m_StartWidth;

  [Header("Debug")]
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

    //Sort Rooms
    SortRooms();

    //Find lowest left corner
    Vector3 anchor = Vector3.zero;
    anchor.x = -m_RoomBaseSize.x * width;
    anchor.z = -m_RoomBaseSize.z * height;

    //Create rooms
    for (int i = 0; i < height; ++i)
    {
      for (int j = 0; j < width; ++j)
      {
        GameObject spawnedRoom = Instantiate(Rooms[0]);
        Vector3 spawnPos = anchor;
        spawnPos.x += m_RoomBaseSize.x * j;
        spawnPos.z += m_RoomBaseSize.z * i;
        spawnedRoom.transform.position = spawnPos;
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

    //Randomly create inner walls


    return true;
  }

  void SortRooms()
  {
    if (p_SortedRooms.Count != 0)
      p_SortedRooms.Clear();

    Debug.Log("[LVGEN] Sorting Rooms");

    //Do nothing atm
    //Place rooms into Sorted
    for (int i = 0; i < Rooms.Length; ++i)
      p_SortedRooms.Add(Rooms[i]);

  }
}
