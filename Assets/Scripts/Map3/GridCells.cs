using System.Collections;
using System.Collections.Generic;
using Unity.Burst.Intrinsics;
using UnityEngine;

public class GridCells : MonoBehaviour
{
    public GameObject gridVisualization;
    // Start is called before the first frame update
    void Start()
    {
        Renderer renderer = gridVisualization.GetComponent<Renderer>();       
        Vector2 surfaceSize = renderer.material.GetVector("_Default");
        Vector2 cellSize = renderer.material.GetVector("_Size");
    }
}
