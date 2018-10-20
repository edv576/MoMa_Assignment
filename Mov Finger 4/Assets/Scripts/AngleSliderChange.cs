using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Single;

public class AngleSliderChange : MonoBehaviour {

    
    public GameObject angleValue;
    public GameObject joint1;
    public GameObject joint2;
    public GameObject joint3;
    public GameObject sliderJoint1;
    public GameObject sliderJoint2;
    public GameObject sliderJoint3;
    public GameObject fixedRatioCheck;
    GameObject angleValueJoint3;
    public int typeJoint;
    Text angleText;
    Text angleTextJoint3;
    Text coordinatesText;
    Slider sliderAngle1;
    Slider sliderAngle2;
    Slider sliderAngle3;
    Toggle toogleFixedRatio;
    float proximalPhalanxLength;
    float intermediatePhalanxLength;
    float distalPhalanxLength;

    // Use this for initialization
    void Start () {

        sliderAngle1 = sliderJoint1.GetComponent<Slider>();
        sliderAngle2 = sliderJoint2.GetComponent<Slider>();
        sliderAngle3 = sliderJoint3.GetComponent<Slider>();

        toogleFixedRatio = fixedRatioCheck.GetComponent<Toggle>();
        toogleFixedRatio.isOn = false;

        angleText = angleValue.GetComponent<Text>();

        angleValueJoint3 = GameObject.Find("DIP Angle");
        angleTextJoint3 = angleValueJoint3.GetComponent<Text>();

        proximalPhalanxLength = 39.8f;
        intermediatePhalanxLength = 22.4f;
        distalPhalanxLength = 15.8f;

        float x, y;

        x = proximalPhalanxLength * Mathf.Cos(sliderAngle1.value * (Mathf.PI / 180.0f)) +
            intermediatePhalanxLength * Mathf.Cos((sliderAngle1.value + sliderAngle2.value) * (Mathf.PI / 180.0f)) +
            distalPhalanxLength * Mathf.Cos((sliderAngle1.value + sliderAngle2.value + sliderAngle3.value) * (Mathf.PI / 180.0f));
        y = proximalPhalanxLength * Mathf.Sin(sliderAngle1.value * (Mathf.PI / 180.0f)) +
            intermediatePhalanxLength * Mathf.Sin((sliderAngle1.value + sliderAngle2.value) * (Mathf.PI / 180.0f)) +
            distalPhalanxLength * Mathf.Sin((sliderAngle1.value + sliderAngle2.value + sliderAngle3.value) * (Mathf.PI / 180.0f));

        x = Mathf.Round(x * 100f) / 100f;
        y = Mathf.Round(y * 100f) / 100f;

        coordinatesText = GameObject.Find("Values Coordinates").GetComponent<Text>();
        coordinatesText.text = "X => " + x.ToString() + " Y => " + y.ToString();

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

        toogleFixedRatio.onValueChanged.AddListener(delegate { ValueChangeCheck(); });








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
                    if (toogleFixedRatio.isOn)
                    {
                        joint3.transform.eulerAngles = new Vector3(0.0f, 0.0f, sliderAngle1.value + sliderAngle2.value + (sliderAngle2.value * 2) / 3);
                        angleTextJoint3.text = ((sliderAngle2.value * 2) / 3).ToString();
                        sliderAngle3.value = ((sliderAngle2.value * 2) / 3);

                    }
                    break;
                }
            case 3:
                {
                    print(toogleFixedRatio.isOn);
                    if (!toogleFixedRatio.isOn)
                    {
                        //sliderAngle3.interactable = true;
                        joint3.transform.eulerAngles = new Vector3(0.0f, 0.0f, sliderAngle1.value + sliderAngle2.value + sliderAngle3.value);
                        angleText.text = sliderAngle3.value.ToString();
                    }
                    else
                    {
                        //sliderAngle3.interactable = false;
                        //joint3.transform.eulerAngles = new Vector3(0.0f, 0.0f, sliderAngle1.value + sliderAngle2.value + (sliderAngle2.value * 2) / 3);
                        //angleText.text = ((sliderAngle2.value * 2) / 3).ToString();
                        print(angleText.text);
                        
                    }
                    
                    
                    break;
                }
        }

        float x, y;

        x = proximalPhalanxLength * Mathf.Cos(sliderAngle1.value * (Mathf.PI / 180.0f)) +
            intermediatePhalanxLength * Mathf.Cos((sliderAngle1.value + sliderAngle2.value) * (Mathf.PI / 180.0f)) +
            distalPhalanxLength * Mathf.Cos((sliderAngle1.value + sliderAngle2.value + sliderAngle3.value) * (Mathf.PI / 180.0f));
        y = proximalPhalanxLength * Mathf.Sin(sliderAngle1.value * (Mathf.PI / 180.0f)) +
            intermediatePhalanxLength * Mathf.Sin((sliderAngle1.value + sliderAngle2.value) * (Mathf.PI / 180.0f)) +
            distalPhalanxLength * Mathf.Sin((sliderAngle1.value + sliderAngle2.value + sliderAngle3.value) * (Mathf.PI / 180.0f));

        x = Mathf.Round(x * 100f) / 100f;
        y = Mathf.Round(y * 100f) / 100f;
        coordinatesText.text = "X => " + x.ToString() + " Y => " + y.ToString();



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
