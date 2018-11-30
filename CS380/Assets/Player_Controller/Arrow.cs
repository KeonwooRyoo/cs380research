using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour {
    //public Transform Target;
    GameObject goal;

    public void SetTarget(Transform target)
    {
        //Target = target;
    }
    // Use this for initialization
    void Start()
    {
        goal = GameObject.FindGameObjectWithTag("Goal");
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 pos = goal.GetComponent<Transform>().position;
        pos.y = gameObject.GetComponent<Transform>().position.y;
        gameObject.GetComponent<Transform>().LookAt(pos, Vector3.up);
    }
}
