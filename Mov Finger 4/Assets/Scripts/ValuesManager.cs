﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Single;

public class ValuesManager : MonoBehaviour {

    Slider sliderAngle1;
    Slider sliderAngle2;
    Slider sliderAngle3;
    Text coordinatesLastO;
    Text anglesLastO;

    float proximalPhalanxLength;
    float intermediatePhalanxLength;
    float distalPhalanxLength;

    // Use this for initialization
    void Start () {

        coordinatesLastO = GameObject.Find("Values Last Point O").GetComponent<Text>();
        anglesLastO = GameObject.Find("Values Angles Last Point O").GetComponent<Text>();

        sliderAngle1 = GameObject.Find("MCP Slider").GetComponent<Slider>();
        sliderAngle2 = GameObject.Find("PIP Slider").GetComponent<Slider>();
        sliderAngle3 = GameObject.Find("DIP Slider").GetComponent<Slider>();

        proximalPhalanxLength = 39.8f;
        intermediatePhalanxLength = 22.4f;
        distalPhalanxLength = 15.8f;

        sliderAngle1.onValueChanged.AddListener(delegate { ValueChangeCheck(); });
        sliderAngle2.onValueChanged.AddListener(delegate { ValueChangeCheck(); });
        sliderAngle3.onValueChanged.AddListener(delegate { ValueChangeCheck(); });

    }

    public void ValueChangeCheck()
    {
        float x, y;

        x = proximalPhalanxLength * Mathf.Cos(sliderAngle1.value * (Mathf.PI / 180.0f)) +
            intermediatePhalanxLength * Mathf.Cos((sliderAngle1.value + sliderAngle2.value) * (Mathf.PI / 180.0f)) +
            distalPhalanxLength * Mathf.Cos((sliderAngle1.value + sliderAngle2.value + sliderAngle3.value) * (Mathf.PI / 180.0f));
        y = proximalPhalanxLength * Mathf.Sin(sliderAngle1.value * (Mathf.PI / 180.0f)) +
            intermediatePhalanxLength * Mathf.Sin((sliderAngle1.value + sliderAngle2.value) * (Mathf.PI / 180.0f)) +
            distalPhalanxLength * Mathf.Sin((sliderAngle1.value + sliderAngle2.value + sliderAngle3.value) * (Mathf.PI / 180.0f));

        x = Mathf.Round(x * 100f) / 100f;
        y = Mathf.Round(y * 100f) / 100f;

        if (y < -20.0f)
        {
            coordinatesLastO.text = "X => " + x.ToString() + " Y => " + y.ToString();
            anglesLastO.text = "MCP => " + Mathf.Round(sliderAngle1.value * 100f) / 100f + 
                " \nPIP => " + Mathf.Round(sliderAngle2.value * 100f) / 100f + 
                " \nDIP => " + Mathf.Round(sliderAngle3.value * 100f) / 100f;
        }


    }

    // Update is called once per frame
    void Update () {
		
	}
}
