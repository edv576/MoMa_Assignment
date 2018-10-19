using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AngleSliderChange2 : MonoBehaviour {

    public GameObject joint;
    public GameObject angleValue;
    public GameObject childJoint;
    public GameObject childJoint2;
    public GameObject subAngleSlider1;
    public GameObject subAngleSlider2;
    Text angleText;
    Slider sliderAngle;
    Slider sliderAngleSub1;
    Slider sliderAngleSub2;

    // Use this for initialization
    void Start () {

        sliderAngle = GetComponent<Slider>();
        sliderAngleSub1 = subAngleSlider1.GetComponent<Slider>();
        sliderAngleSub2 = subAngleSlider2.GetComponent<Slider>();
        angleText = angleValue.GetComponent<Text>();
        angleText.text = sliderAngle.value.ToString();

        sliderAngle.onValueChanged.AddListener(delegate { ValueChangeCheck(); });

    }

    public void ValueChangeCheck()
    {
        //Debug.Log(mainSlider.value);
        //Quaternion childRotation1 = childJoint.transform.rotation;
        //Quaternion childRotation2 = childJoint2.transform.rotation;

        joint.transform.eulerAngles = new Vector3(0.0f, 0.0f, sliderAngle.value);
        childJoint.transform.eulerAngles = new Vector3(0.0f, 0.0f, sliderAngleSub1.value);
        childJoint2.transform.eulerAngles = new Vector3(0.0f, 0.0f, sliderAngleSub2.value);
        childJoint.transform.localEulerAngles = new Vector3(0.0f, 0.0f, sliderAngleSub1.value);
        childJoint2.transform.localEulerAngles = new Vector3(0.0f, 0.0f, sliderAngleSub2.value);
        //childJoint.transform.localRotation = childJoint.transform.rotation;
        //childJoint2.transform.localRotation = childJoint2.transform.rotation;
        print(childJoint.transform.rotation);
        print(childJoint.transform.localRotation);
        print(childJoint.transform.eulerAngles.ToString());
        //childJoint.transform.rotation = childRotation1;
        //childJoint2.transform.rotation = childRotation2;
       
        angleText.text = sliderAngle.value.ToString();
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
