using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public enum RockFormationsEnum
{
    rockFormation1,
    rockFormation2,
    rockFormation3,
}

public enum ResourcesEnum
{
    iron,
    copper,
    gold
}

public enum ElementsEnum
{
    rubedo,
    argon,
    nickel
}

public enum GameMaterialsEnum
{
    iron,
    copper,
    gold,
    refinedIron,
    refinedCopper,
    refinedGold,
    rubedo,
    argon,
    nickel
}
public enum GameMaterialTypesEnum
{
    resource,
    refined,
    element

}

public class MaterialManager : MonoBehaviour
{
    public static List<GameMaterialSO> gameMaterialsList;
    Dictionary<RockFormationsEnum, GameObject> rockFormationDictionary = new();
    Dictionary<GameMaterialsEnum, Material> resourceMaterialDictionary = new();
    [SerializeField] GameObject rockFormation1;
    [SerializeField] GameObject rockFormation2;
    [SerializeField] GameObject rockFormation3;
    [SerializeField] Material iron;
    [SerializeField] Material copper;
    [SerializeField] Material gold;

    public static void LoadGameMaterials()
    {
        // Load all GameMaterialSO assets from the "Resources/GameMaterials" folder
        GameMaterialSO[] materials = Resources.LoadAll<GameMaterialSO>("GameMaterials");

        // Initialize the list
        gameMaterialsList = new List<GameMaterialSO>(materials);

        Debug.Log($"Loaded {gameMaterialsList.Count} game materials at runtime.");
    }

    void Awake()
    {
        LoadGameMaterials();
        // RockFormations Dictionary Fill
        rockFormationDictionary.Add(RockFormationsEnum.rockFormation1, rockFormation1);
        rockFormationDictionary.Add(RockFormationsEnum.rockFormation2, rockFormation2);
        rockFormationDictionary.Add(RockFormationsEnum.rockFormation3, rockFormation3);

        // resourcesList = gameMaterialsList.Where(
        //     material => material.type == GameMaterialTypesEnum.resource
        // ).ToList();

        // Resources Dictionary Fill
        resourceMaterialDictionary.Add(GameMaterialsEnum.iron, iron);
        resourceMaterialDictionary.Add(GameMaterialsEnum.copper, copper);
        resourceMaterialDictionary.Add(GameMaterialsEnum.gold, gold);
    }


    public (GameObject, OreData) GetRandomResourceGO()
    {
        // Random rock formation
        int randomIndexRF = Random.Range(0, rockFormationDictionary.Count);
        KeyValuePair<RockFormationsEnum, GameObject> rockFormationEntry = rockFormationDictionary.ElementAt(randomIndexRF);

        // Random resource
        int randomIndexR = Random.Range(0, resourceMaterialDictionary.Count);
        KeyValuePair<GameMaterialsEnum, Material> resourceEntry = resourceMaterialDictionary.ElementAt(randomIndexR);

        // Save given properties
        OreData oreData = new()
        {
            rockFormation = rockFormationEntry.Key,
            gameMaterialName = resourceEntry.Key
        };

        GameObject oreGO = GenerateOre(oreData);
        return (oreGO, oreData);
    }

    public GameObject GenerateOre(OreData oreData)
    {
        GameObject oreGO = Instantiate(rockFormationDictionary[oreData.rockFormation]);
        oreGO = SetMaterial(oreGO, oreData.gameMaterialName);
        oreGO.AddComponent<Ore>().oreData = oreData;
        return oreGO;
    }

    private GameObject SetMaterial(GameObject oreGO, GameMaterialsEnum gameMaterialEnum)
    {
        foreach (Transform child in oreGO.transform)
        {
            MeshRenderer meshRenderer = child.GetComponent<MeshRenderer>();
            if (meshRenderer != null)
            {
                meshRenderer.material = resourceMaterialDictionary[gameMaterialEnum];
            }
            else
            {
                Debug.LogWarning($"MeshRenderer not found on {child.name}");
            }
        }
        return oreGO;
    }

    public static GameMaterialSO GetGameMaterialSO(GameMaterialsEnum gameMaterial)
    {
        return gameMaterialsList.Find(
            material => material.materialName == gameMaterial
        );
    }

    public static bool CanAfford(List<GameMaterial> cost, Building building)
    {
        foreach (GameMaterial gameMaterial in cost)
        {
            GameMaterial gameMaterialStorage = building.FindGameMaterialInStorage(gameMaterial.gameMaterialSO.materialName, building.data.storage);
            if (gameMaterialStorage == null || gameMaterialStorage.quantity < gameMaterial.quantity)
            {
                return false;
            }
        }
        return true;
    }


}
