using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Extractor : MonoBehaviour, IBuilding
{
    float rate = 1;
    public GameMaterial gameMaterial;
    Building building;

    void Start()
    {
        building = GetComponent<Building>();
        building.buildComplete.AddListener(() => StartCoroutine(ExtractResource()));
        // gameMaterial = new GameMaterial(GetGameMaterialSOFromMaterialCell(), 1);
        gameMaterial = new GameMaterial(GetComponent<Building>().placeholderMaterialSO, 1);
    }

    public void ShowHUD()
    {
        building.hud.GenerateButtons(new());
    }

    IEnumerator ExtractResource()
    {
        while (isActiveAndEnabled && !building.placingOnGoing)
        {
            yield return new WaitForSeconds(rate);
            GetComponent<Building>().AddGameMaterial(gameMaterial.gameMaterialSO, gameMaterial.quantity);
        }
        yield break;
    }

    // GameMaterialSO GetGameMaterialSOFromMaterialCell()
    // {
    //     PlacementSystem placementSystem = FindObjectOfType<PlacementSystem>();
    //     GameMaterialSO gameMaterialSO = placementSystem.buildingState.GetOreResource(transform.parent.position);
    //     return gameMaterialSO;
    // }
}
