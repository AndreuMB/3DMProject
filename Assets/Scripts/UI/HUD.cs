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

    void Start(){
        player = FindObjectOfType<Player>();
        if (!player) { Debug.LogError("Need MainBase gameobject in gamescene to work"); return; };
        
        player.selectedGO.AddListener(ShowGOHUD);
    }

    void Update(){
        if (!player) return;
        drons.text = "DRONS " + player.drons.ToString();
        if (resourcesSE == null) return;
        foreach (Resource resource in resourcesSE)
        {
            resource.HUDGO.GetComponent<TMP_Text>().text = resource.name + ": " + resource.quantity.ToString();
        }
    }

    // public void InstanciateResources(){
    //     List<Resource> resources = player.resources;
    //     foreach (Resource resource in resources)
    //     {
    //         GameObject newResource = Instantiate(resourcePrefab);
    //         newResource.GetComponent<TMP_Text>().text = resource.name + ": " + resource.quantity.ToString();
    //         newResource.transform.SetParent(resourceContainer.transform, false);
    //         resource.HUDGO = newResource;
    //     }
    // }
    
    void ShowGOHUD(GameObject selectedGO){
        print("enter showGOHUD");
        CleanHUDContainer();
        if (selectedGO.GetComponent<Building>())
        {
            ShowResourcesExtractor(selectedGO.GetComponent<Building>());
        }
    }

    void ShowResourcesExtractor(Building selectedExtractor){
        resourcesSE = selectedExtractor.data.storage;
        foreach (Resource resource in resourcesSE)
        {
            GameObject newResource = Instantiate(resourcePrefab);
            newResource.GetComponent<TMP_Text>().text = resource.name + ": " + resource.quantity.ToString();
            newResource.transform.SetParent(resourceContainer.transform, false);
            resource.HUDGO = newResource;
        }
    }

    void CleanHUDContainer(){
        resourcesSE = null;
        foreach (Transform child in resourceContainer.transform) {
            Destroy(child.gameObject);
        }
    }
}
