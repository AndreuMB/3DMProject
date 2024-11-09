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
    GameMaterial newMaterial;
    // public Coroutine coroutine;
    [NonSerialized] public Vector3 movingTo;
    [NonSerialized] public float speed;
    [NonSerialized] public DronData dronData;
    [NonSerialized] public UnityEvent dronGoal = new UnityEvent();
    [NonSerialized] public GameObject row;
    [NonSerialized] public bool delete;
    Vector3 destinationV;
    [NonSerialized] public Vector3 originV;
    Terrain terrain;
    [SerializeField] float dronHeight = 1;
    [NonSerialized] public bool successDelivery;
    public GameObject parentBuildingGO;
    MainBase mainBase;
    public float duration;
    bool coroutineRunning = false;

    void Start()
    {
        mainBase = FindAnyObjectByType<MainBase>();
    }

    public void SetData(GameObject origin, GameObject destination, GameMaterial material, Vector3 movingTo)
    {
        this.origin = origin;
        this.destination = destination;
        this.material = material;
        this.movingTo = movingTo;
        parentBuildingGO = origin;
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
        // if (!destination) return;
        // RaycastHit hit;

        // // Raycast in the forward direction to detect obstacles
        // if (Physics.Raycast(transform.position, transform.forward, out hit, 1, LayerMask.GetMask("Obstacles")))
        // {
        //     if (hit.collider != null)
        //     {
        //         // If we hit an object, rotate around it
        //         // Example: move to the left of the obstacle by adjusting the direction
        //         Vector3 obstacleAvoidanceDirection = Vector3.Cross(transform.up, transform.forward).normalized;
        //         Vector3 newPositionAroundObstacle = transform.position + obstacleAvoidanceDirection * speed * Time.deltaTime;

        //         newPositionAroundObstacle.y = GetTerrainHeight(newPositionAroundObstacle) + dronHeight;
        //         transform.position = newPositionAroundObstacle;

        //         // Optionally, you can adjust the look direction to face where you are moving
        //         transform.LookAt(newPositionAroundObstacle);

        //         return;  // Return early to avoid continuing the normal movement logic
        //     }
        // }

        // float distanceX = destinationV.x - transform.position.x;
        // float distanceZ = destinationV.z - transform.position.z;
        // const float RANGE = 0.5f;

        // if (Math.Abs(distanceX) <= RANGE && Math.Abs(distanceZ) <= RANGE && movingTo == destinationV)
        // {
        //     dronGoal.Invoke();
        //     movingTo = originV;
        //     dronData.movingTo = originV;
        //     transform.LookAt(movingTo);
        // }

        // // if (originV == transform.position){
        // distanceX = originV.x - transform.position.x;
        // distanceZ = originV.z - transform.position.z;
        // if (Math.Abs(distanceX) <= RANGE && Math.Abs(distanceZ) <= RANGE && movingTo == originV)
        // {
        //     dronGoal.Invoke();
        //     movingTo = destinationV;
        //     dronData.movingTo = destinationV;
        //     transform.LookAt(movingTo);
        // }

        // // GetComponent<Rigidbody>().MovePosition(Vector3.MoveTowards(transform.position, movingTo, step));
        // // transform.position = Vector3.MoveTowards(transform.position, movingTo, step);
        // Vector3 direction = (movingTo - transform.position).normalized;
        // Vector3 newPosition = transform.position + direction * speed * Time.deltaTime;

        // newPosition.y = GetTerrainHeight(newPosition) + dronHeight;

        // transform.position = newPosition;
        // dronData.dronPosition = transform.position;
    }

    // float GetTerrainHeight(Vector3 position)
    // {
    //     return terrain.SampleHeight(position) + terrain.transform.position.y;
    // }

    public IEnumerator DronTransport()
    {

        if (!MaterialTransportIsValid()) yield break;

        float timeElapsed = 0f;

        // leave from parent
        if (parentBuildingGO == origin)
        {
            // set new material for transport
            if (newMaterial != null && newMaterial != material) material = newMaterial;

            // check destination storage
            if (!EnoughBuildingStorage()) yield break;

            // remove material
            origin.GetComponent<Building>().AddResource(material.gameMaterialSO, -material.quantity);
        }

        coroutineRunning = true;

        while (timeElapsed < duration)
        {
            transform.position = Vector3.Lerp(origin.transform.position, destination.transform.position, timeElapsed / duration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        coroutineRunning = false;

        // Ensure the object reaches the exact end position
        transform.position = destination.transform.position;

        // already on destination
        if (parentBuildingGO != destination)
        {
            // add material
            destination.GetComponent<Building>().AddResource(material.gameMaterialSO, material.quantity);
        }

        // swap values
        (destination, origin) = (origin, destination);


        if (delete && parentBuildingGO == origin)
        {
            DeleteDron();
            yield return null;
        }

        // if (CheckPlacing(material.gameMaterialSO.materialName)) 
        StartCoroutine(DronTransport());
        // }


    }

    void DeleteDron()
    {
        mainBase.SetDrons(mainBase.drons + 1);
        List<DronData> listDrons = origin.GetComponent<Building>().data.setDrons;
        listDrons.Remove(dronData);
        Destroy(gameObject);
        if (row) Destroy(row);
    }

    bool CheckMaterialPlacingOnGoing(GameMaterialsEnum materialName)
    {
        // destination
        if (parentBuildingGO != destination)
        {
            // building placingOnGoing
            if (destination.GetComponent<Building>().placingOnGoing)
            {
                Building destinationBuilding = destination.GetComponent<Building>();
                GameMaterial materialInBuilding = destinationBuilding.FindGameMaterialInStorage(materialName, destinationBuilding.data.storage);
                // enough material or not needed for build
                if (materialInBuilding == null || materialInBuilding.quantity >= 0)
                {
                    return false;
                }
            }
        }
        return true;
    }

    public void ChangeMaterial(GameMaterial newMaterial)
    {
        this.newMaterial = newMaterial;
        if (!coroutineRunning)
        {
            StartCoroutine(DronTransport());
        }
    }

    bool MaterialTransportIsValid()
    {
        bool valid = false;
        // build dont have enough current material
        if (CheckMaterialPlacingOnGoing(material.gameMaterialSO.materialName)) valid = true;

        // new material selected
        if (newMaterial != null)
        {
            // build dont have enough new material selected
            if (CheckMaterialPlacingOnGoing(newMaterial.gameMaterialSO.materialName)) valid = true;
        }

        return valid;
    }

    public void CheckDeleteDron()
    {
        if (coroutineRunning)
        {
            delete = true;
            return;
        }

        DeleteDron();
    }

    bool EnoughBuildingStorage()
    {
        Building destinationBuilding = destination.GetComponent<Building>();
        GameMaterialsEnum materialName = material.gameMaterialSO.materialName;
        GameMaterial materialInBuilding = destinationBuilding.FindGameMaterialInStorage(materialName, destinationBuilding.data.storage);

        if (materialInBuilding.quantity >= destinationBuilding.data.maxStorage) return false;
        return true;
    }
}
