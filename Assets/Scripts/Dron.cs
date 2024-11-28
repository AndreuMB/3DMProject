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
    [NonSerialized] public float speed;
    [NonSerialized] public DronData dronData;
    [NonSerialized] public UnityEvent dronGoal = new UnityEvent();
    [NonSerialized] public GameObject row;
    [NonSerialized] public bool delete;
    Terrain terrain;
    [SerializeField] float dronHeight = 1;
    [NonSerialized] public bool successDelivery;
    public GameObject parentBuildingGO;
    MainBase mainBase;
    public float duration;
    bool coroutineRunning = false;
    public bool whilePlacingOnGoing = false;

    void Start()
    {
        mainBase = FindAnyObjectByType<MainBase>();
    }

    public void SetData(GameObject origin, GameObject destination, GameMaterial material, float speed)
    {
        this.origin = origin;
        this.destination = destination;
        this.material = material;
        parentBuildingGO = origin;
        this.speed = speed;

        terrain = Terrain.activeTerrain;
    }

    public void CreateData()
    {
        dronData = new(origin.name, destination.name, material, this);
    }

    public float GetDistance()
    {
        float distance = Vector3.Distance(origin.transform.position, destination.transform.position);
        return distance;
    }

    float GetTerrainHeight(Vector3 position)
    {
        return terrain.SampleHeight(position) + terrain.transform.position.y;
    }

    public IEnumerator DronTransport()
    {
        // check if building in progress and needs resource
        if (!MaterialTransportIsValid()) yield break;

        float timeElapsed = 0f;

        // leave from parent
        if (parentBuildingGO == origin)
        {
            // set new material for transport
            if (newMaterial != null && newMaterial != material) material = newMaterial;

            // check there are enough materials to transport
            if (!NotEnoughMaterialsOnStorage()) yield break;

            // check destination storage
            if (!EnoughBuildingStorage()) yield break;

            // remove material
            origin.GetComponent<Building>().AddGameMaterial(material.gameMaterialSO, -material.quantity);

            if (whilePlacingOnGoing != destination.GetComponent<Building>().placingOnGoing) yield break;
        }

        coroutineRunning = true;

        while (timeElapsed < duration)
        {
            Vector3 newPosition = Vector3.Lerp(origin.transform.position, destination.transform.position, timeElapsed / duration);
            newPosition.y = GetTerrainHeight(newPosition) + dronHeight;
            transform.position = newPosition;
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        coroutineRunning = false;

        // Ensure the object reaches the exact end position
        transform.position = destination.transform.position;

        // already on destination
        if (parentBuildingGO != destination)
        {
            if (EnoughBuildingStorage() && CheckMaterialPlacingOnGoing(material.gameMaterialSO.materialName))
            {
                // add material destination
                destination.GetComponent<Building>().AddGameMaterial(material.gameMaterialSO, material.quantity);
            }
            else
            {
                // return material to origin
                origin.GetComponent<Building>().AddGameMaterial(material.gameMaterialSO, material.quantity);
            };
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
            // whilePlacingOnGoing
            if (whilePlacingOnGoing)
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

        if (materialInBuilding != null && materialInBuilding.quantity >= destinationBuilding.data.maxStorage) return false;
        return true;
    }
    bool NotEnoughMaterialsOnStorage()
    {
        Building originBuilding = origin.GetComponent<Building>();
        GameMaterialsEnum materialName = material.gameMaterialSO.materialName;
        // we get the material from the origin building
        GameMaterial materialInBuilding = originBuilding.FindGameMaterialInStorage(materialName, originBuilding.data.storage);
        // if not enough material return false
        if (materialInBuilding != null && materialInBuilding.quantity < material.quantity) return false;
        return true;
    }

    public string GetNextMaterialName()
    {
        return newMaterial != null ? newMaterial.gameMaterialSO.materialName.ToString() : material.gameMaterialSO.materialName.ToString();
    }
}
