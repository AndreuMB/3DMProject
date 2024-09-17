using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    [SerializeField] TMP_Text drons;
    Player player;
    MainBase mainBase;
    [SerializeField] GameObject resourceContainer;
    [SerializeField] GameObject resourcePrefab;
    List<GameMaterial> materialsBuilding;
    [SerializeField] GameObject dronMenuBtn;
    // public GameObject DMMenu;
    // [SerializeField] GameObject DronUpgradeBtn;
    public GameObject buttonPrefab;
    // [SerializeField] GameObject dronManagerPrefab;
    public Transform buildingButtpnsPanel;

    public void Start()
    {
        // mainBase = FindObjectOfType<MainBase>();
        // if (!mainBase) { Debug.LogError("Need MainBase gameobject in gamescene to work"); return; };
        player = FindObjectOfType<Player>();

        player.selectedGOev.AddListener(ShowGOHUD);
        // DMBtn.SetActive(false);
        // DMMenu.SetActive(false);
        // DronUpgradeBtn.SetActive(false);
        // UpdateDronsHUD();
    }

    void Update()
    {
        if (!player) return;
        if (materialsBuilding == null) return;
        foreach (GameMaterial resource in materialsBuilding)
        {
            if (!resource.HUDGO) return;
            resource.HUDGO.GetComponent<TMP_Text>().text = resource.gameMaterialSO.materialName.ToString() + ": " + resource.quantity.ToString();
        }
    }

    public void ShowGOHUD(GameObject selectedGO)
    {
        if (!player.GetMouseSelectorStatus()) return;
        if (!selectedGO) return;
        if (!selectedGO.GetComponent<Building>()) return;

        CleanHUDContainer();
        Building selectedBuilding = selectedGO.GetComponent<Building>();
        if (selectedBuilding.buildingType != null)
        {
            selectedBuilding.GetComponent<IBuilding>().ShowHUD();
        }
        if (selectedBuilding.data.storageBool)
        {
            SetDronManagerButton();
            ShowResourcesBuilding(selectedGO.GetComponent<Building>());
        }
    }

    void ShowResourcesBuilding(Building selectedBuilding)
    {
        materialsBuilding = selectedBuilding.data.storage;

        materialsBuilding.RemoveAll(gameMaterial =>
        {
            if (gameMaterial.quantity <= 0)
            {
                if (selectedBuilding.data.buildingType == BuildingsEnum.Extractor) return false;
                foreach (DronData dron in selectedBuilding.data.setDrons)
                {
                    if (dron.material.gameMaterialSO.materialName == gameMaterial.gameMaterialSO.materialName) return false;
                }
                return true;
            }
            return false;
        });
        foreach (GameMaterial resource in materialsBuilding)
        {
            GameObject newResource = Instantiate(resourcePrefab);
            newResource.GetComponent<TMP_Text>().text = resource.gameMaterialSO.materialName + ": " + resource.quantity.ToString();
            newResource.transform.SetParent(resourceContainer.transform, false);
            resource.HUDGO = newResource;
        }
    }

    void CleanHUDContainer()
    {
        // DMBtn.SetActive(false);
        // DronUpgradeBtn.SetActive(false);
        CleanButtonsPanel();
        materialsBuilding = null;
        foreach (Transform child in resourceContainer.transform)
        {
            Destroy(child.gameObject);
        }
    }
    public void UpdateDronsHUD(int dronsNumber)
    {
        drons.text = "DRONS " + dronsNumber.ToString();
    }

    public void GenerateButtons(List<ButtonData> buttonsList)
    {
        foreach (ButtonData buttonData in buttonsList)
        {
            GameObject buttonGO = Instantiate(buttonPrefab, buildingButtpnsPanel);
            buttonGO.GetComponent<Button>().onClick.AddListener(() => buttonData.buttonAction());
            buttonGO.GetComponentInChildren<TMP_Text>().text = buttonData.buttonName;
        }
    }

    void CleanButtonsPanel()
    {
        foreach (Transform child in buildingButtpnsPanel.transform)
        {
            Destroy(child.gameObject);
        }
    }

    void SetDronManagerButton()
    {
        Instantiate(dronMenuBtn, buildingButtpnsPanel);
    }
}
