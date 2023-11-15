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

   public IEnumerator StartDronCoroutine(Dron dron){
      HUD hud = FindObjectOfType<HUD>();
      while (isActiveAndEnabled)
      {
         yield return new WaitForSeconds(player.dronSpeed);
         
         dron.resource.quantity = player.dronStorage;
         Resource storageResource = data.storage.Find(x => x.name == dron.resource.name);
         // if (storageResource == null) yield return new WaitForSeconds(player.dronSpeed);
         // when storage empty stop deliver
         if (storageResource.quantity >= dron.resource.quantity) {
            storageResource.quantity += -dron.resource.quantity;
            GameObject address = GameObject.Find(dron.destiny);
            List<Resource> addressStorage = address.GetComponent<Building>().data.storage;
            Resource addressR = addressStorage.Find(x => x.name == dron.resource.name);
            if (addressR != null)
            {
               addressR.quantity += dron.resource.quantity;
            }else{
               addressStorage.Add(new Resource(dron.resource.name,dron.resource.quantity));
               hud.ShowGOHUD(player.selectedGO);
            }
         }
         
      }
      // yield break;
   }

   public Coroutine StartDron(Dron dron){
      Coroutine coroutineInstance = StartCoroutine(StartDronCoroutine(dron));
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
