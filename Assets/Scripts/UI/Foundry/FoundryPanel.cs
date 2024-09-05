using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoundryPanel : MonoBehaviour
{
    [SerializeField] GameObject foundryEntryPrefab;
    [SerializeField] Transform listPanel;
    ResourceCombinationManager resourceCombinationM;
    // Start is called before the first frame update
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
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
