using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(HexGridMeshGenerator))]
public class HexGridMeshGeneratorEditor : Editor
{
public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        HexGridMeshGenerator hexGridMeshGenerator = (HexGridMeshGenerator)target;
        if(GUILayout.Button("Genera Hex Mesh"))
        {
            hexGridMeshGenerator.CreateHexMesh();
        }
        if(GUILayout.Button("Elimina Hex Mesh"))
        {
            hexGridMeshGenerator.ClearHexGridMesh();
        }
    }
}
