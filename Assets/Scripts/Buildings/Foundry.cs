using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Foundry : MonoBehaviour, IBuilding
{
    // [SerializeField] GameObject hud;
    readonly List<ButtonData> buttonsList = new();
    HUD hud;
    void Start()
    {
        hud = FindObjectOfType<HUD>();
        buttonsList.Add(new ButtonData("Craft", ShowFoundryMenu));
        hud.GenerateButtons(buttonsList);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ShowHUD(){
        hud.GenerateButtons(buttonsList);
    }

    void ShowFoundryMenu(){
        print("show foundry");
    }
}
