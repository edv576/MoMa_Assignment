using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AngleSliderChange : MonoBehaviour {

    
    public GameObject angleValue;
    public GameObject joint1;
    public GameObject joint2;
    public GameObject joint3;
    public GameObject sliderJoint1;
    public GameObject sliderJoint2;
    public GameObject sliderJoint3;
    public int typeJoint;
    Text angleText;
    Slider sliderAngle1;
    Slider sliderAngle2;
    Slider sliderAngle3;

    // Use this for initialization
    void Start () {

        sliderAngle1 = sliderJoint1.GetComponent<Slider>();
        sliderAngle2 = sliderJoint2.GetComponent<Slider>();
        sliderAngle3 = sliderJoint3.GetComponent<Slider>();

        angleText = angleValue.GetComponent<Text>();

        switch(typeJoint)
        {
            case 1:
                {
                    angleText.text = sliderAngle1.value.ToString();
                    sliderAngle1.onValueChanged.AddListener(delegate { ValueChangeCheck(); });
                    break;
                }
            case 2:
                {
                    angleText.text = sliderAngle2.value.ToString();
                    sliderAngle2.onValueChanged.AddListener(delegate { ValueChangeCheck(); });
                    break;
                }
            case 3:
                {
                    angleText.text = sliderAngle3.value.ToString();
                    sliderAngle3.onValueChanged.AddListener(delegate { ValueChangeCheck(); });
                    break;
                }
        }




        

    }

    public void ValueChangeCheck()
    {
        //Debug.Log(mainSlider.value);
        //Quaternion childRotation1 = childJoint.transform.rotation;
        //Quaternion childRotation2 = childJoint2.transform.rotation;

        switch (typeJoint)
        {
            case 1:
                {
                    joint1.transform.eulerAngles = new Vector3(0.0f, 0.0f, sliderAngle1.value);
                    angleText.text = sliderAngle1.value.ToString();
                    break;
                }
            case 2:
                {
                    joint2.transform.eulerAngles = new Vector3(0.0f, 0.0f, sliderAngle1.value + sliderAngle2.value);
                    angleText.text = sliderAngle2.value.ToString();
                    break;
                }
            case 3:
                {
                    joint3.transform.eulerAngles = new Vector3(0.0f, 0.0f, sliderAngle1.value + sliderAngle2.value + sliderAngle3.value);
                    angleText.text = sliderAngle2.value.ToString();
                    break;
                }
        }

        
        
        //childJoint.transform.eulerAngles =  new Vector3(0.0f, 0.0f, sliderAngleSub1.value + sliderAngle.value);
        //childJoint2.transform.eulerAngles = new Vector3(0.0f, 0.0f, sliderAngleSub2.value + sliderAngleSub1.value + sliderAngle.value);
        //childJoint.transform.localEulerAngles = new Vector3(0.0f, 0.0f, sliderAngleSub1.value);
        //childJoint2.transform.localEulerAngles = new Vector3(0.0f, 0.0f, sliderAngleSub2.value);
        //childJoint.transform.localRotation = childJoint.transform.rotation;
        //childJoint2.transform.localRotation = childJoint2.transform.rotation;
        //print(childJoint.transform.rotation);
        //print(childJoint.transform.localRotation);
        //print(childJoint.transform.eulerAngles.ToString());
        //childJoint.transform.rotation = childRotation1;
        //childJoint2.transform.rotation = childRotation2;
       
        
    }

    // Update is called once per frame
    void Update () {

        //Quaternion childRotation1 = childJoint.transform.rotation;
        //Quaternion childRotation2 = childJoint2.transform.rotation;
        //joint.transform.eulerAngles = new Vector3(0.0f, 0.0f, sliderAngle.value);
        ////childJoint.transform.eulerAngles = childJoint.transform.eulerAngles + new Vector3(0.0f, 0.0f, sliderAngle.value);
        ////childJoint2.transform.eulerAngles = childJoint2.transform.eulerAngles + new Vector3(0.0f, 0.0f, sliderAngle.value);
        //childJoint.transform.rotation = childRotation1;
        //childJoint2.transform.rotation = childRotation2;
        //angleText.text = sliderAngle.value.ToString();
        

    }
}
