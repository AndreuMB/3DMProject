using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Resource
{
   public string resourceEnum;
   public int quantity;
   [System.NonSerialized]
   public GameObject HUDGO;

   public Resource(string resource, int quantitySet)
   {
      resourceEnum = resource;
      quantity = quantitySet;
   }
}
