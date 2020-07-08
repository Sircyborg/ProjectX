using UnityEngine;

[RequireComponent(typeof(MapReader))]
abstract class InfrastructureBehaviour : MonoBehaviour
{
    protected MapReader map;

    protected Vector3 LerpByDistance(Vector3 A, Vector3 B, float x)
    {
        float distX = x * Vector3.Normalize(B - A).x + A.x;
        float distY = x * Vector3.Normalize(B - A).z + A.z;
        return new Vector3(distX, 0f, distY);
    }

    private void Awake()
    {
        map = GetComponent<MapReader>();
    }

    protected Vector3 GetCentre(OsmWay way)
    {

        Vector3 total = Vector3.zero;
        if (way.NodeIDs.Count == 0)
        {
            return total;
        }

        foreach (var id in way.NodeIDs)
        {
            total += map.nodes[id] - map.bounds.Centre;
        }

        return total / way.NodeIDs.Count;
    }
}


