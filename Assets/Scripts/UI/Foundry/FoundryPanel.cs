using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FoundryPanel : MonoBehaviour
{
    [SerializeField] GameObject foundryEntryPrefab;
    [SerializeField] Transform listPanel;
    ResourceCombinationManager resourceCombinationM;
    public Building buildingParent;
    public HUD hud;

    void CraftElement(ResourceCombination elementCombination)
    {
        // check resource
        if (!buildingParent.CheckResources(elementCombination)) return;
        // add to foundry storage elementCombination.result
        buildingParent.AddResource(elementCombination.result.gameMaterialSO, 1);
        // remove resources
        buildingParent.RemoveResources(elementCombination);
        // TODO showgohud should take autom the player selected object
        // update resourcesHUD
        hud.ShowGOHUD(buildingParent.gameObject);
    }

    public void GenerateListCombinations(GameMaterialTypesEnum combinationsResultType)
    {
        CleanListCombinations();
        resourceCombinationM = GetComponent<ResourceCombinationManager>();
        foreach (ResourceCombination elementCombination in resourceCombinationM.elementCombinations)
        {
            if (elementCombination.result.gameMaterialSO.type != combinationsResultType) continue;
            GameObject entry = Instantiate(foundryEntryPrefab, listPanel);
            FoundryEntry foundryEntry = entry.GetComponent<FoundryEntry>();
            foundryEntry.resource1Text.text = elementCombination.resource1.gameMaterialSO.materialName.ToString();
            if (elementCombination.resource2.gameMaterialSO)
            {
                foundryEntry.resource2Text.gameObject.SetActive(true);
                foundryEntry.resource2Text.text = elementCombination.resource2.gameMaterialSO.materialName.ToString();
            }
            else
            {
                foundryEntry.resource2Text.gameObject.SetActive(false);
            }
            foundryEntry.elementText.text = elementCombination.result.gameMaterialSO.materialName.ToString();

            entry.GetComponent<Button>().onClick.AddListener(() => CraftElement(elementCombination));
        }
    }

    void CleanListCombinations()
    {
        foreach (Transform entry in transform)
        {
            Destroy(entry.gameObject);
        }
    }


}
