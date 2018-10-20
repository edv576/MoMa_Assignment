using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DestinationBehavior : MonoBehaviour {

    float speed;

	// Use this for initialization
	void Start () {

        speed = 0.01f;
        
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKey(KeyCode.UpArrow))
        {
            transform.position += new Vector3(0.0f, speed, 0.0f); 

        }

        if (Input.GetKey(KeyCode.DownArrow))
        {
            transform.position += new Vector3(0.0f, -speed, 0.0f);

        }

        if (Input.GetKey(KeyCode.RightArrow))
        {
            transform.position += new Vector3(speed, 0.0f, 0.0f);

        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            transform.position += new Vector3(-speed, 0.0f, 0.0f);

        }

    }

   
}
