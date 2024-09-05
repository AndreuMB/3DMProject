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
    GameObject foundryPanelPrefab;
    GameObject foundryPanel;
    void Start()
    {
        hud = FindObjectOfType<HUD>();
        buttonsList.Add(new ButtonData("Craft", ToggleFoundryMenu));
        hud.GenerateButtons(buttonsList);
    }

    public void ShowHUD()
    {
        hud.GenerateButtons(buttonsList);
    }

    void ToggleFoundryMenu()
    {
        if (!foundryPanel)
        {
            foundryPanel = Instantiate(foundryPanelPrefab, hud.transform);
        }
        else
        {
            foundryPanel.SetActive(!foundryPanel.activeInHierarchy);
        }
    }

    public void SetFoundryPanelPrefab(GameObject foundryPP)
    {
        foundryPanelPrefab = foundryPP;
    }
}
