using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Single;
using MathNet.Numerics.Statistics.Mcmc;
using UnityEditor.Experimental.UIElements;
using UnityEditor.IMGUI.Controls;
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
    private int isAnimated = -1;

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

    class StatisticsTracker
    {
        InitModeEnum mode;

        public StatisticsTracker(InitModeEnum initmode)
        {
            mode = initmode;
        }

        public int totalIterations => data.Sum();

        public List<int> data = new List<int>();

        public void WriteToFile()
        {
            string str = $"Mode: {mode} \n" +
                         $"Total iterations: {totalIterations}\n" +
                         $"data:\n";



            foreach (var d in data)
            {
                str += $"\t{d}";
            }

            Debug.Log("writing to file!");
            File.WriteAllText($"stats for {mode.ToString()}.txt", str);


        }
    }

    //TODO: THIS SHOULD BE SET VARIABLY, it's now set to (3,-2)
    Vector<float> goalPos = Vector<float>.Build.DenseOfArray(new[] { 4f, 2f });
    Vector<float> previousGoalPos = Vector<float>.Build.DenseOfArray(new[] { 4f, 2f });
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

    IEnumerator rotateJoints(int jointNr, float theta1 = 0, float theta2 = 0, float theta3 = 0, int level = 0)
    {
        float timestep = 1f / 60f;

        sliderAngle1.value = theta1 * Mathf.Rad2Deg;
        sliderAngle2.value = theta2 * Mathf.Rad2Deg;
        sliderAngle3.value = theta3 * Mathf.Rad2Deg;

        yield return new WaitForEndOfFrame();



        float min = 01;
        float max = 0;
        Slider slider;
        Transform joint;
        switch (jointNr)
        {
            case 0:
                joint = GameObject.Find("MCP").transform;
                slider = sliderAngle1;
                min = theta_1min;
                max = theta_1max;
                break;
            case 1:
                joint = GameObject.Find("PIP").transform;
                min = theta_2min;
                max = theta_2max;
                slider = sliderAngle2;
                break;
            case 2:
                joint = GameObject.Find("DIP").transform;
                min = theta_3min;
                max = theta_3max;
                slider = sliderAngle3;
                break;

            default:
                joint = GameObject.Find("MCP").transform;
                slider = sliderAngle1;
                min = theta_1min;
                max = theta_1max;
                break;
        }

        var t = GameObject.Find("DIP");
        var verts = new List<Vector3>();

        verts.Add(t.transform.position);

        // joint.eulerAngles = new Vector3(0, 0, min * Mathf.Rad2Deg);
        // yield return new WaitForEndOfFrame();
        for (float t1 = min; t1 <= max; t1 += timestep)
        {
            var degr = t1 * Mathf.Rad2Deg;
            // joint.Rotate(Vector3.forward,t1);
            // var rot = joint.rotation;
            // rot.z = t1;
            //joint.rotation = rot;



            var x = sliderAngle1.value * Mathf.Rad2Deg;
            var y = sliderAngle1.value * Mathf.Rad2Deg;
            var z = sliderAngle1.value * Mathf.Rad2Deg;


            var pos = GameObject.Find("Tip").transform.position;

            //Vector3 p = getTipPos(x, y, z);
            var diff = pos - verts.Last();

            // Debug.DrawLine(verts.Last(), pos, Color.green);
            verts.Add(pos);
            slider.value = degr;

            yield return new WaitForEndOfFrame();
        }

        // Draw a yellow sphere at the transform's position


        for (int i = 0; i < verts.Count - 5; i += 4)
        {
            Debug.DrawLine(verts[i], verts[i + 5], Color.yellow, float.PositiveInfinity, false);
            Debug.Log($"DIFFERENCE: {(verts[i] - verts[i + 5]).magnitude}");

            Gizmos.color = Color.green;
            //Gizmos.DrawSphere(verts[i], 3);

            //var go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            // go.transform.position = verts[i];
            //go.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        }
        //// sliderAngle2.value = theta_2max;
        //for (float t1 = theta_1min; t1 <= theta_1max; t1 += timestep)
        //{
        //    var degr = t1 * Mathf.Rad2Deg;

        //    sliderAngle1.value = degr;

        //    yield return new WaitForEndOfFrame();
        //}


        //for (float t1 = theta_1min; t1 <= theta_1max; t1 += timestep)
        //{
        //    var degr = t1 * Mathf.Rad2Deg;

        //    sliderAngle1.value = degr;

        //    yield return new WaitForEndOfFrame();
        //}

        //sliderAngle2.value = theta_2min;
        //yield return new WaitForEndOfFrame();
        //for (float t1 = theta_1max; t1 >= theta_1min; t1 -= timestep)
        //{
        //    var degr = t1 * Mathf.Rad2Deg;

        //    sliderAngle1.value = degr;

        //    yield return new WaitForEndOfFrame();
        //}

        //for (float t2 = theta_2min; t2 <= theta_2max; t2 += timestep)
        //{
        //    var degr = t2 * Mathf.Rad2Deg;

        //    sliderAngle2.value = degr;
        //    yield return new WaitForEndOfFrame();
        //}
        // yield return null;
        if (level == 0)
        {
            StartCoroutine(rotateJoints(0, theta_1max, theta_2min, theta3, 1));
            // StartCoroutine(rotateJoints(1,theta_1max, theta_2min, theta3, level + 1));
        }

        if (level == 1)
        {
            StartCoroutine(rotateJoints(0, theta_1max, theta_2min, theta_3min, 2));
        }



    }

    Vector3 getTipPos(float x, float y, float z)
    {

        float f1X = L1 * Mathf.Cos(x) +
                    L2 * Mathf.Cos(x + y) +
                    L3 * Mathf.Cos(x + y + z);

        float f2Y = L1 * Mathf.Sin(x) +
                    L2 * Mathf.Sin(x + y) +
                    L3 * Mathf.Sin(x + y + z);

        return new Vector3(f1X, f2Y, 0);

    }


    IEnumerator touchOIllusion()
    {

        var tracker = new StatisticsTracker(InitMode);

        int totalIterations = 0;
        if (!isFixed)
        {
            float x1 = 6.0f;
            float prevTheta1 = 0;
            float prevTheta2 = 0;
            float prevTheta3 = 0;

            int positionIterator = 0;
            while (x1 > 2.0f)
            {
                tracker.data.Add(0);

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
                        found = false;
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

                        tracker.data[positionIterator]++;
                        iterations++;
                        break;
                    }

                    tracker.data[positionIterator]++;
                    iterations++;

                    if (iterations > maxIterations)
                    {
                        break;
                    }

                }

                positionIterator++;
                yield return new WaitForSeconds(0.05f);
                x1 -= 0.1f;

            }

            var totIt = tracker.totalIterations;

            Debug.Log($"Total animation took {totIt} iterations");
            tracker.WriteToFile();
        }


    }

    private Coroutine traceRoutine = null;

    IEnumerator drawTrace()
    {
        var pos = GameObject.Find("Tip").transform.position;
        var s = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        s.transform.position = pos;
        //traceRoutine = null;
        return null;
    }

    private Vector3 LastPos = new Vector3(7.8f,1f,0);

    // Update is called once per frame
    void Update()
    {
        // Debug.DrawLine(new Vector3(0,0,0), Time.realtimeSinceStartup* new Vector3(5,5,0), Color.red);

        isFixed = GameObject.Find("Fixed Ratio Check").GetComponent<Toggle>().isOn;

        if (Input.GetKeyUp(KeyCode.I))
        {
            StartCoroutine(touchOIllusion());

        }

        if (Input.GetKeyUp(KeyCode.Space))
        {
            isAnimated *= -1;

        }

        if (Input.GetKeyUp(KeyCode.R))
        {
            StartCoroutine(TraceReachablePositions());
        }

 
        //if (!Input.GetKeyUp("space")) return;


        var gps = GameObject.Find("Destination").transform.position;
        goalPos = Vector<float>.Build.DenseOfArray(new[] { gps.x, gps.y });

        if (isAnimated == 1 && goalPos != previousGoalPos)
        {
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

            previousGoalPos = goalPos;

        }

        var currentPos = GameObject.Find("Tip").transform.position  ;
        var diffLast = (LastPos-currentPos).magnitude;

        if (diffLast > 0.2f)
        {
            var tracebutton = GameObject.Find("Trace").GetComponent<Toggle>();

            if (tracebutton.isOn)
            {
                Debug.DrawLine(LastPos,currentPos,Color.green,float.PositiveInfinity,false);

            }



            LastPos = currentPos;
        }


    }





    IEnumerator TraceReachablePositions()
    {
        StartCoroutine(rotateJoints(0, 0f, theta_2max, theta_3max));
        yield return new WaitForEndOfFrame();
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
