using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Extractor : MonoBehaviour
{
    // this will be in the grid
    public ResourcesEnum resource;
    [SerializeField] float quantity;
    [SerializeField] float rate;
    [SerializeField] Resource storage;
    // float inventory;
    [SerializeField] float maxStorage;
    Player player;
    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<Player>();
        StartCoroutine(nameof(ExtractResource));
        storage.name = resource;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator ExtractResource(){
        // if (!player) yield break;
        while (isActiveAndEnabled && storage.quantity <= maxStorage) // when storage full stop producing
        {
            yield return new WaitForSeconds(rate);
            // player.resources[(int)resource].quantity+=quantity;
            storage.quantity+=quantity;
        }
        yield break;
    }
}
