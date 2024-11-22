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
    [SerializeField] GameObject gameMaterialContainer;
    [SerializeField] GameObject sendGameMaterialContainer;
    [SerializeField] GameObject gameMaterialPrefab;
    List<GameMaterial> materialsBuilding;
    [SerializeField] GameObject dronMenuBtn;
    // public GameObject DMMenu;
    // [SerializeField] GameObject DronUpgradeBtn;
    public GameObject buttonPrefab;
    // [SerializeField] GameObject dronManagerPrefab;
    public Transform buildingButtpnsPanel;
    public PlacementSystem placementSystem;

    public void Start()
    {
        // mainBase = FindObjectOfType<MainBase>();
        // if (!mainBase) { Debug.LogError("Need MainBase gameobject in gamescene to work"); return; };
        player = FindObjectOfType<Player>();

        player.selectedGOev.AddListener(ShowGOHUD);
        placementSystem = FindObjectOfType<PlacementSystem>();
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

        if (selectedBuilding.placingOnGoing)
        {
            ShowBuildResourcesBuilding(selectedGO.GetComponent<Building>());
            ShowResourcesBuilding(selectedGO.GetComponent<Building>());
            return;
        }

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
            if (gameMaterial.quantity <= 0 && !selectedBuilding.placingOnGoing)
            {
                // if (selectedBuilding.data.buildingType == BuildingsEnum.Extractor) return false;
                foreach (DronData dron in selectedBuilding.data.setDrons)
                {
                    if (dron.material.gameMaterialSO.materialName == gameMaterial.gameMaterialSO.materialName) return false;
                }
                return true;
            }
            return false;
        });
        foreach (GameMaterial gameMaterial in materialsBuilding)
        {
            GameObject newGameMaterial = Instantiate(gameMaterialPrefab);
            newGameMaterial.GetComponent<TMP_Text>().text = gameMaterial.gameMaterialSO.materialName + ": " + gameMaterial.quantity.ToString();
            newGameMaterial.transform.SetParent(gameMaterialContainer.transform, false);
            gameMaterial.HUDGO = newGameMaterial;
        }
    }

    void ShowBuildResourcesBuilding(Building selectedBuilding)
    {
        // sendResourceContainer.SetActive(true);
        ObjectData buildingInfo = placementSystem.database.objectsData.Find(x => x.Type == selectedBuilding.data.buildingType);
        List<GameMaterial> gameMaterialsBuild = buildingInfo.GameMaterialsBuild;
        materialsBuilding = gameMaterialsBuild;

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
            GameObject newResource = Instantiate(gameMaterialPrefab);
            newResource.GetComponent<TMP_Text>().text = resource.gameMaterialSO.materialName + ": " + resource.quantity.ToString();
            newResource.transform.SetParent(sendGameMaterialContainer.transform, false);
            resource.HUDGO = newResource;
        }

        UpdateBuildingResourceContainerColor(selectedBuilding);
    }

    public void CleanHUDContainer()
    {
        // DMBtn.SetActive(false);
        // DronUpgradeBtn.SetActive(false);
        CleanButtonsPanel();
        materialsBuilding = null;
        foreach (Transform child in gameMaterialContainer.transform)
        {
            Destroy(child.gameObject);
        }

        foreach (Transform child in sendGameMaterialContainer.transform)
        {
            Destroy(child.gameObject);
        }

        Color whiteColor = Color.white;
        gameMaterialContainer.GetComponent<Image>().color = whiteColor;

        sendGameMaterialContainer.SetActive(false);
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
            UpgradeButton upgradeButton = buttonGO.GetComponent<UpgradeButton>();
            buttonGO.GetComponent<Button>().onClick.AddListener(() => buttonData.buttonAction());
            upgradeButton.title.text = buttonData.buttonName;
            if (buttonData.actionCost != null)
            {
                upgradeButton.costContainer.SetActive(true);
                foreach (GameMaterial gameMaterial in buttonData.actionCost)
                {
                    GameObject newGameMaterial = Instantiate(gameMaterialPrefab);
                    newGameMaterial.GetComponent<TMP_Text>().color = Color.black;
                    newGameMaterial.GetComponent<TMP_Text>().fontSize = 15;
                    newGameMaterial.GetComponent<TMP_Text>().text = gameMaterial.gameMaterialSO.materialName + ": " + gameMaterial.quantity.ToString();
                    newGameMaterial.transform.SetParent(upgradeButton.costContainer.transform, false);
                }
            }
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

    public void UpdateBuildingResourceContainerColor(Building building)
    {
        if (building.placingOnGoing)
        {
            Color redColor = Color.red;
            // redColor.a = 0.33f;
            gameMaterialContainer.GetComponent<Image>().color = redColor;
        }
        else
        {
            Color whiteColor = Color.white;
            // blackColor.a = 0.33f;
            gameMaterialContainer.GetComponent<Image>().color = whiteColor;
        }
    }
}
