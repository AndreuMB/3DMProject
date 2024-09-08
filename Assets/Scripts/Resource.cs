using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Resource
{
    public ResourcesEnum resourceEnum;
    public float quantity;
    [System.NonSerialized]
    public GameObject HUDGO;

    public Resource(ResourcesEnum resource, float quantitySet)
   {
      resourceEnum = resource;
      quantity = quantitySet;
   }
}
