using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum Rooms
{
    Walkable = 0,
    NotWalkable = 1,
    Jump = 2,
    StartRoom = 3,
    GoalRoom = 4,
    ItemRoomA = 5,
    ItemRoomB = 6,
    EmptyRoom = 7,
    HazardRoomA = 8,
    HazardRoomB = 9,
    RoomIndex = 10
};

public class Player_Stat_Traker : MonoBehaviour {

  float Deaths_A = 0;
  float Deaths_B = 0;
  float Item_A = 0;
  float Item_B = 0;
  float Hazard_A_Enter = 0;
  float Hazard_B_Enter = 0;
  public float Item_A_Amount = 10;
  public float Item_B_Amount = 10;
  bool respawn = false;
    bool cooldown = false;
  public float Item_A_Left = 0;
  public float Item_B_Left = 0;
  public float AB_DeathRatio = 0;
  public float BA_DeathRatio = 0;
  public float PathRefreshTime;
  float DT;

  public float[] RoomWeights;

    public GameObject spawn;
    public Target path;
    public GameObject Item_A_Counter;
  public GameObject Item_B_Counter;
    // Use this for initialization
    void Start()
    {
        spawn = GameObject.FindGameObjectWithTag("Spawn");
        //gameObject.transform.position = spawn.transform.position;

        RoomWeights = new float[(int)Rooms.RoomIndex];
        
        string tex = "Item A: " + Item_A.ToString() + "/" + Item_A_Amount.ToString();
        Item_A_Counter.GetComponent<Text>().text = tex;
        tex = "Item B: " + Item_B.ToString() + "/" + Item_B_Amount.ToString();
        Item_B_Counter.GetComponent<Text>().text = tex;
        
        RoomWeights[(int)Rooms.StartRoom] = 0.0f;
        RoomWeights[(int)Rooms.HazardRoomA] = AB_DeathRatio;
        RoomWeights[(int)Rooms.HazardRoomB] = BA_DeathRatio;
        RoomWeights[(int)Rooms.ItemRoomA] = Item_A_Left;
        RoomWeights[(int)Rooms.ItemRoomB] = Item_B_Left;
        RoomWeights[(int)Rooms.GoalRoom] = 0.0f;

        DT = PathRefreshTime;
        //NavMeshPath path = null;
        //NavMesh.CalculatePath(spawn.transform.position, g, NavMesh.AllAreas, path);
        //GetComponent<NavMeshAgent>().SetPath(path);
        //Debug.LogError("the number of nodes are: " + path.corners);
    }
	
	// Update is called once per frame
	void Update () {
            if (respawn == true)
            {
                path.ReCalculatePath();
                respawn = false;
            }
            if (cooldown == true)
            {
                cooldown = false;
            }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.collider.tag == "HazardA" && respawn == false)
        {
            gameObject.GetComponent<Transform>().position = spawn.transform.position;
            ++Deaths_A;
            respawn = true;
            gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;

            AB_DeathRatio = ((Deaths_A) / ((Deaths_A  + Deaths_B)));
            BA_DeathRatio = ((Deaths_B) / ((Deaths_A  + Deaths_B)));
            RoomWeights[(int)Rooms.HazardRoomA] = AB_DeathRatio;
            RoomWeights[(int)Rooms.HazardRoomB] = BA_DeathRatio;
        }
        if (collision.collider.tag == "HazardB" && respawn == false)
        {
            gameObject.GetComponent<Transform>().position = spawn.transform.position;
            ++Deaths_B;
            respawn = true;
            gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
            BA_DeathRatio = (Deaths_B / (Deaths_A + Deaths_B));
            AB_DeathRatio = (Deaths_A / (Deaths_A + Deaths_B));
            RoomWeights[(int)Rooms.HazardRoomA] = AB_DeathRatio;
            RoomWeights[(int)Rooms.HazardRoomB] = BA_DeathRatio;
        }

    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "ItemA" && cooldown == false)
        {
            cooldown = true;
            ++Item_A;
            Item_A_Left = 1.0f - (Item_A_Amount - Item_A) / Item_A_Amount;
            Object.Destroy(other.gameObject);
            string tex = "Item A: " + Item_A.ToString() + "/" + Item_A_Amount.ToString();
            Item_A_Counter.GetComponent<Text>().text = tex;
            RoomWeights[(int)Rooms.ItemRoomA] = Item_A_Left;
        }
        if (other.tag == "ItemB" && cooldown == false)
        {
            cooldown = true;
            ++Item_B;
            Item_B_Left = 1.0f - (Item_B_Amount - Item_B) / Item_B_Amount;
            Object.Destroy(other.gameObject);
            string tex = "Item B: " + Item_B.ToString() + "/" + Item_B_Amount.ToString();
            Item_B_Counter.GetComponent<Text>().text = tex;
            RoomWeights[(int)Rooms.ItemRoomB] = Item_B_Left;
        }
        if (other.tag == "HazardAEnter" && cooldown == false)
        {
            cooldown = true;
            ++Hazard_A_Enter;
            Object.Destroy(other.gameObject);
            RoomWeights[(int)Rooms.HazardRoomA] = AB_DeathRatio;
        }
        if (other.tag == "HazardBEnter" && cooldown == false)
        {
            cooldown = true;
            ++Hazard_B_Enter;
            Object.Destroy(other.gameObject);
            RoomWeights[(int)Rooms.HazardRoomB] = BA_DeathRatio;
        }
    }

    public float GetCost(Rooms room)
    {
        return RoomWeights[(int)room];
    }
}
