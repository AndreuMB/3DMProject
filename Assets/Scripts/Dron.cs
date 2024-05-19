using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class Dron : MonoBehaviour
{
    public GameObject origin;
    public GameObject destination;
    public Resource resource;
    public Resource newResource;
    // public Coroutine coroutine;
    public Vector3 movingTo;
    public float speed;
    public DronData dronData;
    public UnityEvent dronGoal = new UnityEvent();
    public GameObject row;
    public bool detele;
    Vector3 destinationV;
    public Vector3 originV;
    
    public void SetData(GameObject origin, GameObject destination, Resource resource, Vector3 movingTo){
        this.origin = origin;
        this.destination = destination;
        this.resource = resource;
        this.movingTo = movingTo;
        this.speed = FindObjectOfType<Player>().dronSpeed;


        // destinationV = new(destination.transform.position.x,MathF.Floor(destination.transform.position.y),destination.transform.position.z);
        // originV = new(origin.transform.position.x,MathF.Floor(origin.transform.position.y),origin.transform.position.z);
        destinationV = destination.transform.position;
        originV = origin.transform.position;
    }

    public void CreateData(){
        dronData = new(origin.name,destination.name,resource,this,movingTo);
    }

    public float GetDistance(){
        return Vector2.Distance(transform.position,movingTo);
    }

    void FixedUpdate(){
        float step = speed * Time.deltaTime;
        if (!destination) return;
        // print("origin = " + originV);
        // print("destination = " + destinationV);
        // print("dron = " + transform.position);
        // print("movingto = " + movingTo);

        float distanceX = destinationV.x - transform.position.x;
        float distanceZ = destinationV.z - transform.position.z;
        const float RANGE = 0.5f;
        
        if (Math.Abs(distanceX) <= RANGE && Math.Abs(distanceZ) <= RANGE && movingTo == destinationV) {
            dronGoal.Invoke();
            movingTo = originV;
            dronData.movingTo = originV;
            transform.LookAt(movingTo);
        }
        
        // if (originV == transform.position){
        distanceX = originV.x - transform.position.x;
        distanceZ = originV.z - transform.position.z;
        if (Math.Abs(distanceX) <= RANGE && Math.Abs(distanceZ) <= RANGE  && movingTo == originV){
            dronGoal.Invoke();
            movingTo = destinationV;
            dronData.movingTo = destinationV;
            transform.LookAt(movingTo);
        }

        GetComponent<Rigidbody>().MovePosition(Vector3.MoveTowards(transform.position, movingTo, step));
        // transform.position = Vector3.MoveTowards(transform.position, movingTo, step);
        dronData.dronPosition = transform.position;
    }
}
