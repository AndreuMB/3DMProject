using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MainBase : MonoBehaviour, IBuilding
{
    readonly List<ButtonData> buttonsList = new();
    public HUD hud;
    MaterialManager materialManager;
    Building building;

    [Header("Dron Settings")]
    public int drons = 2;
    public int dronStorage = 1;
    public float dronSpeed = 2;

    [Header("Upgrade Costs")]
    public List<GameMaterial> addDronCost = new();
    public List<GameMaterial> dronSpeedCost = new();
    public List<GameMaterial> dronStorageCost = new();
    public float costMultiplier = 1.5f; // Multiplier for scaling costs

    void Awake()
    {
        // put prices
        addDronCost.Add(new(MaterialManager.GetGameMaterialSO(GameMaterialsEnum.gold), 50));
        dronSpeedCost.Add(new(MaterialManager.GetGameMaterialSO(GameMaterialsEnum.iron), 2));
        dronSpeedCost.Add(new(MaterialManager.GetGameMaterialSO(GameMaterialsEnum.copper), 2));
        dronStorageCost.Add(new(MaterialManager.GetGameMaterialSO(GameMaterialsEnum.copper), 10));
        dronStorageCost.Add(new(MaterialManager.GetGameMaterialSO(GameMaterialsEnum.gold), 5));

        // generate drons
        buttonsList.Add(new ButtonData("Create Dron", () => Upgrade(addDronCost), addDronCost));
        buttonsList.Add(new ButtonData("Upgrade Drons Speed", () => Upgrade(dronSpeedCost), dronSpeedCost));
        buttonsList.Add(new ButtonData("Upgrade Drons Storage", () => Upgrade(dronStorageCost), dronStorageCost));
    }

    void Start()
    {
        hud = FindObjectOfType<HUD>();
        materialManager = FindObjectOfType<MaterialManager>();
        building = GetComponent<Building>();
        SetDrons(2);
        // select main base on load
        Player player = GetComponent<Player>();
        if (!player) return;
        player.SetClickedGOFromVector3(player.gameObject.transform.position, true);
    }

    public void ShowHUD()
    {
        hud.GenerateButtons(buttonsList);
    }

    void Upgrade(List<GameMaterial> cost)
    {
        if (MaterialManager.CanAfford(cost, building))
        {
            dronStorage += 2;
            ApplyCostUpgrade(cost);
            ScaleCost(cost);
            hud.UpdateDronsHUD(drons);
            hud.ShowGOHUD(gameObject);
        }
    }

    public void SetDrons(int dronsNew)
    {
        drons = dronsNew;
        hud.UpdateDronsHUD(drons);
    }

    private void ScaleCost(List<GameMaterial> cost)
    {
        foreach (GameMaterial gameMaterial in cost)
        {
            gameMaterial.quantity = Mathf.CeilToInt(gameMaterial.quantity * costMultiplier);
        }
    }

    void ApplyCostUpgrade(List<GameMaterial> cost)
    {
        foreach (GameMaterial gameMaterial in cost)
        {
            building.RemoveGameMaterial(gameMaterial.gameMaterialSO.materialName, gameMaterial.quantity);
        }
    }
}
