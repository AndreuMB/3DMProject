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
    static List<GameMaterialSO> gameMaterialsList;
    Dictionary<RockFormationsEnum, GameObject> rockFormationDictionary = new();
    Dictionary<GameMaterialSO, Material> resourceMaterialDictionary = new();
    [SerializeField] GameObject rockFormation1;
    [SerializeField] GameObject rockFormation2;
    [SerializeField] GameObject rockFormation3;
    [SerializeField] Material iron;
    [SerializeField] Material copper;
    [SerializeField] Material gold;

    public static void LoadGameMaterials()
    {
        // Initialize the list
        gameMaterialsList = new List<GameMaterialSO>();

        // Get all asset GUIDs from the folder "Assets/GameMaterials"
        string[] guids = AssetDatabase.FindAssets("t:GameMaterialSO", new[] { "Assets/GameMaterials" });

        // Loop through the GUIDs to load each asset
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            GameMaterialSO gameMaterial = AssetDatabase.LoadAssetAtPath<GameMaterialSO>(path);

            if (gameMaterial != null)
            {
                gameMaterialsList.Add(gameMaterial);
            }
        }

        Debug.Log("Loaded " + gameMaterialsList.Count + " game materials.");
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
        resourceMaterialDictionary.Add(GetGameMaterialSO(GameMaterialsEnum.iron), iron);
        resourceMaterialDictionary.Add(GetGameMaterialSO(GameMaterialsEnum.copper), copper);
        resourceMaterialDictionary.Add(GetGameMaterialSO(GameMaterialsEnum.gold), gold);
    }


    public (GameObject, OreData) GetRandomResourceGO()
    {
        // Random rock formation
        int randomIndexRF = Random.Range(0, rockFormationDictionary.Count);
        KeyValuePair<RockFormationsEnum, GameObject> rockFormationEntry = rockFormationDictionary.ElementAt(randomIndexRF);

        // Random resource
        int randomIndexR = Random.Range(0, resourceMaterialDictionary.Count);
        KeyValuePair<GameMaterialSO, Material> resourceEntry = resourceMaterialDictionary.ElementAt(randomIndexR);

        // Save given properties
        OreData oreData = new()
        {
            rockFormation = rockFormationEntry.Key,
            gameMaterialSO = resourceEntry.Key
        };

        GameObject oreGO = GenerateOre(oreData);
        return (oreGO, oreData);
    }

    public GameObject GenerateOre(OreData oreData)
    {
        GameObject oreGO = Instantiate(rockFormationDictionary[oreData.rockFormation]);
        oreGO = SetMaterial(oreGO, oreData.gameMaterialSO);
        oreGO.AddComponent<Ore>().oreData = oreData;
        return oreGO;
    }

    private GameObject SetMaterial(GameObject oreGO, GameMaterialSO gameMaterialSO)
    {
        foreach (Transform child in oreGO.transform)
        {
            MeshRenderer meshRenderer = child.GetComponent<MeshRenderer>();
            if (meshRenderer != null)
            {
                meshRenderer.material = resourceMaterialDictionary[gameMaterialSO];
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


}
