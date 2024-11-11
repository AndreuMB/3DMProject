using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Extractor : MonoBehaviour, IBuilding
{
    // [SerializeField] GameObject hud;
    // readonly List<ButtonData> buttonsList = new();
    HUD hud;
    float rate = 1;
    GameMaterial gameMaterial;
    Building building;

    void Awake()
    {
        // buttonsList.Add(new ButtonData("Craft", ToggleFoundryMenu));
    }
    void Start()
    {
        hud = FindObjectOfType<HUD>();
        building = GetComponent<Building>();
        building.buildComplete.AddListener(() => StartCoroutine(ExtractResource()));
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
            GetComponent<Building>().AddResource(gameMaterial.gameMaterialSO, gameMaterial.quantity);
        }
        yield break;
    }

    public void SetGameMaterial(GameMaterial gameMaterial)
    {
        this.gameMaterial = gameMaterial;
    }
}
