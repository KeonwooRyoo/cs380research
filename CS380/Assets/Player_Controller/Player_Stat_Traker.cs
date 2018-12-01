using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum Rooms
{
    HazardRoomA,
    HazardRoomB,
    ItemRoomA,
    ItemRoomB,
    RoomIndex
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
  public GameObject spawn;
  bool respawn = false;
  public float Item_A_Left = 0;
  public float Item_B_Left = 0;
  public float AB_DeathRatio = 0;
  public float BA_DeathRatio = 0;

  public float[] RoomWeights;

  public GameObject Item_A_Counter;
  public GameObject Item_B_Counter;
    // Use this for initialization
    void Start ()
    {
        RoomWeights = new float[(int)Rooms.RoomIndex];
        spawn = GameObject.FindGameObjectWithTag("Spawn");
        gameObject.GetComponent<Transform>().position = spawn.transform.position;
        string tex = "Item A: " + Item_A.ToString() + "/" + Item_A_Amount.ToString();
        Item_A_Counter.GetComponent<Text>().text = tex;
        tex = "Item B: " + Item_B.ToString() + "/" + Item_B_Amount.ToString();
        Item_B_Counter.GetComponent<Text>().text = tex;


        //RoomWeights[(int)Rooms.SpawnRoom] = 0.0f;
        RoomWeights[(int)Rooms.HazardRoomA] = AB_DeathRatio;
        RoomWeights[(int)Rooms.HazardRoomB] = BA_DeathRatio;
        RoomWeights[(int)Rooms.ItemRoomA] = Item_A_Left;
        RoomWeights[(int)Rooms.ItemRoomB] = Item_B_Left;
        //RoomWeights[(int)Rooms.GoalRoom] = 0.0f;
    }
	
	// Update is called once per frame
	void Update () {
        if(respawn == true)
        {
            respawn = false;
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
        }
        if (collision.collider.tag == "HazardB" && respawn == false)
        {
            gameObject.GetComponent<Transform>().position = spawn.transform.position;
            ++Deaths_B;
            respawn = true;
            gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
            BA_DeathRatio = (Deaths_B / (Deaths_A + Deaths_B));
            AB_DeathRatio = (Deaths_A / (Deaths_A + Deaths_B));
        }

    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "ItemA" && respawn == false)
        {
            respawn = true;
            ++Item_A;
            Item_A_Left = 1.0f - (Item_A_Amount - Item_A) / Item_A_Amount;
            Object.Destroy(other.gameObject);
            string tex = "Item A: " + Item_A.ToString() + "/" + Item_A_Amount.ToString();
            Item_A_Counter.GetComponent<Text>().text = tex;
            RoomWeights[(int)Rooms.ItemRoomA] = Item_A_Left;
        }
        if (other.tag == "ItemB" && respawn == false)
        {
            respawn = true;
            ++Item_B;
            Item_B_Left = 1.0f - (Item_B_Amount - Item_B) / Item_B_Amount;
            Object.Destroy(other.gameObject);
            string tex = "Item B: " + Item_B.ToString() + "/" + Item_B_Amount.ToString();
            Item_B_Counter.GetComponent<Text>().text = tex;
            RoomWeights[(int)Rooms.ItemRoomB] = Item_B_Left;
        }
        if (other.tag == "HazardAEnter" && respawn == false)
        {
            respawn = true;
            ++Hazard_A_Enter;
            Object.Destroy(other.gameObject);
            RoomWeights[(int)Rooms.HazardRoomA] = AB_DeathRatio;
        }
        if (other.tag == "HazardBEnter" && respawn == false)
        {
            respawn = true;
            ++Hazard_B_Enter;
            Object.Destroy(other.gameObject);
            RoomWeights[(int)Rooms.HazardRoomB] = BA_DeathRatio;
        }
    }

    public float GetCaust(Rooms room)
    {
    Debug.Log("[STAT] Getting cost for room: " + room);
        return RoomWeights[(int)room];
    }
}
