using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BlenderPanel : MonoBehaviour
{
    [SerializeField] GameObject foundryEntryPrefab;
    [SerializeField] Transform listPanel;
    ResourceCombinationManager resourceCombinationM;
    [NonSerialized] public Building buildingParent;
    [NonSerialized] public HUD hud;

    void CraftElement(ResourceCombination elementCombination)
    {
        // check resource
        if (!buildingParent.CheckResources(elementCombination)) return;
        // add to foundry storage elementCombination.result
        buildingParent.AddResource(elementCombination.result.gameMaterialSO, 1);
        // remove resources
        buildingParent.RemoveResources(elementCombination);
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
            BlenderEntry foundryEntry = entry.GetComponent<BlenderEntry>();

            // foundryEntry.resource1Text.text = elementCombination.resource1.gameMaterialSO.materialName.ToString();
            // if (elementCombination.resource1.quantity > 1) foundryEntry.resource1Text.text += " x " + elementCombination.resource1.quantity;
            SetTextGameMaterialBlendEntry(foundryEntry.resource1Text, elementCombination.resource1);

            if (elementCombination.resource2.gameMaterialSO)
            {
                foundryEntry.resource2Text.gameObject.SetActive(true);
                foundryEntry.operationText.gameObject.SetActive(true);
                SetTextGameMaterialBlendEntry(foundryEntry.resource2Text, elementCombination.resource2);
            }
            else
            {
                foundryEntry.resource2Text.gameObject.SetActive(false);
                foundryEntry.operationText.gameObject.SetActive(false);
            }
            SetTextGameMaterialBlendEntry(foundryEntry.elementText, elementCombination.result);

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

    void SetTextGameMaterialBlendEntry(TMP_Text textEntry, GameMaterial gameMaterial)
    {
        textEntry.text = gameMaterial.gameMaterialSO.materialName.ToString();
        if (gameMaterial.quantity > 1) textEntry.text += " x " + gameMaterial.quantity;
    }


}
