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
    
    }
}
