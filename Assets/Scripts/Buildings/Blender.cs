using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Blender : MonoBehaviour, IBuilding
{
    // [SerializeField] GameObject hud;
    readonly List<ButtonData> buttonsList = new();
    HUD hud;
    FoundryPanel foundryPanel;
    public GameMaterialTypesEnum buildCraftingType;

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

        foundryPanel.gameObject.SetActive(!foundryPanel.gameObject.activeInHierarchy);

        // if (!foundryPanel.gameObject.activeInHierarchy) return;

        foundryPanel.GenerateListCombinations(buildCraftingType);
        // need for references of the building resource
        foundryPanel.GetComponent<FoundryPanel>().buildingParent = gameObject.GetComponent<Building>();
        foundryPanel.GetComponent<FoundryPanel>().hud = hud;
    }

    public void SetFoundryPanel(FoundryPanel foundryPP)
    {
        foundryPanel = foundryPP;
    }
}
