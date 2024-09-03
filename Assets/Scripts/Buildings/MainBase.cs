using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MainBase : MonoBehaviour, IBuilding
{
    readonly List<ButtonData> buttonsList = new();
    HUD hud;
    [Header("Dron Settings")]
    public int drons = 2;
    public int dronStorage = 1;
    public float dronSpeed = 2;
    void Start()
    {
        hud = FindObjectOfType<HUD>();
        buttonsList.Add(new ButtonData("Create Dron", CreateDron));
        buttonsList.Add(new ButtonData("Upgrade Drons Speed", UpgradeDronsSpeed));
        buttonsList.Add(new ButtonData("Upgrade Drons Storage", UpgradeDronsStorage));
        hud.GenerateButtons(buttonsList);
    }

    public void ShowHUD()
    {
        hud.GenerateButtons(buttonsList);
    }

    public void CreateDron(){
        drons += 1;
        hud.UpdateDronsHUD();
    }

    public void UpgradeDronsSpeed(){
        dronSpeed += 1;
        hud.UpdateDronsHUD();
    }
    
    public void UpgradeDronsStorage(){
        dronStorage += 2;
        hud.UpdateDronsHUD();
    }

    public void SetDrons(int dronsNew){
        drons = dronsNew;
        hud.UpdateDronsHUD();
    }
}
