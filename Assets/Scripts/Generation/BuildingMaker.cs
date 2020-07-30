using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

[RequireComponent(typeof(MapReader))]
class BuildingMaker : InfrastructureBehaviour
{

    public GameObject[] stonewall;

    public GameObject woodenfloor;

    public Material wood;

    private float getRandomMathSign(float number)
    {
        if (UnityEngine.Random.value > 0.5f)
        {
            return number;
        } else
        {
            return -number;
        }

    }

    private IEnumerator Start()
    {
        while (!map.IsReady)
        {
            yield return null;
        }
        float xCentre = map.bounds.Centre.x;
        float yCentre = map.bounds.Centre.z;
        GameObject buildings = new GameObject();
        buildings.transform.name = "Buildings";
        buildings.transform.parent = map.transform;
        foreach (var way in map.ways.FindAll((w) => { return w.IsBuilding && w.NodeIDs.Count > 1; }))
        {

            GameObject building = new GameObject();
            Vector3 localOrigin = GetCentre(way);
            building.transform.position = localOrigin;
            building.transform.parent = buildings.transform;
            building.transform.name = way.Name;
            List<Vector3> vertices = new List<Vector3>();
            float roadDist = 100f;
            Vector3 doorv3 = new Vector3(0f, 100f, 0f);
            // checking the nearest streetnode
            for (int m = 1; m < way.NodeIDs.Count; m++)
            {
                foreach (var road in map.ways.FindAll((w) => { return w.IsRoad; }))
                {
                    for (int l = 1; l < road.NodeIDs.Count; l++)
                    {
                        OsmNode roadp1 = map.nodes[road.NodeIDs[l - 1]];
                        double roadx = (map.nodes[way.NodeIDs[m]].X + map.nodes[way.NodeIDs[m - 1]].X) / 2 - xCentre;
                        double roady = (map.nodes[way.NodeIDs[m]].Y + map.nodes[way.NodeIDs[m - 1]].Y) / 2 - yCentre;
                        if (Vector3.Distance(new Vector3(Convert.ToSingle(roadx), 0f, Convert.ToSingle(roady)), new Vector3(Convert.ToSingle(roadp1.X) - xCentre, 0f, Convert.ToSingle(roadp1.Y) - yCentre)) < roadDist && 10 < Vector3.Distance(new Vector3(map.nodes[way.NodeIDs[m]].X, 0f, map.nodes[way.NodeIDs[m]].Y), new Vector3(map.nodes[way.NodeIDs[m - 1]].X, 0f, map.nodes[way.NodeIDs[m - 1]].Y)))
                        {
                            roadDist = Vector3.Distance(new Vector3(Convert.ToSingle(roadx), 0f, Convert.ToSingle(roady)), new Vector3(Convert.ToSingle(roadp1.X) - xCentre, 0f, Convert.ToSingle(roadp1.Y) - yCentre));
                            doorv3 = new Vector3(Convert.ToSingle(roadx), 0f, Convert.ToSingle(roady));
                        }
                    }
                }
            }
            // create first layer walls
            for (int i = 1; i < way.NodeIDs.Count; i++)
            {
                GameObject point = new GameObject();
                point.transform.parent = building.transform;
                OsmNode p1 = map.nodes[way.NodeIDs[i - 1]];
                OsmNode p2 = map.nodes[way.NodeIDs[i]];
                Vector3 v1 = new Vector3(p1.X, 0f, p1.Y);
                Vector3 v2 = new Vector3(p2.X, 0f, p2.Y);

                vertices.Add(new Vector3(p1.X - xCentre, 0f, p1.Y - yCentre));
                point.transform.name = $"point {i - 1}";

                point.transform.position = new Vector3(map.nodes[way.NodeIDs[i - 1]].X - xCentre, 0f, map.nodes[way.NodeIDs[i - 1]].Y - yCentre);

                double x = (p1.X + p2.X) / 2 - map.bounds.Centre.x;
                double y = (p1.Y + p2.Y) / 2 - map.bounds.Centre.z;
                Vector3 v3 = v2 - v1;

                Quaternion quaternion = Quaternion.FromToRotation(Vector3.right, v3) * Quaternion.Euler(0f, 0f, 90f);
                bool isOnLine = (doorv3.x - (p1.X - xCentre)) / ((p2.X - xCentre) - (p1.X - xCentre)) == (doorv3.z - (p1.Y - yCentre)) / ((p2.Y - yCentre) - (p1.Y - yCentre));
                Vector3 v4 = LerpByDistance(v1, v2, (Vector3.Distance(v1, v2) / 2) + 5f);
                Vector3 v5 = LerpByDistance(v1, v2, (Vector3.Distance(v1, v2) / 2) - 5f);
                float dist = Vector3.Distance(v1, v2);
                float dist2 = Vector3.Distance(v2, v4);
                float dist3 = Vector3.Distance(v1, v5);
                int j = (int)dist * 44 < 445 ? (int)dist * 44 : 444;
                int n = (int)dist2 * 44 < 445 ? (int)dist2 * 44 : 444;
                int o = (int)dist3 * 44 < 445 ? (int)dist3 * 44 : 444;
                int wallCount = (int)dist / 10;
                if (!isOnLine)
                {
                    if (wallCount >= 1)
                    {
                        GameObject wall = Instantiate(stonewall[j], LerpByDistance(v1, v2, 5) - map.bounds.Centre, quaternion);
                        wall.transform.parent = building.transform;
                        for (int k = 1; k < wallCount; k++)
                        {
                            GameObject wall2 = Instantiate(stonewall[j], LerpByDistance(v1, v2, k * 10 + 5) - map.bounds.Centre, quaternion);
                            wall2.transform.parent = building.transform;
                        }
                    }
                    else if (wallCount == 0)
                    {
                        GameObject wall = Instantiate(stonewall[j], new Vector3(Convert.ToSingle(x), 0f, Convert.ToSingle(y)), quaternion);
                        wall.transform.parent = building.transform;
                    }
                    if (!(dist % 10 == 0) && wallCount >= 1)
                    {
                        j = ((int)dist - wallCount * 10) * 44 < 445 ? ((int)dist - wallCount * 10) * 44 : 444;
                        GameObject wall = Instantiate(stonewall[j], LerpByDistance(v1, v2, (wallCount * 10) + (j != 0 ? (45f / 4430f) * j + 0.5f - (45f / 4430f) : 0.5f)) - map.bounds.Centre, quaternion);
                        wall.transform.parent = building.transform;
                    }
                }
                else
                {
                    double x2 = (v1.x + v5.x) / 2 - xCentre;
                    double x3 = (v4.x + v2.x) / 2 - xCentre;
                    double y2 = (v1.z + v5.z) / 2 - yCentre;
                    double y3 = (v2.z + v4.z) / 2 - yCentre;
                    //GameObject go = new GameObject("go");
                    //go.transform.position = new Vector3(v4.x - xCentre, 0f, v4.z - yCentre);
                    //go.transform.parent = building.transform;
                    //GameObject go2 = new GameObject("go2");
                    //go2.transform.position = new Vector3(v5.x - xCentre, 0f, v5.z - yCentre);
                    //go2.transform.parent = building.transform;
                    if (Vector3.Distance(v1, v5) >= 10 || Vector3.Distance(v4, v2) >= 10)
                    {
                        if (Vector3.Distance(v1, v5) >= 10)
                        {
                            GameObject wall = Instantiate(stonewall[444], new Vector3(Convert.ToSingle(x2), 0f, Convert.ToSingle(y2)), quaternion);
                            wall.transform.parent = building.transform;
                            for (int k = 1; k < (Vector3.Distance(v1, v5) / 10); k++)
                            {
                                GameObject wall2 = Instantiate(stonewall[444], LerpByDistance(v1, v5, k * 10 + 2.5f) - map.bounds.Centre, quaternion);
                                wall2.transform.parent = building.transform;
                            }
                        }
                        if (Vector3.Distance(v2, v4) >= 10)
                        {
                            GameObject wall = Instantiate(stonewall[444], new Vector3(Convert.ToSingle(x3), 0f, Convert.ToSingle(y3)), quaternion);
                            wall.transform.parent = building.transform;
                            for (int k = 1; k < (Vector3.Distance(v4, v2) / 10); k++)
                            {
                                GameObject wall2 = Instantiate(stonewall[444], LerpByDistance(v2, v4, k * 10 + 2.5f) - map.bounds.Centre, quaternion);
                                wall2.transform.parent = building.transform;
                            }
                        }

                    }
                    if (Vector3.Distance(v2, v4) < 10 || Vector3.Distance(v1, v5) < 10)
                    {

                        GameObject wall = Instantiate(stonewall[n], new Vector3(Convert.ToSingle(x2), 0f, Convert.ToSingle(y2)), quaternion);
                        wall.transform.parent = building.transform;
                        GameObject wall2 = Instantiate(stonewall[o], new Vector3(Convert.ToSingle(x3), 0f, Convert.ToSingle(y3)), quaternion);
                        wall2.transform.parent = building.transform;

                    }
                    else if ((!(dist2 % 10 == 0) && Vector3.Distance(v2, v4) >= 10) || (!(dist3 % 10 == 0) && Vector3.Distance(v2, v5) >= 10))
                    {

                        n = ((int)dist2 - (int)(Vector3.Distance(v2, v4) / 10) * 10) * 44 < 445 ? ((int)dist2 - (int)(Vector3.Distance(v1, v5) / 10) * 10) * 44 : 444;
                        GameObject wall = Instantiate(stonewall[n], LerpByDistance(v2, v4, (((int)(Vector3.Distance(v1, v5) / 2) / 10) * 10) + (n != 0 ? (45f / 4430f) * n + 0.5f - (45f / 4430f) : 0.5f)) - map.bounds.Centre, quaternion);
                        wall.transform.parent = building.transform;
                        o = ((int)dist3 - (int)(Vector3.Distance(v1, v5) / 10) * 10) * 44 < 445 ? ((int)dist3 - (int)(Vector3.Distance(v1, v5) / 10) * 10) * 44 : 444;
                        GameObject wall2 = Instantiate(stonewall[o], LerpByDistance(v1, v5, (((int)(Vector3.Distance(v1, v5) / 2) / 10) * 10) + (o != 0 ? (45f / 4430f) * o + 0.5f - (45f / 4430f) : 0.5f)) - map.bounds.Centre, quaternion);
                        wall2.transform.parent = building.transform;
                    }
                }
            }
            // TODO: implement windows
            // create other layers
           
            for (int u = 1; u < (way.Height * 3); u++)
            {
                for (int i = 1; i < way.NodeIDs.Count; i++)
                {
                    float height = u * 0.7f;
                    OsmNode p1 = map.nodes[way.NodeIDs[i - 1]];
                    OsmNode p2 = map.nodes[way.NodeIDs[i]];
                    Vector3 v1 = new Vector3(p1.X, height, p1.Y);
                    Vector3 v2 = new Vector3(p2.X, height, p2.Y);
                    double x = (p1.X + p2.X) / 2 - map.bounds.Centre.x;
                    double y = (p1.Y + p2.Y) / 2 - map.bounds.Centre.z;
                    Vector3 v3 = v2 - v1;

                    Quaternion quaternion = Quaternion.FromToRotation(Vector3.right, v3) * Quaternion.Euler(u * 90f, 0f, 90f);
                    bool isOnLine = (doorv3.x - (p1.X - xCentre)) / ((p2.X - xCentre) - (p1.X - xCentre)) == (doorv3.z - (p1.Y - yCentre)) / ((p2.Y - yCentre) - (p1.Y - yCentre));
                    Vector3 v4 = LerpByDistance(v1, v2, (Vector3.Distance(v1, v2) / 2) + 5f);
                    Vector3 v5 = LerpByDistance(v1, v2, (Vector3.Distance(v1, v2) / 2) - 5f);
                    float dist = Vector3.Distance(v1, v2);
                    float dist2 = Vector3.Distance(v2, v4);
                    float dist3 = Vector3.Distance(v1, v5);
                    int j = (int)dist * 44 < 445 ? (int)dist * 44 : 444;
                    int n = (int)dist2 * 44 < 445 ? (int)dist2 * 44 : 444;
                    int o = (int)dist3 * 44 < 445 ? (int)dist3 * 44 : 444;
                    int wallCount = (int)dist / 10;
                    
                    if (!isOnLine)
                    {
                        if (wallCount >= 1)
                        {
                            GameObject wall = Instantiate(stonewall[j], LerpByDistance(v1, v2, 5) - map.bounds.Centre + new Vector3(0, height, 0), quaternion);
                            wall.transform.parent = building.transform;
                            Debug.Log(wall.transform.rotation.x != 0.0f);
                            wall.transform.localScale = new Vector3(getRandomMathSign(quaternion.x != 0.0f ? 125f : 100f), getRandomMathSign(100f), getRandomMathSign(quaternion.x != 0.0f ? 100f : 125f));
                            for (int k = 1; k < wallCount; k++)
                            {
                                GameObject wall2 = Instantiate(stonewall[j], LerpByDistance(v1, v2, k * 10 + 5) - map.bounds.Centre + new Vector3(0, height, 0), quaternion);
                                wall2.transform.parent = building.transform;
                                wall2.transform.localScale = new Vector3(getRandomMathSign(100f), getRandomMathSign(100f), getRandomMathSign(100f));
                            }
                        }
                        else if (wallCount == 0)
                        {
                            if (!(j < 222) || ((j < 222) && (u % 2 == 0)))
                            {
                                GameObject wall = Instantiate(stonewall[j], new Vector3(Convert.ToSingle(x), height, Convert.ToSingle(y)), quaternion);
                                wall.transform.parent = building.transform;
                                wall.transform.localScale = new Vector3(getRandomMathSign(100f), getRandomMathSign(100f), getRandomMathSign(100f));
                            }
                        }
                        if (!(dist % 10 == 0) && wallCount >= 1)
                        {
                            j = ((int)dist - wallCount * 10) * 44 < 445 ? ((int)dist - wallCount * 10) * 44 : 444;
                            if (!(j < 222) || ((j < 222) && (u % 2 == 0)))
                            {
                                GameObject wall = Instantiate(stonewall[j], LerpByDistance(v1, v2, (wallCount * 10) + (j != 0 ? (45f / 4430f) * j + 0.5f - (45f / 4430f) : 0.5f)) - map.bounds.Centre + new Vector3(0, height, 0), quaternion);
                                wall.transform.parent = building.transform;
                                wall.transform.localScale = new Vector3(getRandomMathSign(100f), getRandomMathSign(100f), getRandomMathSign(100f));
                            }
                        }
                    }
                    else
                    {
                        double x2 = (v1.x + v5.x) / 2 - xCentre;
                        double x3 = (v4.x + v2.x) / 2 - xCentre;
                        double y2 = (v1.z + v5.z) / 2 - yCentre;
                        double y3 = (v2.z + v4.z) / 2 - yCentre;
                        if (Vector3.Distance(v1, v5) >= 10 || Vector3.Distance(v4, v2) >= 10)
                        {
                            if (Vector3.Distance(v1, v5) >= 10)
                            {
                                GameObject wall = Instantiate(stonewall[444], new Vector3(Convert.ToSingle(x2), height, Convert.ToSingle(y2)), quaternion);
                                wall.transform.parent = building.transform;
                                wall.transform.localScale = new Vector3(getRandomMathSign(100f), getRandomMathSign(100f), getRandomMathSign(100f));
                                for (int k = 1; k < (Vector3.Distance(v1, v5) / 10); k++)
                                {
                                    GameObject wall2 = Instantiate(stonewall[444], LerpByDistance(v1, v5, k * 10 + 2.5f) - map.bounds.Centre + new Vector3(0, height, 0), quaternion);
                                    wall2.transform.parent = building.transform;
                                    wall2.transform.localScale = new Vector3(getRandomMathSign(100f), getRandomMathSign(100f), getRandomMathSign(100f));
                                }
                            }
                            if (Vector3.Distance(v2, v4) >= 10)
                            {
                                GameObject wall = Instantiate(stonewall[444], new Vector3(Convert.ToSingle(x3), height, Convert.ToSingle(y3)), quaternion);
                                wall.transform.parent = building.transform;
                                wall.transform.localScale = new Vector3(getRandomMathSign(100f), getRandomMathSign(100f), getRandomMathSign(100f));
                                for (int k = 1; k < (Vector3.Distance(v4, v2) / 10); k++)
                                {
                                    GameObject wall2 = Instantiate(stonewall[444], LerpByDistance(v2, v4, k * 10 + 2.5f) - map.bounds.Centre + new Vector3(0, height, 0), quaternion);
                                    wall2.transform.parent = building.transform;
                                    wall2.transform.localScale = new Vector3(getRandomMathSign(100f), getRandomMathSign(100f), getRandomMathSign(100f));
                                }
                            }

                        }
                        if (Vector3.Distance(v2, v4) < 10 || Vector3.Distance(v1, v5) < 10)
                        {
                            if (!(n < 222) || ((n < 222) && (u % 2 == 0)))
                            {
                                GameObject wall = Instantiate(stonewall[n], new Vector3(Convert.ToSingle(x2), height, Convert.ToSingle(y2)), quaternion);
                                wall.transform.parent = building.transform;
                                wall.transform.localScale = new Vector3(getRandomMathSign(100f), getRandomMathSign(100f), getRandomMathSign(100f));
                            }
                            if (!(o < 222) || ((o < 222) && (u % 2 == 0)))
                            {
                                GameObject wall2 = Instantiate(stonewall[o], new Vector3(Convert.ToSingle(x3), height, Convert.ToSingle(y3)), quaternion);
                                wall2.transform.parent = building.transform;
                                wall2.transform.localScale = new Vector3(getRandomMathSign(100f), getRandomMathSign(100f), getRandomMathSign(100f));

                            }
                        }
                        else if ((!(dist2 % 10 == 0) && Vector3.Distance(v2, v4) >= 10) || (!(dist3 % 10 == 0) && Vector3.Distance(v2, v5) >= 10))
                        {
                            if (!(n < 222) || ((n < 222) && (u % 2 == 0)))
                            {
                                n = ((int)dist2 - (int)(Vector3.Distance(v2, v4) / 10) * 10) * 44 < 445 ? ((int)dist2 - (int)(Vector3.Distance(v1, v5) / 10) * 10) * 44 : 444;
                                GameObject wall = Instantiate(stonewall[n], LerpByDistance(v2, v4, (((int)(Vector3.Distance(v1, v5) / 2) / 10) * 10) + (n != 0 ? (45f / 4430f) * n + 0.5f - (45f / 4430f) : 0.5f)) - map.bounds.Centre + new Vector3(0, height, 0), quaternion);
                                wall.transform.parent = building.transform;
                                wall.transform.localScale = new Vector3(getRandomMathSign(100f), getRandomMathSign(100f), getRandomMathSign(100f));
                            }
                            if (!(o < 222) || ((o < 222) && (u % 2 == 0)))
                            {
                                o = ((int)dist3 - (int)(Vector3.Distance(v1, v5) / 10) * 10) * 44 < 445 ? ((int)dist3 - (int)(Vector3.Distance(v1, v5) / 10) * 10) * 44 : 444;
                                GameObject wall2 = Instantiate(stonewall[o], LerpByDistance(v1, v5, (((int)(Vector3.Distance(v1, v5) / 2) / 10) * 10) + (o != 0 ? (45f / 4430f) * o + 0.5f - (45f / 4430f) : 0.5f)) - map.bounds.Centre + new Vector3(0, height, 0), quaternion);
                                wall2.transform.parent = building.transform;
                                wall2.transform.localScale = new Vector3(getRandomMathSign(100f), getRandomMathSign(100f), getRandomMathSign(100f));
                            }
                        }
                    }
                }
            }
            GameObject door = new GameObject();
            door.transform.name = "door";
            door.transform.parent = building.transform;
            door.transform.position = doorv3;
            GameObject floor = new CustomShape().createMesh(vertices, "floor", wood);
            floor.transform.parent = building.transform;

            yield return null;
        }
    }
}


    


