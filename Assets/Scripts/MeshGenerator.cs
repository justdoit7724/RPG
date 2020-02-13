using System.Collections.Generic;
using UnityEngine;

public class MeshGenerator
{

    public static Mesh Circle(int sliceCount, float rad)
    {
        float dTheta = (Mathf.PI*2) / sliceCount;

        List<Vector3> vertice=new List<Vector3>();
        List<int> indice=new List<int>();

        vertice.Add(new Vector3(0, 0, 0));
        for (int i = 0; i < sliceCount + 1; ++i)
        {
            float x = rad * Mathf.Cos(i * dTheta);
            float z = rad * Mathf.Sin(i * dTheta);
            float u = x / 2.0f + 0.5f;
            float v = z / 2.0f + 0.5f;

            vertice.Add(new Vector3(x, 0,z));
        }

        for (int i = 1; i < sliceCount; ++i)
        {
            indice.Add(0);
            indice.Add(i + 1);
            indice.Add(i);
        }
        indice.Add(0);
        indice.Add(1);
        indice.Add(sliceCount);

        Mesh circleMesh = new Mesh();
        circleMesh.vertices = vertice.ToArray();
        circleMesh.triangles = indice.ToArray();

        return circleMesh;
    }
}
