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
    [SerializeField] GameObject resourceContainer;
    [SerializeField] GameObject resourcePrefab;
    List<Resource> resourcesSE;
    [SerializeField] GameObject dronMenuBtn;
    // public GameObject DMMenu;
    // [SerializeField] GameObject DronUpgradeBtn;
    public GameObject buttonPrefab;
    // [SerializeField] GameObject dronManagerPrefab;
    public Transform buildingButtpnsPanel;
    
    public void IniHUD(){
        if (!FindObjectOfType<Player>()) { Debug.LogError("Need MainBase gameobject in gamescene to work"); return; };
        player = FindObjectOfType<Player>();
        
        player.selectedGOev.AddListener(ShowGOHUD);
        // DMBtn.SetActive(false);
        // DMMenu.SetActive(false);
        // DronUpgradeBtn.SetActive(false);
        UpdateDronsHUD();
    }

    void Update(){
        if (!player) return;
        if (resourcesSE == null) return;
        foreach (Resource resource in resourcesSE)
        {
            if (!resource.HUDGO) return;
            resource.HUDGO.GetComponent<TMP_Text>().text = resource.resourceEnum.ToString() + ": " + resource.quantity.ToString();
        }
    }
    
    public void ShowGOHUD(GameObject selectedGO){
        if (!player.GetMouseSelectorStatus()) return;
        CleanHUDContainer();
        Building selectedBuilding = selectedGO.GetComponent<Building>();
        if (selectedBuilding)
        {
            // if (selectedBuilding.data.buildingType == BuildingsEnum.MainBase) DronUpgradeBtn.SetActive(true);
            if (selectedBuilding.buildingType != null) selectedBuilding.GetComponent<IBuilding>().ShowHUD();
            if (selectedBuilding.data.storageBool) {
                SetDronManagerButton();
                ShowResourcesBuilding(selectedGO.GetComponent<Building>());
            }
        }
    }

    void ShowResourcesBuilding(Building selectedBuilding){
        resourcesSE = selectedBuilding.data.storage;
        foreach (Resource resource in resourcesSE)
        {
            bool validation = true;
            if (resource.quantity <= 0) {
                // check if extractor is mining this resource
                if (selectedBuilding.data.buildingType == BuildingsEnum.Extractor && selectedBuilding.resource == resource.resourceEnum){
                    validation = false;
                }

                // check if dron is delivering this resource
                foreach (var dron in selectedBuilding.data.setDrons)
                {
                    if (dron.resource.resourceEnum == resource.resourceEnum) validation = false;
                }

                if (validation)
                {
                    resourcesSE.Remove(resource);
                    return;
                }
            }
            GameObject newResource = Instantiate(resourcePrefab);
            newResource.GetComponent<TMP_Text>().text = resource.resourceEnum + ": " + resource.quantity.ToString();
            newResource.transform.SetParent(resourceContainer.transform, false);
            resource.HUDGO = newResource;
        }
    }

    void CleanHUDContainer(){
        // DMBtn.SetActive(false);
        // DronUpgradeBtn.SetActive(false);
        CleanButtonsPanel();
        resourcesSE = null;
        foreach (Transform child in resourceContainer.transform) {
            Destroy(child.gameObject);
        }
    }
    public void UpdateDronsHUD(){
        drons.text = "DRONS " + player.drons.ToString();
    }

    public void UpgradeDrons(){
        player.drons += 1;
        player.dronSpeed += 1;
        player.dronStorage += 2;
        UpdateDronsHUD();
    }

    public void GenerateButtons(List<string> buttonsList){
        foreach (string buttonName in buttonsList)
        {
            GameObject buttonGO = Instantiate(buttonPrefab, buildingButtpnsPanel);
            buttonGO.GetComponentInChildren<TMP_Text>().text = buttonName;
        }
    }

    void CleanButtonsPanel(){
        foreach (Transform child in buildingButtpnsPanel.transform) {
            Destroy(child.gameObject);
        }
    }

    void SetDronManagerButton() {
        Instantiate(dronMenuBtn, buildingButtpnsPanel);
        // GameObject buttonGO = Instantiate(buttonPrefab, buildingButtpnsPanel);
        // buttonGO.GetComponentInChildren<TMP_Text>().text = "Manage Drons";
        // SetDronManagerMenu(buttonGO.GetComponent<Button>());

    }

    // void SetDronManagerMenu(Button button) {
    //     GameObject dronManagerGO = Instantiate(dronManagerPrefab, buildingButtpnsPanel);
    //     button.onClick.AddListener(() => dronManagerGO.SetActive(!dronManagerGO.activeSelf));
    // }
}
