using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Single;

public class IterAlgo : MonoBehaviour
{
    private float L1, L2, L3;
    // Use this for initialization
    void Start()
    {
        Matrix<float> m = Matrix<float>.Build.Random(3, 4);

    }

    // Update is called once per frame
    void Update()
    {

    }
    Matrix<float> inverseJacobian(float x, float y, float z)
    {
        //x, y , and z are theta 1, 2, and 3 respectively
        float f1_x = -Mathf.Sin(x + y + z) -
                     Mathf.Cos(x + y + z) -
                     L1 * Mathf.Sin(x) -
                     L2 * Mathf.Sin(x + y) -
                     L3 * Mathf.Sin(x + y + z);

        float f1_y = -Mathf.Sin(x + y + z) -
                     Mathf.Cos(x + y + z) -
                     //L1 * Mathf.Sin(x) +
                     L2 * Mathf.Sin(x + y) -
                     L3 * Mathf.Sin(x + y + z);


        float f1_z = -Mathf.Sin(x + y + z) -
                     Mathf.Cos(x + y + z) -
                     //L1 * Mathf.Sin(x) +       0
                     //L2 * Mathf.Sin(x + y) +   0
                     L3 * Mathf.Sin(x + y + z);

        float f2_x = Mathf.Cos(x + y + z) -
                     Mathf.Sin(x + y + z) +
                     L1 * Mathf.Cos(x) +
                     L2 * Mathf.Cos(x + y) +
                     L3 * Mathf.Cos(x + y + z);

        float f2_y = Mathf.Cos(x + y + z) -
                     Mathf.Sin(x + y + z) +
                     // L1 * Mathf.Cos(x) +     0
                     L2 * Mathf.Cos(x + y) +
                     L3 * Mathf.Cos(x + y + z);

        float f2_z = Mathf.Cos(x + y + z) -
                     Mathf.Sin(x + y + z) +
                     // L1 * Mathf.Cos(x) +     0
                     //L2 * Mathf.Cos(x + y) +  0
                     L3 * Mathf.Cos(x + y + z);

        Matrix<float> Jacobian = DenseMatrix.OfArray(new[,] {
            {f1_x, f1_y, f2_z},
            {f2_x, f2_y, f2_z}});

        var invJ = Jacobian.PseudoInverse();


        return invJ;
    }

}
