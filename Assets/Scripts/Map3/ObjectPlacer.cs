using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ObjectPlacer : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> placedGameObject = new();
    [SerializeField] GameObject buildingPrefab;
    Player player;

    void Start(){
        player = FindObjectOfType<Player>();
    }

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
        newParent.transform.position = position;
        newObject.transform.localPosition = new Vector3(0.5f,0,0.5f);
        building.SetBuildType(bType);
        placedGameObject.Add(newObject);
        if (player) player.SetActiveGO(newObject);
        return (placedGameObject.Count - 1, newObject);
        // return newObject;
    }

    internal void RemoveObjectAt(int gameObjectIndex)
    {
        if(placedGameObject.Count <= gameObjectIndex || placedGameObject[gameObjectIndex] == null)
        {
            return;
        }
        Destroy(placedGameObject[gameObjectIndex]);
        placedGameObject[gameObjectIndex] = null;
    }
}
