using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour {
  public Transform Target;

 public void SetTarget(Transform target)
  {
      Target = target;
  }

	// Use this for initialization
	void Start () {
		
	}

    // Update is called once per frame
    void Update()
    {
       Vector3 pos = Target.position;
        pos.y = gameObject.GetComponent<Transform>().position.y;
        gameObject.GetComponent<Transform>().LookAt(pos, Vector3.up);
    }
}
