using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Foundry : MonoBehaviour, IBuilding
{
    // [SerializeField] GameObject hud;
    List<string> buttonsList = new();
    HUD hud;
    void Start()
    {
        hud = FindObjectOfType<HUD>();
        buttonsList.Add("Craft");
        hud.GenerateButtons(buttonsList);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ShowHUD(){
        hud.GenerateButtons(buttonsList);
    }
}
