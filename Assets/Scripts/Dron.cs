using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Dron : MonoBehaviour
{
    public GameObject origin;
    public GameObject destination;
    public Resource resource;
    public Coroutine coroutine;
    public Vector3 movingTo;
    public float speed;
    public DronData dronData;
    
    public void SetData(GameObject origin, GameObject destination, Resource resource, Vector3 movingTo){
        this.origin = origin;
        this.destination = destination;
        this.resource = resource;
        this.movingTo = movingTo;
    }

    public void CreateData(){
        dronData = new(origin.name,destination.name,resource,this,movingTo);
    }

    public float GetDistance(){
        return Vector2.Distance(transform.position,movingTo);
    }

    void Update(){
        float step = speed * Time.deltaTime;
        if (destination.transform.position == transform.position) {
            movingTo = origin.transform.position;
            dronData.movingTo = origin.transform.position;
            transform.LookAt(movingTo);
        }
        if (origin.transform.position == transform.position){
            movingTo = destination.transform.position;
            dronData.movingTo = destination.transform.position;
            transform.LookAt(movingTo);
        }

        transform.position = Vector3.MoveTowards(transform.position, movingTo, step);
        dronData.dronPosition = transform.position;
    }
}
