using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XRAssembly;
using static Calculations;
using Vuforia;

public class XRAssemblyLoader : MonoBehaviour
{
    public GameObject meshGenerator;
    public GameObject progressManager;
    public UnityEngine.Quaternion frameQuaternion;
    public Vector3 transformVector;
    public Vector3 originDifference;

    public bool assemblyLoaded;
    public int scalingFactor;
    public int maxIntKeys;

    public Dictionary<string, XRAssembly.Node> assemblyNodes;
    public IDictionary<string, GameObject> assemblyMeshes = new Dictionary<string, GameObject>();
    public Dictionary<string, Dictionary<string, XRAssembly.Adjacency>> assemblyAdjacency;
    public Dictionary<string, Dictionary<string, XRAssembly.Adjacency>> assemblyEdge;

    // Generate the Assembly Model using the Collective Assembly.Assembly class.
    public void generateAssembly(string fileText)
    {
        var assembly = XRAssembly.Assembly.FromJson(fileText);
        var assemblyData = assembly;

        assemblyNodes = assembly.Node;
        assemblyAdjacency = assembly.Adjacency;
        assemblyEdge = assembly.Edge;

        maxIntKeys = (int)assembly.MaxNode;

        Vector3 originFrame = new Vector3(
        (float)-assemblyData.Node[(assemblyData.Node.Count - 1).ToString()].Element.Frame.Point[1],
        (float)assemblyData.Node[(assemblyData.Node.Count - 1).ToString()].Element.Frame.Point[2],
        (float)assemblyData.Node[(assemblyData.Node.Count - 1).ToString()].Element.Frame.Point[0]);

        originDifference = new Vector3(0, originFrame.y / 4, 0);

        // Get the node data for each of the keys in Assembly data & generate meshes.
        foreach (var key in assemblyData.Node.Keys)
        {
            var node = assemblyData.Node[key];

            Vector3 pos = new Vector3((float)-node.Element.Frame.Point[1],
                (float)node.Element.Frame.Point[2],
                (float)node.Element.Frame.Point[0]);

            Vector3 ownXAxis = new Vector3((float)node.Element.Frame.Xaxis[0],
                (float)node.Element.Frame.Xaxis[1],
                (float)node.Element.Frame.Xaxis[2]);

            Vector3 ownYAxis = new Vector3((float)node.Element.Frame.Yaxis[0],
                (float)node.Element.Frame.Yaxis[1],
                (float)node.Element.Frame.Yaxis[2]);

            UnityEngine.Quaternion ownQuaternion = QuaternionFromMatrix3x3(MatrixFromBasisVectors(ownXAxis, ownYAxis));

            GameObject newPivot = new GameObject("Frame" + key);
            newPivot.transform.parent = transform;

            // Generate meshes using the Assembly class node data and keys.
            GameObject newMesh = Instantiate(meshGenerator, Vector3.zero, UnityEngine.Quaternion.identity, newPivot.transform) as GameObject;
            newPivot.SetActive(true);
            newMesh.name = "element" + key;
            assemblyMeshes.Add(key, newMesh);
            newMesh.GetComponent<MeshGenerator>().CreateMesh(node, key);

            // Draw outlines for each element.
            var outline = newMesh.AddComponent<Outline>();
            outline.OutlineMode = Outline.Mode.OutlineAll;
            outline.OutlineColor = Color.white;
            outline.OutlineWidth = 5f;

        }

        // Start the Progress Manager.
        progressManager.GetComponent<ProgressManager>().Restart();

        assemblyLoaded = true;

        TrackerManager.Instance.GetTracker<ObjectTracker>().Start();
    }

}
