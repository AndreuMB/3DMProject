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
    [SerializeField] float storage;
    Player player;
    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<Player>();
        StartCoroutine(nameof(ExtractResource));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator ExtractResource(){
        if (!player) yield break;
        while (isActiveAndEnabled)
        {
            yield return new WaitForSeconds(rate);
            player.resources[(int)resource].quantity+=quantity;  
        }
        yield break;
    }
}
