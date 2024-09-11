using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ObjectPlacer : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> placedGameObject = new();
    [SerializeField] Mesh cubeMesh;
    [SerializeField] GameObject buildingPrefab;
    [SerializeField] Transform buildingsContainer;
    [SerializeField] Transform oresContainer;

    public int PlaceObject(GameObject prefab, Vector3 position)
    {
        GameObject newObject = Instantiate(prefab);
        newObject.transform.position = position;
        placedGameObject.Add(newObject);
        return placedGameObject.Count - 1;
    }

    public (int, GameObject) PlaceBuild(GameObject prefab, Vector3 position, BuildingsEnum bType)
    {
        GameObject newObject = Instantiate(buildingPrefab);
        Building building = newObject.GetComponent<Building>();
        // if (building){
        newObject.name = bType.ToString() + placedGameObject.Count;
        // print();
        if (prefab) building.SetModel(prefab);
        // }
        GameObject newParent = new GameObject();
        newObject.transform.parent = newParent.transform;
        newParent.transform.parent = buildingsContainer;

        newParent.transform.position = transform.InverseTransformPoint(position);
        FixYAxyGO(newParent.transform);

        building.SetBuildType(bType);
        placedGameObject.Add(newObject);
        // if (player) player.SetActiveGO(newObject);
        return (placedGameObject.Count - 1, newObject);
        // return newObject;
    }

    public (int, GameObject) PlaceOre(GameObject oreGO, Vector3 position)
    {
        oreGO.transform.localScale = new Vector3(.2f, .2f, .2f);

        GameObject newParent = new GameObject();
        oreGO.transform.parent = newParent.transform;
        newParent.transform.position = position;
        newParent.transform.parent = oresContainer;

        newParent.transform.position = transform.InverseTransformPoint(position);
        FixYAxyGO(newParent.transform);

        newParent.tag = Tags.Ore.ToString();
        newParent.AddComponent<MeshCollider>();
        // put mesh collider to this parent or delete parent in code and create it already on the prefab with the mesh
        newParent.GetComponent<MeshCollider>().sharedMesh = cubeMesh;

        placedGameObject.Add(oreGO);

        return (placedGameObject.Count - 1, oreGO);
    }

    internal void RemoveObjectAt(int gameObjectIndex)
    {
        if (placedGameObject.Count <= gameObjectIndex || placedGameObject[gameObjectIndex] == null)
        {
            return;
        }
        Destroy(placedGameObject[gameObjectIndex]);
        placedGameObject[gameObjectIndex] = null;
    }

    internal GameObject GetGObjectAt(int gameObjectIndex)
    {
        if (placedGameObject.Count <= gameObjectIndex || placedGameObject[gameObjectIndex] == null)
        {
            return null;
        }
        return placedGameObject[gameObjectIndex];
    }

    void FixYAxyGO(Transform GOtransform)
    {
        const float OFFSET_Y = 10f;
        Vector3 positionFix = GOtransform.position;
        positionFix.y = GOtransform.position.y - OFFSET_Y;
        // Apply the corrected local position
        GOtransform.position = positionFix;
    }
}

public enum Tags
{
    MainBase,
    Building,
    Ore,
}
