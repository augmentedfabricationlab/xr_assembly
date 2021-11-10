using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XRAssembly;
using static Calculations;

public class MeshGenerator : MonoBehaviour
{
    // Generate meshes for each element according to their node data.
    public void CreateMesh(XRAssembly.Node data, string number)
    {
        MeshFilter filter = GetComponent<MeshFilter>();
        UnityEngine.Mesh mesh = filter.mesh;
        mesh.Clear();

        #region Vertices
        
        Vector3[] vertices = new Vector3[data.Element.Mesh.Vertex.Count];

        for (int i=0; i< data.Element.Mesh.Vertex.Count; i++)
        {
            vertices.SetValue(new Vector3((float)-data.Element.Mesh.Vertex[i.ToString()].Y, (float)data.Element.Mesh.Vertex[i.ToString()].Z, (float)data.Element.Mesh.Vertex[i.ToString()].X),i);
        }

        #endregion

        #region Triangles
        int[] triangles = new int[data.Element.Mesh.Face.Values.Count*3];
        int counter = 0;

        foreach (var key in data.Element.Mesh.Face.Keys)
        {
            triangles.SetValue((int)data.Element.Mesh.Face[key][0], counter);
            counter = counter + 1;
            triangles.SetValue((int)data.Element.Mesh.Face[key][2], counter);
            counter = counter + 1;
            triangles.SetValue((int)data.Element.Mesh.Face[key][1], counter);
            counter = counter + 1;
        }

        string text = "";

        if (number == "1")
        {
            for (int i = 0; i < triangles.Length; i++)
            {
                text = text + triangles[i] + ",";
            }
            
        }
        #endregion

        #region Normales
        
        Vector3[] normales = new Vector3[vertices.Length];
        int triangleCount = triangles.Length / 3;

        for(int i = 0; i < triangleCount; i++)
        {
            int normalTriangleIndex = i * 3;
            int vertexIndexA = triangles[normalTriangleIndex];
            int vertexIndexB = triangles[normalTriangleIndex+1];
            int vertexIndexC = triangles[normalTriangleIndex+2];

            Vector3 pointA = vertices[vertexIndexA];
            Vector3 pointB = vertices[vertexIndexB];
            Vector3 pointC = vertices[vertexIndexC];

            Vector3 sideAB = pointB - pointA;
            Vector3 sideAC = pointC - pointA;
            Vector3 triangleNormal = Vector3.Cross(sideAB, sideAC).normalized;

            normales[vertexIndexA] += triangleNormal;
            normales[vertexIndexB] += triangleNormal;
            normales[vertexIndexC] += triangleNormal;
        }

        for (int i = 0; i < normales.Length; i++)
        {
            normales[i].Normalize();
        }

        #endregion

        #region UVs

        #endregion

        mesh.vertices = vertices;
        mesh.normals = normales;
        //mesh.uv = uvs;
        mesh.triangles = triangles;

        mesh.RecalculateBounds();
    }
}


