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

   // public IEnumerator StartDronCoroutine(Dron dron) {
      
   //    if (!player) player = FindObjectOfType<Player>();
   //    float distance = dron.GetDistance();
      
   //    while (isActiveAndEnabled)
   //    {
   //       dron.speed = player.dronSpeed;
   //       Resource storageResource = data.storage.Find(x => x.name == dron.resource.name);
   //       if (storageResource == null) yield return null;


   //       if (dron.movingTo == dron.destination.transform.position) {
   //          // check storage quantity and choose dron storage quantity
   //          dron.resource.quantity = storageResource.quantity > player.dronStorage ? player.dronStorage : storageResource.quantity;

   //          storageResource.quantity += -dron.resource.quantity;
   //       }else{
   //          List<Resource> addressStorage = dron.destination.GetComponent<Building>().data.storage;
   //          Resource addressR = addressStorage.Find(x => x.name == dron.resource.name);
   //          if (addressR != null)
   //          {
   //             addressR.quantity += dron.resource.quantity;
   //          }else{
   //             addressStorage.Add(new Resource(dron.resource.name,dron.resource.quantity));
   //             // hud.ShowGOHUD(player.selectedGO);
   //          }
   //       }

         

   //       yield return new WaitForSeconds(distance/dron.speed);
         
   //       distance = dron.GetDistance();
   //       if (distance <= 1)
   //       {
   //          yield return new WaitForSeconds(.5f);
   //          distance = dron.GetDistance();
   //       }

   //       if (dron.movingTo == dron.destination.transform.position) {
   //          // check storage quantity and choose dron storage quantity
   //          dron.resource.quantity = storageResource.quantity > player.dronStorage ? player.dronStorage : storageResource.quantity;

   //          storageResource.quantity += -dron.resource.quantity;
   //       }else{
   //          List<Resource> addressStorage = dron.destination.GetComponent<Building>().data.storage;
   //          Resource addressR = addressStorage.Find(x => x.name == dron.resource.name);
   //          if (addressR != null)
   //          {
   //             addressR.quantity += dron.resource.quantity;
   //          }else{
   //             addressStorage.Add(new Resource(dron.resource.name,dron.resource.quantity));
   //             // hud.ShowGOHUD(player.selectedGO);
   //          }
   //       }
         
   //       yield return new WaitForSeconds(distance/dron.speed);
         
   //       distance = dron.GetDistance();
   //       if (distance <= 1)
   //       {
   //          yield return new WaitForSeconds(.5f);
   //          distance = dron.GetDistance();
   //       }
         
   //    }
   // }

   // public Coroutine StartDron(Dron dron){
   //    Coroutine coroutineInstance = StartCoroutine(StartDronCoroutine(dron));
   //    return coroutineInstance;
   // }

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
            
            ResourceCells rCells = FindObjectOfType<ResourceCells>();
            
            foreach (ResourceCell rc in rCells.resourceCells)
            {
               if (transform.parent == null) break;
               if (transform.parent.position == rc.cellPosition) {
                  resource = rc.resource;
                  break;
               }
            }

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
         case BuildingsEnum.Factory:
                GetComponent<MeshRenderer>().material.color = Color.white;
                break;
         case BuildingsEnum.Forge:
                GetComponent<MeshRenderer>().material.color = Color.grey;
                break;
            default:
            break;
      }
      formatB = false;
   }

   public void SetModel(GameObject model){
      transform.GetChild(0).GetComponent<MeshFilter>().mesh = model.GetComponentInChildren<MeshFilter>().sharedMesh;
      transform.GetChild(0).GetComponent<MeshRenderer>().materials = model.GetComponentInChildren<MeshRenderer>().sharedMaterials;
      transform.GetChild(0).localScale = model.transform.localScale;
      transform.GetChild(0).rotation = model.transform.rotation;
   }

   public IEnumerator StartDronCoroutineV2(Dron dron) {
      
      if (!player) player = FindObjectOfType<Player>();
      float distance = dron.GetDistance();
      dron.speed = player.dronSpeed;

      yield return new WaitForSeconds(distance/dron.speed);
      
      if (dron.movingTo == dron.destination.transform.position) {
         
         // print("enter dron");
         if (dron.newResource != null) dron.resource = dron.newResource;

         Resource storageResource = data.storage.Find(x => x.name == dron.resource.name);
         if (storageResource == null) yield return null;

         // check storage quantity and choose dron storage quantity
         dron.resource.quantity = storageResource.quantity > player.dronStorage ? player.dronStorage : storageResource.quantity;

         storageResource.quantity += -dron.resource.quantity;
      }else{
         if (dron.detele) {
            player.SetDrons(player.drons+1);
            List<DronData> listDrons = player.selectedGO.GetComponent<Building>().data.setDrons;
            listDrons.Remove(dron.dronData);
            if (dron.row) Destroy(dron.row);
            Destroy(dron.gameObject);
            yield return null;
         }
         List<Resource> addressStorage = dron.destination.GetComponent<Building>().data.storage;
         Resource addressR = addressStorage.Find(x => x.name == dron.resource.name);
         if (addressR != null)
         {
            addressR.quantity += dron.resource.quantity;
         }else{
            addressStorage.Add(new Resource(dron.resource.name,dron.resource.quantity));
         }
      }
      
   }

   public void StartDronV2(Dron dron){
      // Coroutine coroutineInstance = StartCoroutine(StartDronCoroutine(dron));
      dron.dronGoal.AddListener(()=>StartCoroutine(StartDronCoroutineV2(dron)));
   }

}

public enum BuildingsEnum{
   Extractor,
   Storage,
   MainBase,
   Forge,
   Factory
}
