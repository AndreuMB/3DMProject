using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Building : MonoBehaviour
{
   // this will be in the grid
   public ResourcesEnum resource;
   public BuildingData data;
   Player player;
   
   // Start is called before the first frame update
   void Start()
   {
      player = FindObjectOfType<Player>();
      switch (data.buildingType)
      {
         case BuildingsEnum.Extractor:
            data.storage.Add(new Resource(resource,0));
            StartCoroutine(nameof(ExtractResource));
            GetComponent<MeshRenderer>().material.color = Color.green;
            break;
         case BuildingsEnum.Storage:
            GetComponent<MeshRenderer>().material.color = Color.blue;
            break;
         case BuildingsEnum.MainBase:
            GetComponent<MeshRenderer>().material.color = Color.red;
            break;
         default:
            break;
      }
   }

   IEnumerator ExtractResource(){
      // if (!player) yield break;
      while (isActiveAndEnabled && data.storage[0].quantity <= data.maxStorage) // when storage full stop producing
      {
         yield return new WaitForSeconds(data.rate);
         data.storage[0].quantity+=data.quantity;
      }
      yield break;
   }
}

public enum BuildingsEnum{
   Extractor,
   Storage,
   MainBase
}
