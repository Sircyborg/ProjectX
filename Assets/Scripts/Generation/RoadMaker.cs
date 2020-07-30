using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


class RoadMaker : InfrastructureBehaviour
{
    public GameObject road;

    private IEnumerator Start()
    {
        while (!map.IsReady)
        {
            yield return null;
        }
        GameObject roads = new GameObject();
        roads.transform.name = "Roads";
        roads.transform.parent = map.transform;
        // TODO: Process map data to create roads
        foreach (var way in map.ways.FindAll((w) => { return w.IsRoad; }))
        {
            GameObject roadGO = new GameObject();
            Vector3 localOrigin = GetCentre(way);
            roadGO.transform.position = localOrigin;
            roadGO.transform.parent = roads.transform;
            roadGO.transform.name = way.Name;
            for (int i = 1; i < way.NodeIDs.Count; i++)
            {
                OsmNode p1 = map.nodes[way.NodeIDs[i - 1]];
                OsmNode p2 = map.nodes[way.NodeIDs[i]];
                double x = (p1.X + p2.X) / 2 - map.bounds.Centre.x;
                double y = (p1.Y + p2.Y) / 2 - map.bounds.Centre.z;
                Vector3 v3 = new Vector3(p2.X, 0f, p2.Y) - new Vector3(p1.X, 0f, p1.Y);
                GameObject waypoint = Instantiate(road, new Vector3(Convert.ToSingle(x), 0f, Convert.ToSingle(y)), Quaternion.FromToRotation(Vector3.left, v3));
                float dist = Vector3.Distance(map.nodes[way.NodeIDs[i]], map.nodes[way.NodeIDs[i - 1]]);
                Destroy(waypoint.transform.GetChild(0).gameObject);
                Destroy(waypoint.transform.GetChild(2).gameObject);
                //waypoint.transform.LookAt(map.nodes[way.NodeIDs[i - 1]]);
                waypoint.transform.localScale = new Vector3(dist / 2, 0f, 1f);
                waypoint.transform.parent = roadGO.transform;
            }

            yield return null;

        }
    }
}

