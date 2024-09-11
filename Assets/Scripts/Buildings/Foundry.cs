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
    GameObject foundryPanel;
    void Start()
    {
        hud = FindObjectOfType<HUD>();
        buttonsList.Add(new ButtonData("Craft", ToggleFoundryMenu));
        hud.ShowGOHUD(gameObject);
    }

    public void ShowHUD()
    {
        hud.GenerateButtons(buttonsList);
    }

    void ToggleFoundryMenu()
    {
        if (!foundryPanel) return;

        foundryPanel.SetActive(!foundryPanel.activeInHierarchy);
        foundryPanel.GetComponent<FoundryPanel>().foundryParent = this;
        foundryPanel.GetComponent<FoundryPanel>().hud = hud;
    }

    public void SetFoundryPanel(GameObject foundryPP)
    {
        foundryPanel = foundryPP;
    }
}
