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
   HUD hud;
   bool formatB = true;
   
   // Start is called before the first frame update
   void Start()
   {
      player = FindObjectOfType<Player>();
      hud = FindObjectOfType<HUD>();
      Format();
   }

   IEnumerator ExtractResource(){
      while (isActiveAndEnabled) 
      {
         yield return new WaitForSeconds(data.rate);
         if (data.storage.Count > 0){
            // when storage full stop producing
            if (data.storage[0].quantity < data.maxStorage) data.storage[0].quantity+=data.quantity;
         }else{
            data.storage.Add(new Resource(resource,data.quantity));
            hud.ShowGOHUD(player.selectedGO);
         }
         
      }
      yield break;
   }

   public IEnumerator StartDronCoroutine(Dron dron){
      
      float distance = dron.getDistance();
      bool firstTime = true;
      
      while (isActiveAndEnabled)
      {

         Resource storageResource = data.storage.Find(x => x.name == dron.resource.name);
         if (storageResource == null) yield return null;
         
         // check storage quantity and choose dron storage quantity
         dron.resource.quantity = storageResource.quantity > player.dronStorage ? player.dronStorage : storageResource.quantity;

         storageResource.quantity += -dron.resource.quantity;

         if (firstTime)
         {
            yield return new WaitForSeconds(distance/player.dronSpeed);
            firstTime = false;
         }else{
            yield return new WaitForSeconds((distance/player.dronSpeed)*2);
         }
         
         List<Resource> addressStorage = dron.destiny.GetComponent<Building>().data.storage;
         Resource addressR = addressStorage.Find(x => x.name == dron.resource.name);
         if (addressR != null)
         {
            addressR.quantity += dron.resource.quantity;
         }else{
            addressStorage.Add(new Resource(dron.resource.name,dron.resource.quantity));
            hud.ShowGOHUD(player.selectedGO);
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

   public void SetBuildType(BuildingsEnum bType){
      data.buildingType = bType;
      Format();
   }

   void Format(){
      if (!formatB) return;
      switch (data.buildingType)
      {
         case BuildingsEnum.Extractor:
            data.storageBool = true;
            data.storage.Add(new Resource(resource,0));
            StartCoroutine(nameof(ExtractResource));
            // GetComponent<MeshRenderer>().material.color = Color.green;
            break;
         case BuildingsEnum.Storage:
            GetComponent<MeshRenderer>().material.color = Color.blue;
            data.storageBool = true;
            break;
         case BuildingsEnum.MainBase:
            GetComponent<MeshRenderer>().material.color = Color.red;
            break;
         default:
            break;
      }
      formatB = false;
   }

   public void SetModel(GameObject model){
      GetComponent<MeshFilter>().mesh = model.GetComponentInChildren<MeshFilter>().sharedMesh;
      GetComponent<MeshRenderer>().materials = model.GetComponentInChildren<MeshRenderer>().sharedMaterials;
      transform.localScale = model.transform.localScale;
      transform.rotation = model.transform.rotation;
   }
}

public enum BuildingsEnum{
   Extractor,
   Storage,
   MainBase
}
