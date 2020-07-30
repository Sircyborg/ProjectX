using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;


class CustomShape : MonoBehaviour
{

    public GameObject createMesh(List<Vector3> points, string name = "", Material material = null)
    {
        GameObject go = new GameObject();
        Poly2Mesh.Polygon poly = new Poly2Mesh.Polygon();
        poly.outside = points;
        go.AddComponent<MeshFilter>().mesh = Poly2Mesh.CreateMesh(poly);
        go.GetComponent<MeshFilter>().name = name;
        go.AddComponent<MeshRenderer>().material = material;
        go.transform.position = new Vector3(0f, 0f, 0f);
        return go;
    }
}