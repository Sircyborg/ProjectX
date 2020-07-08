using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MapReader))]
class TerrainMaker : InfrastructureBehaviour
{

    public GameObject road;

    private IEnumerator Start()
    {
        while (!map.IsReady)
        {
            yield return null;
        }

        foreach (var way in map.ways.FindAll((w) => { return w.IsRoad; }))
        {
            for (int i = 1; i < way.NodeIDs.Count; i++) 
            {
                double x = (map.nodes[way.NodeIDs[i-1]].X + map.nodes[way.NodeIDs[i]].X) / 2 - map.bounds.Centre.x;
                double y = (map.nodes[way.NodeIDs[i-1]].Y + map.nodes[way.NodeIDs[i]].Y) / 2 - map.bounds.Centre.z;
                GameObject waypoint = Instantiate(road, new Vector3(Convert.ToSingle(x),0f,Convert.ToSingle(y)), Quaternion.identity);
                float dist = Vector3.Distance(map.nodes[way.NodeIDs[i]], map.nodes[way.NodeIDs[i - 1]]);
                Destroy(waypoint.transform.GetChild(0).gameObject);
                Destroy(waypoint.transform.GetChild(2).gameObject);
                waypoint.transform.LookAt(map.nodes[way.NodeIDs[i - 1]]);
                waypoint.transform.localScale = new Vector3(1f, 0f, 1f);
            }
            yield return null;
        }
    }
}
