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

    void Awake()
    {
        buttonsList.Add(new ButtonData("Craft", ToggleFoundryMenu));
    }
    void Start()
    {
        hud = FindObjectOfType<HUD>();
    }

    public void ShowHUD()
    {
        GetComponent<Building>().hud.GenerateButtons(buttonsList);
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
