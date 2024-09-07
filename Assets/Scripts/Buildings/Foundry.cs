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
        hud.ShowGOHUD(gameObject);
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
        foundryPanel.GetComponent<FoundryPanel>().foundryParent = this;
        foundryPanel.GetComponent<FoundryPanel>().hud = hud;
    }

    public void SetFoundryPanelPrefab(GameObject foundryPP)
    {
        foundryPanelPrefab = foundryPP;
    }
}
