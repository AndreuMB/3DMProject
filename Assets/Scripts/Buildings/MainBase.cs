using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MainBase : MonoBehaviour, IBuilding
{
    readonly List<ButtonData> buttonsList = new();
    public HUD hud;
    [Header("Dron Settings")]
    public int drons = 2;
    public int dronStorage = 1;
    public float dronSpeed = 2;

    void Awake()
    {
        buttonsList.Add(new ButtonData("Create Dron", CreateDron));
        buttonsList.Add(new ButtonData("Upgrade Drons Speed", UpgradeDronsSpeed));
        buttonsList.Add(new ButtonData("Upgrade Drons Storage", UpgradeDronsStorage));
    }

    void Start()
    {
        hud = FindObjectOfType<HUD>();
        SetDrons(2);
    }

    public void ShowHUD()
    {
        hud.GenerateButtons(buttonsList);
    }

    public void CreateDron()
    {
        drons += 1;
        hud.UpdateDronsHUD(drons);
    }

    public void UpgradeDronsSpeed()
    {
        dronSpeed += 1;
        hud.UpdateDronsHUD(drons);
    }

    public void UpgradeDronsStorage()
    {
        dronStorage += 2;
        hud.UpdateDronsHUD(drons);
    }

    public void SetDrons(int dronsNew)
    {
        drons = dronsNew;
        hud.UpdateDronsHUD(drons);
    }
}
