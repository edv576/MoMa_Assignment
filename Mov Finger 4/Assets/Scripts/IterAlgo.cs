using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Single;
using MathNet.Numerics.Statistics.Mcmc;
using UnityEditor.Experimental.UIElements;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class IterAlgo : MonoBehaviour
{
    private float L1 = 3.98f;
    private float L2 = 2.24f;
    private float L3 = 1.58f;

    public Slider sliderAngle1;
    public Slider sliderAngle2;
    public Slider sliderAngle3;
    public GameObject sliderJoint1;
    public GameObject sliderJoint2;
    public GameObject sliderJoint3;
    private bool found = false;

    private float theta_1min = -Mathf.PI / 3f;       // -60  degrees
    private float theta_1max = Mathf.PI / 3f;        //  60  degrees
    private float theta_2min = -2f / 3f * Mathf.PI;  // -120 degrees
    private float theta_2max = 0;                    //  0   degrees
    private float theta_3min = -2f / 3f * Mathf.PI;  // -120 degrees
    private float theta_3max = 0;                    //  0   degrees

    private float lastTheta1 = -0.001f;
    private float lastTheta2 = -0.001f;
    private float lastTheta3 = -0.001f;

    public InitModeEnum InitMode = InitModeEnum.Zero;

    public enum InitModeEnum
    {
        Zero,
        Random,
        Previous
    };



    //TODO: THIS SHOULD BE SET VARIABLY, it's now set to (3,-2)
    Vector<float> goalPos = Vector<float>.Build.DenseOfArray(new[] { 4f, 2f });
    public bool isFixed;

    // Use this for initialization
    void Start()
    {

        sliderJoint1 = GameObject.Find("MCP Slider");
        sliderJoint2 = GameObject.Find("PIP Slider");
        sliderJoint3 = GameObject.Find("DIP Slider");
        sliderAngle1 = sliderJoint1.GetComponent<Slider>();
        sliderAngle2 = sliderJoint2.GetComponent<Slider>();
        sliderAngle3 = sliderJoint3.GetComponent<Slider>();

    }

    IEnumerator touchOIllusion()
    {
        int totalIterations = 0;
        if (!isFixed)
        {
            float x1 = 6.0f;
            float prevTheta1 = 0;
            float prevTheta2 = 0;
            float prevTheta3 = 0;


            while (x1 > 2.0f)
            {
                goalPos = Vector<float>.Build.DenseOfArray(new[] { x1, -2f });

                switch (InitMode)
                {
                    case InitModeEnum.Zero:
                        prevTheta1 = 0;
                        prevTheta2 = 0;
                        prevTheta3 = 0;
                        break;
                    case InitModeEnum.Random:
                        prevTheta1 = Random.Range(theta_1min, theta_1max);
                        prevTheta2 = Random.Range(theta_2min, theta_2max);
                        prevTheta3 = Random.Range(theta_2min, theta_2max);
                        break;
                    case InitModeEnum.Previous:
                        break;
                }

                float epsilon = 9999f;

                Vector<float> prevQ = DenseVector.OfArray(new[] { prevTheta1, prevTheta2, prevTheta3 });


                int iterations = 0;
                int maxIterations = 50;


                while (true)
                {
                    Debug.Log($"iteration: {iterations}");

                    Vector<float> newQ = inverseJacobian(prevTheta1, prevTheta2, prevTheta3);
                    epsilon = (float)(newQ - prevQ).L2Norm();

                    Debug.Log($"epsilon: {epsilon}");

                   // if (epsilon <= 0.0001f) break;

                    prevTheta1 = newQ[0];
                    prevTheta2 = newQ[1];
                    prevTheta3 = newQ[2];

                    var diff = newQ - prevTheta1;

                    prevQ = DenseVector.OfArray(new[] { prevTheta1, prevTheta2, prevTheta3 });

                    var finaltheta1 = prevTheta1 * Mathf.Rad2Deg;
                    var finaltheta2 = (prevTheta1 + prevTheta2) * Mathf.Rad2Deg;
                    var finaltheta3 = (prevTheta1 + prevTheta2 + prevTheta3) * Mathf.Rad2Deg;
                    sliderAngle1.value = prevTheta1 * Mathf.Rad2Deg;
                    sliderAngle2.value = prevTheta2 * Mathf.Rad2Deg;
                    sliderAngle3.value = prevTheta3 * Mathf.Rad2Deg;

                    if (found)
                    {
                        // GameObject.Find("MCP").transform.eulerAngles = new Vector3(0, 0, prevTheta1 * Mathf.Rad2Deg);
                        // GameObject.Find("PIP").transform.eulerAngles = new Vector3(0, 0, (prevTheta1 + prevTheta2) * Mathf.Rad2Deg);
                        // GameObject.Find("DIP").transform.eulerAngles = new Vector3(0, 0, (prevTheta3 + prevTheta1 + prevTheta2) * Mathf.Rad2Deg);

                        Debug.Log($"1: {finaltheta1}. 2: {finaltheta2}. 3: {finaltheta3}");

                        if (!isFixed)
                        {
                            sliderAngle1.value = prevTheta1 * Mathf.Rad2Deg;
                            sliderAngle2.value = prevTheta2 * Mathf.Rad2Deg;
                            sliderAngle3.value = prevTheta3 * Mathf.Rad2Deg;
                        }
                        else
                        {
                            GameObject.Find("MCP").transform.eulerAngles = new Vector3(0, 0, prevTheta1 * Mathf.Rad2Deg);
                            GameObject.Find("PIP").transform.eulerAngles = new Vector3(0, 0, (prevTheta1 + prevTheta2) * Mathf.Rad2Deg);
                            GameObject.Find("DIP").transform.eulerAngles = new Vector3(0, 0, (prevTheta3 + prevTheta1 + prevTheta2) * Mathf.Rad2Deg);
                        }

                        totalIterations += iterations;
                        break;
                    }

                    iterations++;
                    if (iterations > maxIterations)
                        break;

                }


                yield return new WaitForSeconds(0.05f);
                x1 -= 0.1f;

            }

        }


    }

    // Update is called once per frame
    void Update()
    {

        isFixed = GameObject.Find("Fixed Ratio Check").GetComponent<Toggle>().isOn;

        if (Input.GetKeyDown(KeyCode.I))
        {
            StartCoroutine(touchOIllusion());

        }

        if (!Input.GetKeyUp("space")) return;

        var gps = GameObject.Find("Destination").transform.position;
        goalPos = Vector<float>.Build.DenseOfArray(new[] { gps.x, gps.y });


        float prevTheta1 = lastTheta1;
        float prevTheta2 = lastTheta2;
        float prevTheta3 = lastTheta3;


        // float prevTheta1 = Random.Range(theta_1min, theta_1max);
        // float prevTheta2 = Random.Range(theta_2min, theta_2max);
        // float prevTheta3 = Random.Range(theta_2min, theta_2max);

        float epsilon = 9999f;

        Vector<float> prevQ = DenseVector.OfArray(new[] { prevTheta1, prevTheta2, prevTheta3 });

        int iterations = 0;
        int maxIterations = 50;

        while (true)
        {
            Debug.Log($"iteration: {iterations}");

            Vector<float> newQ = inverseJacobian(prevTheta1, prevTheta2, prevTheta3);
            epsilon = (float)(newQ - prevQ).L2Norm();

            Debug.Log($"epsilon: {epsilon}");

            //if (epsilon <= 0.0001f) break;

            prevTheta1 = newQ[0];
            prevTheta2 = newQ[1];
            prevTheta3 = newQ[2];

            var diff = newQ - prevTheta1;

            prevQ = DenseVector.OfArray(new[] { prevTheta1, prevTheta2, prevTheta3 });

            var finaltheta1 = prevTheta1 * Mathf.Rad2Deg;
            var finaltheta2 = (prevTheta1 + prevTheta2) * Mathf.Rad2Deg;
            var finaltheta3 = (prevTheta1 + prevTheta2 + prevTheta3) * Mathf.Rad2Deg;
            sliderAngle1.value = prevTheta1 * Mathf.Rad2Deg;
            sliderAngle2.value = prevTheta2 * Mathf.Rad2Deg;
            sliderAngle3.value = prevTheta3 * Mathf.Rad2Deg;


            lastTheta1 = prevTheta1;
            lastTheta2 = prevTheta2;
            lastTheta3 = prevTheta3;

            if (found)
            {
                found = false;
                // GameObject.Find("MCP").transform.eulerAngles = new Vector3(0, 0, prevTheta1 * Mathf.Rad2Deg);
                // GameObject.Find("PIP").transform.eulerAngles = new Vector3(0, 0, (prevTheta1 + prevTheta2) * Mathf.Rad2Deg);
                // GameObject.Find("DIP").transform.eulerAngles = new Vector3(0, 0, (prevTheta3 + prevTheta1 + prevTheta2) * Mathf.Rad2Deg);

                Debug.Log($"1: {finaltheta1}. 2: {finaltheta2}. 3: {finaltheta3}");

                //if (!isFixed)
                //{
                //    sliderAngle1.value = prevTheta1 * Mathf.Rad2Deg;
                //    sliderAngle2.value = prevTheta2 * Mathf.Rad2Deg;
                //    sliderAngle3.value = prevTheta3 * Mathf.Rad2Deg;
                //}
                //else
                //{
                //    GameObject.Find("MCP").transform.eulerAngles = new Vector3(0, 0, prevTheta1 * Mathf.Rad2Deg);
                //    GameObject.Find("PIP").transform.eulerAngles = new Vector3(0, 0, (prevTheta1 + prevTheta2) * Mathf.Rad2Deg);
                //    GameObject.Find("DIP").transform.eulerAngles = new Vector3(0, 0, (prevTheta3 + prevTheta1 + prevTheta2) * Mathf.Rad2Deg);
                //}

                return;
            }

            iterations++;
            if (iterations > maxIterations)
                break;

        }





    }

    Vector<float> inverseJacobian2(float x, float y)
    {

        var f1 = Mathf.Cos(x) + Mathf.Cos(x + y);
        var f2 = Mathf.Sin(x) + Mathf.Sin(x + y);




        //x, y , and z are theta 1, 2, and 3 respectively
        float f1_x = -L1 * Mathf.Sin(x) -
                      L2 * Mathf.Sin(x + y);

        float f1_y = -L2 * Mathf.Sin(x + y);

        float f2_x = L1 * Mathf.Cos(x) +
                      L2 * Mathf.Cos(x + y);

        float f2_y = L2 * Mathf.Cos(x + y);

        Matrix<float> Jacobian = DenseMatrix.OfArray(new[,] {
            {f1_x, f1_y  },
            {f2_x, f2_y  }});

        var invJ = Jacobian.PseudoInverse();


        Vector<float> T = Vector<float>.Build.DenseOfArray(new[] { f1, f2 });

        var GoalVec = Vector<float>.Build.DenseOfArray(new[] { 1f, 1f });
        var deltaT = GoalVec - T;
        var prevTheta = Vector<float>.Build.DenseOfArray(new[] { x, y });

        var newTheta = prevTheta + invJ * deltaT;

        return newTheta;
    }



    //x y z in rads
    Vector<float> inverseJacobian(float x, float y, float z)
    {
        //these are the original function from the
        //original matrix (not jacobian!)
        float f1X = L1 * Mathf.Cos(x) +
                    L2 * Mathf.Cos(x + y) +
                    L3 * Mathf.Cos(x + y + z);

        float f2Y = L1 * Mathf.Sin(x) +
                    L2 * Mathf.Sin(x + y) +
                    L3 * Mathf.Sin(x + y + z);

        //Gradients of f1 and f2
        //x, y , and z are theta 1, 2, and 3 respectively
        float f1_x = -L1 * Mathf.Sin(x) -
                      L2 * Mathf.Sin(x + y) -
                      L3 * Mathf.Sin(x + y + z);

        float f1_y = -L2 * Mathf.Sin(x + y) -
                      L3 * Mathf.Sin(x + y + z);

        float f1_z = -L3 * Mathf.Sin(x + y + z);

        float f2_x = L1 * Mathf.Cos(x) +
                      L2 * Mathf.Cos(x + y) +
                      L3 * Mathf.Cos(x + y + z);

        float f2_y = L2 * Mathf.Cos(x + y) +
                      L3 * Mathf.Cos(x + y + z);

        float f2_z = L3 * Mathf.Cos(x + y + z);

        //2x3 Jacobian
        Matrix<float> Jacobian = DenseMatrix.OfArray(new[,] {
            {f1_x, f1_y, f1_z},
            {f2_x, f2_y, f2_z}});

        //pseudo inverse becomes 3x2
        var invJ = Jacobian.PseudoInverse();

        //prev guess is the x,y position with the previously calculated thetas
        Vector<float> prevGuess = Vector<float>.Build.DenseOfArray(new[] { f1X, f2Y });

        //q is the vector of current thetas (q^(i))
        Vector<float> q = Vector<float>.Build.DenseOfArray(new[] { x, y, z });

        //the difference between the previous and goal positino
        Vector<float> deltaT = goalPos - prevGuess;

        if (deltaT.L2Norm() < 0.001f)
        {
            found = true;
            Debug.Log($"EARLY STOPPING AT {prevGuess}");
            return q;
        }

        var newQ = q + invJ * deltaT;

        newQ[0] = Mathf.Clamp(newQ[0], theta_1min, theta_1max);
        newQ[1] = Mathf.Clamp(newQ[1], theta_2min, theta_2max);
        newQ[2] = Mathf.Clamp(newQ[2], theta_3min, theta_3max);

        if (isFixed)
            newQ[2] = 2f / 3f * newQ[1];

        return newQ;
    }

}
