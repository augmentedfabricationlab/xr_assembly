using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trajectories : MonoBehaviour
{
    public List<Vector3> trajectories = new List<Vector3>();
    public List<Quaternion> quaternions = new List<Quaternion>();
    public Quaternion originQuaternion = new Quaternion();
    public Vector3 toolFrame = new Vector3();
    public List<Quaternion> toolQuaternion = new List<Quaternion>();
    public List<float[]> jointValues = new List<float[]>();
}
