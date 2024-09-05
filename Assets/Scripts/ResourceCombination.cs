using UnityEngine;

[CreateAssetMenu(fileName = "NewResourceCombination", menuName = "Combination")]
public class ResourceCombination : ScriptableObject
{
    public ResourcesEnum resource1;
    public ResourcesEnum resource2;
    public ElementsEnum result;
}