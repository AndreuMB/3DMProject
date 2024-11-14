using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DronData
{
    public string origin;
    public string destination;
    public GameMaterial material;
    public Vector3 movingTo;
    public Vector3 dronPosition;
    public Dron dronRef;

    public DronData(string origin, string destination, GameMaterial material, Dron dron)
    {
        this.origin = origin;
        this.destination = destination;
        this.material = material;
        dronRef = dron;
    }
}
