using UnityEngine;
using System.Collections.Generic;

public class BuildingsUtilsPrefabManager : MonoBehaviour
{
    [SerializeField] private List<GameObject> prefabs;

    // Optionally, use a dictionary to map prefabs by name or type for quick access
    private Dictionary<string, GameObject> prefabDictionary;

    void Awake()
    {
        // Initialize the dictionary if needed
        prefabDictionary = new Dictionary<string, GameObject>();
        foreach (var prefab in prefabs)
        {
            prefabDictionary[prefab.name] = prefab;
        }
    }

    public GameObject GetPrefab(string prefabName)
    {
        prefabDictionary.TryGetValue(prefabName, out GameObject prefab);
        return prefab;
    }
}