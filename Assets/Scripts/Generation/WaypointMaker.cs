﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MapReader))]
class WaypointMaker : InfrastructureBehaviour
{
    public GameObject sign;

    private IEnumerator Start()
    {
        while (!map.IsReady)
        {
            yield return null;
        }
        GameObject waypoints = new GameObject();
        waypoints.transform.name = "Waypoints";
        waypoints.transform.parent = map.transform;
        foreach (var node in map.nodes)
        {
            if (node.Value.IsBusstop)
            {
                GameObject waypoint = Instantiate(sign, new Vector3(node.Value.X - map.bounds.Centre.x, 0f, node.Value.Y - map.bounds.Centre.z), Quaternion.identity);
                waypoint.transform.parent = waypoints.transform;
                waypoint.transform.name = node.Value.Name;
                Destroy(waypoint.transform.GetChild(0).gameObject);
                Destroy(waypoint.transform.GetChild(2).gameObject);
            }
            
        }
        
        yield return null;
    }
}
