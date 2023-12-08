using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DronData
{
    public string origin;
    public string destination;
    public Resource resource;
    public Vector3 movingTo;
    public Vector3 dronPosition;
    public Dron dronRef;

    public DronData(string origin, string destination, Resource resource, Dron dron, Vector3 movingTo){
        this.origin = origin;
        this.destination = destination;
        this.resource = resource;
        this.movingTo = movingTo;
        dronRef = dron;
    }
}
