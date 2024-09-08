using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
[Serializable]
public class Extractor : MonoBehaviour
{
    // // this will be in the grid
    // public ResourcesEnum resource;
    // public BuildingData data;
    // // [SerializeField] float quantity;
    // // [SerializeField] float rate;
    // // public List<Resource> storage;
    // // // float inventory;
    // // [SerializeField] float maxStorage;
    // Player player;
    // // Start is called before the first frame update
    // void Start()
    // {
    //     player = FindObjectOfType<Player>();
    //     data.storage.Add(new Resource(resource.ToString().FirstCharacterToUpper(), 0));
    //     StartCoroutine(nameof(ExtractResource));
    // }

    // IEnumerator ExtractResource()
    // {
    //     // if (!player) yield break;
    //     while (isActiveAndEnabled && data.storage[0].quantity <= data.maxStorage) // when storage full stop producing
    //     {
    //         yield return new WaitForSeconds(data.rate);
    //         // player.resources[(int)resource].quantity+=quantity;
    //         data.storage[0].quantity += data.quantity;
    //     }
    //     yield break;
    // }
}
