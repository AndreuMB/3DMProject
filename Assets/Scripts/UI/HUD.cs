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

    void Start(){
        player = FindObjectOfType<Player>();
        if (!player) { Debug.LogError("Need MainBase gameobject in gamescene to work"); return; };
        List<Resource> resources = player.resources;
        foreach (Resource resource in resources)
        {
            GameObject newResource = Instantiate(resourcePrefab);
            newResource.GetComponent<TMP_Text>().text = resource.name + ": " + resource.quantity.ToString();
            newResource.transform.SetParent(resourceContainer.transform, false);
            resource.HUDGO = newResource;
        }
    }

    void Update(){
        if (!player) return;
        drons.text = "DRONS " + player.drons.ToString();
        List<Resource> resources = player.resources;
        foreach (Resource resource in resources)
        {
            resource.HUDGO.GetComponent<TMP_Text>().text = resource.name + ": " + resource.quantity.ToString();
        }
    }

    public void InstanciateResources(){
        List<Resource> resources = player.resources;
        foreach (Resource resource in resources)
        {
            GameObject newResource = Instantiate(resourcePrefab);
            newResource.GetComponent<TMP_Text>().text = resource.name + ": " + resource.quantity.ToString();
            newResource.transform.SetParent(resourceContainer.transform, false);
            resource.HUDGO = newResource;
        }
    }
}
