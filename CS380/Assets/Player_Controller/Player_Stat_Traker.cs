using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Stat_Traker : MonoBehaviour {

  float deaths_A = 0;
  float deaths_B = 0;
  public GameObject spawn;
  bool respawn = false;
  public float ADeathRatio;
  public float BDeathRatio;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if(respawn == true)
        {
            respawn = false;
        }

        if(deaths_A != 0)
        {
            ADeathRatio = (deaths_A / (deaths_A + deaths_B));
        }
        if(deaths_B != 0)
        {
            BDeathRatio = (deaths_B / (deaths_A + deaths_B));
        }
	}

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.collider.tag == "hazard_A" && respawn == false)
        {
            gameObject.GetComponent<Transform>().position = spawn.transform.position;
            ++deaths_A;
            respawn = true;
            gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
        }
        if (collision.collider.tag == "hazard_B" && respawn == false)
        {
            gameObject.GetComponent<Transform>().position = spawn.transform.position;
            ++deaths_B;
            respawn = true;
            gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
        }
    }

}
