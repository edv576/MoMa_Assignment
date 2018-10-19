using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CheckBehaviour : MonoBehaviour {

    Toggle checkO;
    public GameObject O;

	// Use this for initialization
	void Start () {

        checkO = GetComponent<Toggle>();
        checkO.isOn = true;

        checkO.onValueChanged.AddListener(delegate { ValueChangeCheck(); });
    }

    public void ValueChangeCheck()
    {
        if (checkO.isOn)
        {
            O.SetActive(true);

        }
        else
        {
            O.SetActive(false);

        }
    }

    // Update is called once per frame
    void Update ()
    {

        
		
	}
}
