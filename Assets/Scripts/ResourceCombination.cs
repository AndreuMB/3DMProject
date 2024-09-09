using UnityEngine;

[CreateAssetMenu(fileName = "NewResourceCombination", menuName = "Combination")]
public class ResourceCombination : ScriptableObject
{
    public GameMaterial resource1;
    public GameMaterial resource2;
    public GameMaterial result;
}