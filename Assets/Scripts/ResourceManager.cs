using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum RockFormationsEnum
{
    rockFormation1,
    rockFormation2,
    rockFormation3,
}

public enum ResourcesEnum
{
    cooper,
    silver,
    gold
}

public class ResourceManager : MonoBehaviour
{
    public Dictionary<RockFormationsEnum, GameObject> rockFormationDictionary = new();
    public Dictionary<ResourcesEnum, Material> resourceDictionary = new();
    [SerializeField] GameObject rockFormation1;
    [SerializeField] GameObject rockFormation2;
    [SerializeField] GameObject rockFormation3;
    [SerializeField] Material copper;
    [SerializeField] Material silver;
    [SerializeField] Material gold;

    void Awake()
    {
        // RockFormations Dictionary Fill
        rockFormationDictionary.Add(RockFormationsEnum.rockFormation1, rockFormation1);
        rockFormationDictionary.Add(RockFormationsEnum.rockFormation2, rockFormation2);
        rockFormationDictionary.Add(RockFormationsEnum.rockFormation3, rockFormation3);
        
        // Resources Dictionary Fill
        resourceDictionary.Add(ResourcesEnum.cooper, copper);
        resourceDictionary.Add(ResourcesEnum.silver, silver);
        resourceDictionary.Add(ResourcesEnum.gold, gold);
    }

    public (GameObject, OreData) GetRandomResourceGO() {
        // Random rock formation
        int randomIndexRF = Random.Range(0, rockFormationDictionary.Count);
        KeyValuePair<RockFormationsEnum, GameObject> rockFormationEntry = rockFormationDictionary.ElementAt(randomIndexRF);

        // Random resource
        int randomIndexR  = Random.Range(0, resourceDictionary.Count);
        KeyValuePair<ResourcesEnum, Material> resourceEntry = resourceDictionary.ElementAt(randomIndexR);

        // Save given properties
        OreData oreData = new()
        {
            rockFormationEnum = rockFormationEntry.Key,
            resourceEnum = resourceEntry.Key
        };

        GameObject oreGO = GenerateOre(oreData);
        return (oreGO, oreData);
    }

    public GameObject GenerateOre(OreData oreData){
        GameObject oreGO = Instantiate(rockFormationDictionary[oreData.rockFormationEnum]);
        oreGO = SetMaterial(oreGO,oreData.resourceEnum);
        oreGO.AddComponent<Ore>().oreData = oreData;
        return oreGO;
    }

    private GameObject SetMaterial(GameObject oreGO, ResourcesEnum resource) {
         foreach (Transform child in oreGO.transform)
        {
            MeshRenderer meshRenderer = child.GetComponent<MeshRenderer>();
            if (meshRenderer != null)
            {
                meshRenderer.material = resourceDictionary[resource];
            }
            else
            {
                Debug.LogWarning($"MeshRenderer not found on {child.name}");
            }
        }
        return oreGO;
    }

    
}
