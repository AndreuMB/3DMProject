using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Resource
{
    public ResourcesEnum name;
    public float quantity;
    [System.NonSerialized]
    public GameObject HUDGO;

    public Resource(ResourcesEnum nameSet, float quantitySet)
   {
      name = nameSet;
      quantity = quantitySet;
   }
}
