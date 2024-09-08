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

    void Start()
    {
        resourceCombinationM = GetComponent<ResourceCombinationManager>();
        foreach (ResourceCombination elementCombination in resourceCombinationM.elementCombinations)
        {
            GameObject entry = Instantiate(foundryEntryPrefab, listPanel);
            FoundryEntry foundryEntry = entry.GetComponent<FoundryEntry>();
            foundryEntry.resource1Text.text = elementCombination.resource1.ToString();
            foundryEntry.resource2Text.text = elementCombination.resource2.ToString();
            foundryEntry.elementText.text = elementCombination.result.ToString();

            entry.GetComponent<Button>().onClick.AddListener(() => CraftElement(elementCombination));
        }
    }

    void CraftElement(ResourceCombination elementCombination)
    {
        // add to foundry storage elementCombination.result
        foundryParent.GetComponent<Building>().AddResource(elementCombination.result.ToString(), 1);
        // TODO showgohud should take autom the player selected object
        hud.ShowGOHUD(foundryParent.gameObject);
    }
}
