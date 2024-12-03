using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Terraformer : MonoBehaviour, IBuilding
{
    readonly List<ButtonData> buttonsList = new();
    HUD hud;
    Building building;
    public GameMaterialTypesEnum buildCraftingType;

    void Awake()
    {
        List<GameMaterial> gameOverPrice = new();
        gameOverPrice.Add(new(MaterialManager.GetGameMaterialSO(GameMaterialsEnum.argon), 100));
        buttonsList.Add(new ButtonData("Terraform", () => GameOver(gameOverPrice), gameOverPrice));
    }
    void Start()
    {
        hud = FindObjectOfType<HUD>();
        building = GetComponent<Building>();
    }

    public void ShowHUD()
    {
        building.hud.GenerateButtons(buttonsList);
    }

    void GameOver(List<GameMaterial> cost)
    {
        if (MaterialManager.CanAfford(cost, building))
        {
            building.ApplyCostUpgrade(cost);
            Time.timeScale = 0;
            hud.gameOverScreen.SetActive(true);
        }
    }
}
