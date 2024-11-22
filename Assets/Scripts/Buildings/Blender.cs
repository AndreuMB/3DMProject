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
    BlenderPanel foundryPanel;
    Building building;
    public GameMaterialTypesEnum buildCraftingType;

    void Awake()
    {
        buttonsList.Add(new ButtonData("Craft", ToggleFoundryMenu));
    }
    void Start()
    {
        hud = FindObjectOfType<HUD>();
        building = GetComponent<Building>();
    }

    public void ShowHUD()
    {
        building.hud.GenerateButtons(buttonsList);
    }

    void ToggleFoundryMenu()
    {
        if (!foundryPanel) return;

        foundryPanel.gameObject.SetActive(!foundryPanel.gameObject.activeInHierarchy);

        // if (!foundryPanel.gameObject.activeInHierarchy) return;

        foundryPanel.GenerateListCombinations(buildCraftingType);
        // need for references of the building resource
        foundryPanel.GetComponent<BlenderPanel>().buildingParent = gameObject.GetComponent<Building>();
        foundryPanel.GetComponent<BlenderPanel>().blenderParent = this;
        foundryPanel.GetComponent<BlenderPanel>().hud = hud;
    }

    public void SetFoundryPanel(BlenderPanel foundryPP)
    {
        foundryPanel = foundryPP;
    }

    public bool CheckResources(ResourceCombination elementCombination)
    {
        GameMaterial storagedMaterial1 = building.FindGameMaterialInStorage(elementCombination.resource1.gameMaterialSO.materialName, building.data.storage);
        if (storagedMaterial1 == null || storagedMaterial1.quantity < elementCombination.resource1.quantity) return false;
        // if only one element
        if (elementCombination.resource2.gameMaterialSO == null) return true;
        GameMaterial storagedMaterial2 = building.FindGameMaterialInStorage(elementCombination.resource2.gameMaterialSO.materialName, building.data.storage);
        if (storagedMaterial2 == null || storagedMaterial2.quantity < elementCombination.resource2.quantity) return false;

        return true;
    }

    public void RemoveResources(ResourceCombination elementCombination)
    {
        GameMaterial storagedMaterial1 = building.FindGameMaterialInStorage(elementCombination.resource1.gameMaterialSO.materialName, building.data.storage);
        storagedMaterial1.quantity -= elementCombination.resource1.quantity;

        // if only one element
        if (elementCombination.resource2.gameMaterialSO == null) return;

        GameMaterial storagedMaterial2 = building.FindGameMaterialInStorage(elementCombination.resource2.gameMaterialSO.materialName, building.data.storage);
        storagedMaterial2.quantity -= elementCombination.resource2.quantity;
    }
}
