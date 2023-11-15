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
    [SerializeField] GameObject DMBtn;
    public GameObject DMMenu;
    [SerializeField] GameObject DronUpgradeBtn;

    void Start(){
        player = FindObjectOfType<Player>();
        if (!player) { Debug.LogError("Need MainBase gameobject in gamescene to work"); return; };
        
        player.selectedGOev.AddListener(ShowGOHUD);
        DMBtn.SetActive(false);
        DMMenu.SetActive(false);
        DronUpgradeBtn.SetActive(false);
        UpdateDronsHUD();
    }

    void Update(){
        if (!player) return;
        if (resourcesSE == null) return;
        foreach (Resource resource in resourcesSE)
        {
            if (!resource.HUDGO) return;
            resource.HUDGO.GetComponent<TMP_Text>().text = resource.name + ": " + resource.quantity.ToString();
        }
    }
    
    public void ShowGOHUD(GameObject selectedGO){
        CleanHUDContainer();
        Building selectedBuilding = selectedGO.GetComponent<Building>();
        if (selectedBuilding)
        {
            if (selectedBuilding.data.storageBool) DMBtn.SetActive(true);
            if (selectedBuilding.data.buildingType == BuildingsEnum.MainBase) DronUpgradeBtn.SetActive(true);
            ShowResourcesExtractor(selectedGO.GetComponent<Building>());
        }
    }

    void ShowResourcesExtractor(Building selectedExtractor){
        resourcesSE = selectedExtractor.data.storage;
        foreach (Resource resource in resourcesSE)
        {
            if (resource.quantity <= 0) {
                resourcesSE.Remove(resource);
                return;
            }
            GameObject newResource = Instantiate(resourcePrefab);
            newResource.GetComponent<TMP_Text>().text = resource.name + ": " + resource.quantity.ToString();
            newResource.transform.SetParent(resourceContainer.transform, false);
            resource.HUDGO = newResource;
        }
    }

    void CleanHUDContainer(){
        DMBtn.SetActive(false);
        DronUpgradeBtn.SetActive(false);
        resourcesSE = null;
        foreach (Transform child in resourceContainer.transform) {
            Destroy(child.gameObject);
        }
    }

    public void ManageDrons(){
        DMMenu.SetActive(!DMMenu.activeInHierarchy);
    }

    public void UpdateDronsHUD(){
        drons.text = "DRONS " + player.drons.ToString();
    }

    public void UpgradeDrons(){
        player.drons += 1;
        player.dronSpeed -= 1;
        player.dronStorage += 2;
        UpdateDronsHUD();
    }
}
