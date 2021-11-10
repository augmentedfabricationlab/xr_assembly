using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Calculations : MonoBehaviour
{
    public Vector3 robotPos = new Vector3(0,0,0);

    public static float[][] MatrixFromBasisVectors(Vector3 XAxis, Vector3 YAxis)
    {
        float[][] matrix = new float[3][];
        matrix[0] = new float[3];
        matrix[1] = new float[3];
        matrix[2] = new float[3];

        XAxis = XAxis.normalized;
        YAxis = YAxis.normalized;

        Vector3 ZAxis = Vector3.Cross(XAxis, YAxis);
        YAxis = Vector3.Cross(ZAxis, XAxis);

        matrix[0][0] = XAxis.x; matrix[1][0] = XAxis.y; matrix[2][0] = XAxis.z;
        matrix[0][1] = YAxis.x; matrix[1][1] = YAxis.y; matrix[2][1] = YAxis.z;
        matrix[0][2] = ZAxis.x; matrix[1][2] = ZAxis.y; matrix[2][2] = ZAxis.z;


        return matrix;
    }

    public static Quaternion QuaternionFromMatrix3x3(float[][] M)
    {

        Quaternion q = new Quaternion(0,0,0,0);
        float trace = M[0][0] + M[1][1] + M[2][2];

        if(trace > 0.0)
        {
            float s = 0.5f / Mathf.Sqrt(trace + 1);
            q.w = 0.25f / s;
            q.x = (M[2][1] - M[1][2]) * s;
            q.y = (M[0][2] - M[2][0]) * s;
            q.z = (M[1][0] - M[0][1]) * s;
        }
        else if (M[0][0] > M[1][1] && M[0][0] > M[2][2])
        {
            float s = 2.0f * Mathf.Sqrt(1.0f + M[0][0] - M[1][1] - M[2][2]);
            q.w = (M[2][1] - M[1][2]) / s;
            q.x = 0.25f * s;
            q.y = (M[0][1] + M[1][0]) / s;
            q.z = (M[0][2] + M[2][0]) / s;
        }
        else if (M[1][1] > M[2][2])
        {
            float s = 2.0f * Mathf.Sqrt(1.0f + M[1][1] - M[0][0] - M[2][2]);
            q.w = (M[0][2] - M[2][0]) / s;
            q.x = (M[0][1] + M[1][0]) / s;
            q.y = 0.25f * s;
            q.z = (M[1][2] + M[2][1]) / s;
        }
        else
        {
            float s = 2.0f * Mathf.Sqrt(1.0f + M[2][2] - M[0][0] - M[1][1]);
            q.w = (M[1][0] - M[0][1]) / s;
            q.x = (M[0][2] + M[2][0]) / s;
            q.y = (M[1][2] + M[2][1]) / s;
            q.z = 0.25f * s;
        }

        Quaternion finalQuaternion = new Quaternion(0,0,0,0);

        finalQuaternion.x = -1 * q.y;
        finalQuaternion.y = q.z;
        finalQuaternion.z = -1* q.x;
        finalQuaternion.w = q.w;

        return finalQuaternion;
    }

    

    //move according to robot base frame
    public static Vector3 transformPosition(Vector3 elementPos, Vector3 robotPos)
    {
        /*Vector3 helpVector = new Vector3(robotPos.x - 0.4f, robotPos.y, robotPos .z + 0.4f);

        Vector3 newPos = new Vector3(
            elementPos.x - helpVector.x,
            elementPos.y - helpVector.y,
            elementPos.z - helpVector.z);

        return newPos;*/

        return elementPos;
    }

    //transform frame of bricks from middlepoint to top
    public static Vector3 transformFrame(Vector3 elementPos, Vector3 originElement)
    {
        float difference = (originElement.y) / 2;
        Vector3 newFrame = new Vector3(elementPos.x, elementPos.y - difference, elementPos.z);
        return newFrame;
    }
}
