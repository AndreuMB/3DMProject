using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Storage : MonoBehaviour
{
    public List<Resource> storage;
    public float inventory;
    [SerializeField] float maxStorage;
    Player player;
    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<Player>();
        Deposit(new Resource(ResourcesEnum.silver,10));
    }

    void Deposit(Resource resource){
        if (inventory >= maxStorage) return;
        storage.Add(resource);
        inventory += resource.quantity;
    }

}
