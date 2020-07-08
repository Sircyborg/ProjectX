using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

[RequireComponent(typeof(MapReader))]
class BuildingMaker : InfrastructureBehaviour
{

    public GameObject[] stonewall;



    private IEnumerator Start()
    {
        while (!map.IsReady)
        {
            yield return null;
        }
        foreach (var way in map.ways.FindAll((w) => { return w.IsBuilding && w.NodeIDs.Count > 1; }))
        {
            
            GameObject building = new GameObject();
            Vector3 localOrigin = GetCentre(way);
            building.transform.position = localOrigin;
            building.transform.parent = map.transform;
            building.transform.name = way.Name;
            for (int i = 1; i < way.NodeIDs.Count; i++)
            {

                OsmNode p1 = map.nodes[way.NodeIDs[i - 1]];
                OsmNode p2 = map.nodes[way.NodeIDs[i]];
                Vector3 v1 = new Vector3(p1.X, 0f, p1.Y);
                Vector3 v2 = new Vector3(p2.X, 0f, p2.Y);
                float dist = Vector3.Distance(v1, v2);
                int j = (int)dist * 44 < 445 ? (int)dist * 44 : 444;
                int wallCount = (int)dist / 10;


                double x = (p1.X + p2.X) / 2 - map.bounds.Centre.x;
                double y = (p1.Y + p2.Y) / 2 - map.bounds.Centre.z;
                Vector3 v3 = v2 - v1;
                Quaternion quaternion = Quaternion.FromToRotation(Vector3.right, v3) * Quaternion.Euler(0f, 0f, 90f);
                if (wallCount >= 1)
                {
                    GameObject wall2 = Instantiate(stonewall[j], LerpByDistance(v1, v2, 5) - map.bounds.Centre, quaternion);
                    wall2.transform.parent = building.transform;
                    for (int k = 1; k < wallCount; k++)
                    {
                        GameObject wall = Instantiate(stonewall[j], LerpByDistance(v1, v2, k * 10 + 5) - map.bounds.Centre, quaternion);
                        wall.transform.parent = building.transform;
                    }
                } else if (wallCount == 0)
                {
                    GameObject wall = Instantiate(stonewall[j], new Vector3(Convert.ToSingle(x), 0f, Convert.ToSingle(y)), quaternion);
                    //wall.transform.position = LerpByDistance(v1, v2, 5) - map.bounds.Centre;
                    wall.transform.parent = building.transform;
                }
                if (!(dist % 10 == 0) & wallCount>=1)
                {

                    j = ((int)dist - wallCount*10) * 44 < 445 ? ((int)dist - wallCount * 10) * 44 : 444;
                    GameObject wall = Instantiate(stonewall[j], LerpByDistance(v1, v2,(wallCount*10) + (j != 0 ? (45f / 4430f) * j + 0.5f - (45f / 4430f): 0.5f)) - map.bounds.Centre, quaternion);
                    wall.transform.parent = building.transform;
                }
                

                /*GameObject wall = Instantiate(stonewall[j], new Vector3(0f, 0f, 0f), Quaternion.FromToRotation(Vector3.right, v3) * Quaternion.Euler(0f, 0f, 90f));
                wall.transform.position = new Vector3(Convert.ToSingle(x), 0f, Convert.ToSingle(y));
                wall.transform.parent = building.transform;*/

                // wall.transform.localScale = new Vector3(100f, dist * 40, 100f);
            }

            yield return null;
        }
    }
}

