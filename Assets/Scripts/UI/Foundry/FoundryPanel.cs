using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FoundryPanel : MonoBehaviour
{
    [SerializeField] GameObject foundryEntryPrefab;
    [SerializeField] Transform listPanel;
    ResourceCombinationManager resourceCombinationM;
    public Foundry foundryParent;
    public HUD hud;
    Building buildingRef;

    void Start()
    {
        buildingRef = foundryParent.GetComponent<Building>();
        resourceCombinationM = GetComponent<ResourceCombinationManager>();
        foreach (ResourceCombination elementCombination in resourceCombinationM.elementCombinations)
        {
            GameObject entry = Instantiate(foundryEntryPrefab, listPanel);
            FoundryEntry foundryEntry = entry.GetComponent<FoundryEntry>();
            foundryEntry.resource1Text.text = elementCombination.resource1.gameMaterialSO.materialName.ToString();
            if (elementCombination.resource2.gameMaterialSO)
            {
                foundryEntry.resource2Text.text = elementCombination.resource2.gameMaterialSO.materialName.ToString();
            }
            foundryEntry.elementText.text = elementCombination.result.gameMaterialSO.materialName.ToString();

            entry.GetComponent<Button>().onClick.AddListener(() => CraftElement(elementCombination));
        }
    }

    void CraftElement(ResourceCombination elementCombination)
    {
        // check resource
        if (!buildingRef.CheckResources(elementCombination)) return;
        // add to foundry storage elementCombination.result
        buildingRef.AddResource(elementCombination.result.gameMaterialSO, 1);
        // remove resources
        buildingRef.RemoveResources(elementCombination);
        // TODO showgohud should take autom the player selected object
        hud.ShowGOHUD(foundryParent.gameObject);
    }


}