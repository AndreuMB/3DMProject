using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ResourcesEnum
{
    R1,
    R2,
    R3,
}

public class ResourceManager : MonoBehaviour
{
    public Dictionary<ResourcesEnum, GameObject> objectDictionary = new();
    [SerializeField] GameObject r1;
    [SerializeField] GameObject r2;
    [SerializeField] GameObject r3;
    [SerializeField] Material copper;
    [SerializeField] Material silver;
    [SerializeField] Material gold;

    void Start()
    {
        // Populate the dictionary with your enum and corresponding GameObjects
        // objectDictionary.Add(ResourcesEnum.Enemy, enemyPrefab);
        // objectDictionary.Add(ObjectType.Player, playerPrefab);
        // objectDictionary.Add(ObjectType.PowerUp, powerUpPrefab);
        // objectDictionary.Add(ObjectType.NPC, npcPrefab);
    }

    public GameObject GetRandomResourceGO() {
        GameObject[] rockFormations = { r1, r2, r3 };
        Material[] resources = { copper, silver, gold };

        // Random rock formation
        int randomIndexRF  = Random.Range(0, rockFormations.Length);
        GameObject rockFormation = rockFormations[randomIndexRF];

        // Random resource
        int randomIndexR  = Random.Range(0, resources.Length);
        Material resource = resources[randomIndexR];
        
        foreach (Transform child in rockFormation.transform)
        {
            child.GetComponent<MeshRenderer>().material = resource;
        }
        return rockFormation;
    }

    
}
