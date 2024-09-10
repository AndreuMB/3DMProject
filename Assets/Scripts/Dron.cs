using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class Dron : MonoBehaviour
{
    [NonSerialized] public GameObject origin;
    [NonSerialized] public GameObject destination;
    [NonSerialized] public GameMaterial material;
    [NonSerialized] public GameMaterial newMaterial;
    // public Coroutine coroutine;
    [NonSerialized] public Vector3 movingTo;
    [NonSerialized] public float speed;
    [NonSerialized] public DronData dronData;
    [NonSerialized] public UnityEvent dronGoal = new UnityEvent();
    [NonSerialized] public GameObject row;
    [NonSerialized] public bool detele;
    Vector3 destinationV;
    [NonSerialized] public Vector3 originV;
    Terrain terrain;
    [SerializeField] float dronHeight = 1;

    public void SetData(GameObject origin, GameObject destination, GameMaterial material, Vector3 movingTo)
    {
        this.origin = origin;
        this.destination = destination;
        this.material = material;
        this.movingTo = movingTo;
        speed = FindObjectOfType<MainBase>().dronSpeed;

        destinationV = destination.transform.position;
        originV = origin.transform.position;
        terrain = Terrain.activeTerrain;
    }

    public void CreateData()
    {
        dronData = new(origin.name, destination.name, material, this, movingTo);
    }

    public float GetDistance()
    {
        return Vector2.Distance(transform.position, movingTo);
    }

    void Update()
    {
        if (!destination) return;
        RaycastHit hit;

        // Raycast in the forward direction to detect obstacles
        if (Physics.Raycast(transform.position, transform.forward, out hit, 1, LayerMask.GetMask("Obstacles")))
        {
            if (hit.collider != null)
            {
                // If we hit an object, rotate around it
                // Example: move to the left of the obstacle by adjusting the direction
                Vector3 obstacleAvoidanceDirection = Vector3.Cross(transform.up, transform.forward).normalized;
                Vector3 newPositionAroundObstacle = transform.position + obstacleAvoidanceDirection * speed * Time.deltaTime;

                newPositionAroundObstacle.y = GetTerrainHeight(newPositionAroundObstacle) + dronHeight;
                transform.position = newPositionAroundObstacle;

                // Optionally, you can adjust the look direction to face where you are moving
                transform.LookAt(newPositionAroundObstacle);

                return;  // Return early to avoid continuing the normal movement logic
            }
        }
        // print("origin = " + originV);
        // print("destination = " + destinationV);
        // print("dron = " + transform.position);
        // print("movingto = " + movingTo);

        float distanceX = destinationV.x - transform.position.x;
        float distanceZ = destinationV.z - transform.position.z;
        const float RANGE = 0.5f;

        if (Math.Abs(distanceX) <= RANGE && Math.Abs(distanceZ) <= RANGE && movingTo == destinationV)
        {
            dronGoal.Invoke();
            movingTo = originV;
            dronData.movingTo = originV;
            transform.LookAt(movingTo);
        }

        // if (originV == transform.position){
        distanceX = originV.x - transform.position.x;
        distanceZ = originV.z - transform.position.z;
        if (Math.Abs(distanceX) <= RANGE && Math.Abs(distanceZ) <= RANGE && movingTo == originV)
        {
            dronGoal.Invoke();
            movingTo = destinationV;
            dronData.movingTo = destinationV;
            transform.LookAt(movingTo);
        }

        // GetComponent<Rigidbody>().MovePosition(Vector3.MoveTowards(transform.position, movingTo, step));
        // transform.position = Vector3.MoveTowards(transform.position, movingTo, step);
        Vector3 direction = (movingTo - transform.position).normalized;
        Vector3 newPosition = transform.position + direction * speed * Time.deltaTime;

        newPosition.y = GetTerrainHeight(newPosition) + dronHeight;

        transform.position = newPosition;
        dronData.dronPosition = transform.position;
    }

    float GetTerrainHeight(Vector3 position)
    {
        return terrain.SampleHeight(position) + terrain.transform.position.y;
    }
}
