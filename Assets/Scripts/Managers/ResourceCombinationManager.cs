using System.Collections.Generic;
using UnityEngine;

public class ResourceCombinationManager : MonoBehaviour
{
    public List<ResourceCombination> elementCombinations;

    // public ElementsEnum? GetResult(ResourcesEnum res1, ResourcesEnum res2)
    // {
    //     foreach (ResourceCombination elementCombination in elementCombinations)
    //     {
    //         if (elementCombination.resource1 == res1 && elementCombination.resource2 == res2)
    //             return elementCombination.result;
    //     }
    //     return null; // or some default value indicating no match
    // }
}