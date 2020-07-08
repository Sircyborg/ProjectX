using System.Collections;
using System.Collections.Generic;
using UnityEngine;


class RoadMaker : InfrastructureBehaviour
{

    public Material roadMaterial;

    private IEnumerator Start()
    {
        while (!map.IsReady)
        {
            yield return null;
        }

        // TODO: Process map data to create roads

        foreach (var way in map.ways.FindAll((w) => { return w.IsRoad; }))
        {
            GameObject go = new GameObject();
            Vector3 localOrigin = GetCentre(way);
            go.transform.position = localOrigin;
            go.transform.parent = map.transform;
            go.transform.name = way.Name;
            MeshFilter mf = go.AddComponent<MeshFilter>();
            MeshRenderer mr = go.AddComponent<MeshRenderer>();

            mr.material = roadMaterial;

            List<Vector3> vectors = new List<Vector3>();
            List<Vector3> normals = new List<Vector3>();
            List<int> indicies = new List<int>();

            for (int i = 1; i < way.NodeIDs.Count; i++)
            {
                OsmNode p1 = map.nodes[way.NodeIDs[i - 1]];
                OsmNode p2 = map.nodes[way.NodeIDs[i]];

                Vector3 s1 = p1 - localOrigin - map.bounds.Centre;
                Vector3 s2 = p2 - localOrigin - map.bounds.Centre;

                Vector3 diff = (s2 - s1).normalized;
                var cross = Vector3.Cross(diff, Vector3.up) * 5.0f * (way.Lanes > 0 ? way.Lanes : 1f); // 2 metres = width of lane

                Vector3 v1 = s1 + cross;
                Vector3 v2 = s1 - cross;
                Vector3 v3 = s2 + cross;
                Vector3 v4 = s2 - cross;
                vectors.Add(v1);
                vectors.Add(v2);
                vectors.Add(v3);
                vectors.Add(v4);

                normals.Add(Vector3.up);
                normals.Add(Vector3.up);
                normals.Add(Vector3.up);
                normals.Add(Vector3.up);

                int idx1, idx2, idx3, idx4;
                idx4 = vectors.Count - 1;
                idx3 = vectors.Count - 2;
                idx2 = vectors.Count - 3;
                idx1 = vectors.Count - 4;

                // first triangle v1, v3, v2
                indicies.Add(idx1);
                indicies.Add(idx3);
                indicies.Add(idx2);

                // second triangle v3, v4, v2
                indicies.Add(idx3);
                indicies.Add(idx4);
                indicies.Add(idx2);

                // third triangle v2, v3, v1
                indicies.Add(idx2);
                indicies.Add(idx3);
                indicies.Add(idx1);

                // fourth triangle v2, v4, v3
                indicies.Add(idx2);
                indicies.Add(idx4);
                indicies.Add(idx3);

            }
            mf.mesh.vertices = vectors.ToArray();
            mf.mesh.normals = normals.ToArray();
            mf.mesh.triangles = indicies.ToArray();

            yield return null;

        }
    }
}

