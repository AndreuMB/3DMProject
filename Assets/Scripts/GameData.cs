using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;

[System.Serializable]
public class GameData
{
    public int drons;
    public int dronStorage;
    public float dronSpeed;
    // public List<Extractor> extractors;
    public List<BuildingData> buildings;
    public List<OreData> ores;
    public Vector3 cameraPosition; 
    public Quaternion cameraRotation;
    public Vector3 zoom;
    // public Building building;

    public GameData(Player player,List<BuildingData> buildings, Transform cameraTransform, List<OreData> resources){
        drons = player.drons;
        dronStorage = player.dronStorage;
        dronSpeed = player.dronSpeed;
        // building = extractor.data;
        // this.extractors = extractors;
        this.buildings = buildings;
        cameraPosition = cameraTransform.position;
        cameraRotation = cameraTransform.rotation;
        zoom = cameraTransform.GetChild(0).transform.localPosition;
        ores = resources;
    }
}
