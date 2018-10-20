using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Single;
using MathNet.Numerics.Statistics.Mcmc;
using UnityEditor.Experimental.UIElements;
using Random = UnityEngine.Random;

public class IterAlgo : MonoBehaviour
{
    private float L1 = 3.98f;
    private float L2 = 2.24f;
    private float L3 = 1.58f;

    private bool found = false;


    //TODO: THIS SHOULD BE SET VARIABLY, it's now set to (1,1)
    Vector<float> goalPos = Vector<float>.Build.DenseOfArray(new[] { 6f, -2f });

    // Use this for initialization
    void Start()
    {
        //  inverseJacobian (0, 0, 0);

        float prevTheta1 = Random.Range(0, Mathf.PI);
        float prevTheta2 = Random.Range(0, Mathf.PI);
        float prevTheta3 = Random.Range(0, Mathf.PI);

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

            if (epsilon <= 0.0001f) break;

            prevTheta1 = newQ[0];
            prevTheta2 = newQ[1];
            prevTheta3 = newQ[2];

            var diff = newQ - prevTheta1;

            prevQ = DenseVector.OfArray(new[] { prevTheta1, prevTheta2, prevTheta3 });

            GameObject.Find("MCP").transform.localRotation = Quaternion.AngleAxis(prevTheta1 * Mathf.Rad2Deg, new Vector3(0, 0, 1));
            GameObject.Find("PIP").transform.localRotation = Quaternion.AngleAxis(prevTheta2 * Mathf.Rad2Deg, new Vector3(0, 0, 1));
            GameObject.Find("DIP").transform.localRotation = Quaternion.AngleAxis(prevTheta3 * Mathf.Rad2Deg, new Vector3(0, 0, 1));


            if (found)
            {
                return;
            }

            iterations++;
            if (iterations > maxIterations)
                break;

        }


    }


    // Update is called once per frame
    void Update()
    {



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
        if (deltaT.L2Norm() < 0.01f)
        {
            found = true;
        }

        var newQ = q + invJ * deltaT;

        return newQ;
    }

}
