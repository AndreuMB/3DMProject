using System.Collections;
using System.Collections.Generic;
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

    public GameData(MainBase mainBase, List<BuildingData> buildings, Transform cameraTransform, List<OreData> resources)
    {
        drons = mainBase.drons;
        dronStorage = mainBase.dronStorage;
        dronSpeed = mainBase.dronSpeed;

        this.buildings = buildings;
        cameraPosition = cameraTransform.position;
        cameraRotation = cameraTransform.rotation;
        zoom = cameraTransform.GetChild(0).transform.localPosition;
        ores = resources;
    }
}
