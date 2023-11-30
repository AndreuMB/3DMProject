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

    public int PlaceBuild(GameObject prefab, Vector3 position, BuildingsEnum bType)
    {
        GameObject newObject = Instantiate(buildingPrefab);
        Building building = newObject.GetComponent<Building>();
        // if (building){
        newObject.name = bType.ToString() + new Vector2Int((int) position.x,(int) position.y);
        building.SetBuildType(bType);
        // print();
        if (prefab) building.SetModel(prefab);
        // }
        GameObject newParent = new GameObject();
        newObject.transform.parent = newParent.transform;
        newParent.transform.position = position;
        newObject.transform.localPosition = new Vector3(0.5f,0,0.5f);
        placedGameObject.Add(newObject);
        player.SetActiveGO(newObject);
        return placedGameObject.Count - 1;
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
