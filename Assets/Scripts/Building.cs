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
      while (isActiveAndEnabled) 
      {
         yield return new WaitForSeconds(data.rate);
         // when storage full stop producing
         if (data.storage[0].quantity <= data.maxStorage) data.storage[0].quantity+=data.quantity;
         
      }
      yield break;
   }

   public IEnumerator StartDronCoroutine(string dronDestiny){ // STOPs when dronmenu close
      int rQuantity = player.dronStorage;
      while (isActiveAndEnabled)
      {
         yield return new WaitForSeconds(player.dronSpeed);
         // when storage empty stop deliver
         if (data.storage[0].quantity >= rQuantity) {
            data.storage[0].quantity += -rQuantity;
            GameObject address = GameObject.Find(dronDestiny);
            List<Resource> addressStorage = address.GetComponent<Building>().data.storage;
            Resource addressR = addressStorage.Find(x => x.name == data.storage[0].name);
            if (addressR != null)
            {
               addressR.quantity += rQuantity;
            }else{
               addressStorage.Add(new Resource(data.storage[0].name,rQuantity));
            }
         }
         
      }
      // yield break;
   }

   public Coroutine StartDron(string dronDestiny){
      Coroutine coroutineInstance = StartCoroutine(StartDronCoroutine(dronDestiny));
      return coroutineInstance;
   }

   public void StopDron(Coroutine dronCoroutine){
      StopCoroutine(dronCoroutine);
   }
}

public enum BuildingsEnum{
   Extractor,
   Storage,
   MainBase
}
