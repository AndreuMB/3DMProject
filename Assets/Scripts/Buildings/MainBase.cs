using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MainBase : MonoBehaviour, IBuilding
{
    List<string> buttonsList = new();
    HUD hud;
    void Start()
    {
        hud = FindObjectOfType<HUD>();
        buttonsList.Add("Upgrade Drons");
        hud.GenerateButtons(buttonsList);
    }

    public void ShowHUD()
    {
        hud.GenerateButtons(buttonsList);
    }
}
