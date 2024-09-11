using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
public class Building : MonoBehaviour
{
   public GameMaterialSO resourceSO;
   public BuildingData data;
   Player player;
   MainBase mainBase;
   HUD hud;
   public IBuilding buildingType;
   bool formatB = true;
   BuildingsUtilsPrefabManager buildingsUtilsPrefabManager;

   // Start is called before the first frame update
   void Start()
   {
      player = FindObjectOfType<Player>();
      mainBase = FindObjectOfType<MainBase>();
      hud = FindObjectOfType<HUD>();
      Format();
   }

   IEnumerator ExtractResource()
   {
      while (isActiveAndEnabled)
      {
         yield return new WaitForSeconds(data.rate);
         // if (data.storage.Count > 0)
         // {
         //    // when storage full stop producing
         //    if (data.storage[0].quantity < data.maxStorage) data.storage[0].quantity += data.quantity;
         // }
         // else
         // {
         //    data.storage.Add(new Resource(resource.ToString(), data.quantity));
         // }
         AddResource(data.storage[0].gameMaterialSO, data.quantity);
         hud.ShowGOHUD(player.selectedGO);

      }
      yield break;
   }

   public void StopDron(Coroutine dronCoroutine)
   {
      StopCoroutine(dronCoroutine);
   }

   public void SetBuildType(BuildingsEnum bType)
   {
      data.buildingType = bType;
      buildingsUtilsPrefabManager = FindObjectOfType<BuildingsUtilsPrefabManager>();
      Format();
   }

   void Format()
   {
      if (!formatB) return;
      switch (data.buildingType)
      {
         case BuildingsEnum.MainBase:
            MainBase mainBase = gameObject.AddComponent<MainBase>();
            buildingType = mainBase;
            break;
         case BuildingsEnum.Extractor:
            data.storageBool = true;
            PlacementSystem placementSystem = FindObjectOfType<PlacementSystem>();
            GameMaterialSO gameMaterialSO = placementSystem.buildingtState.GetOreResource(transform.parent.position);
            data.storage.Add(new GameMaterial(gameMaterialSO, 0));
            resourceSO = gameMaterialSO;
            StartCoroutine(nameof(ExtractResource));
            break;
         case BuildingsEnum.Storage:
            data.storageBool = true;
            break;
         case BuildingsEnum.Foundry:
            data.storageBool = true;
            Foundry foundry = gameObject.AddComponent<Foundry>();
            buildingType = foundry;
            GameObject foundryPanel = buildingsUtilsPrefabManager.GetPrefab("FoundryPanel");
            foundry.SetFoundryPanel(foundryPanel);
            break;
      }
      formatB = false;
   }

   public void SetModel(GameObject model)
   {
      transform.GetChild(0).GetComponent<MeshFilter>().mesh = model.GetComponentInChildren<MeshFilter>().sharedMesh;
      transform.GetChild(0).GetComponent<MeshRenderer>().materials = model.GetComponentInChildren<MeshRenderer>().sharedMaterials;
      transform.GetChild(0).localScale = model.transform.localScale;
      transform.GetChild(0).rotation = model.transform.rotation;
   }

   public IEnumerator StartDronCoroutineV2(Dron dron)
   {

      if (!mainBase) mainBase = FindObjectOfType<MainBase>();
      float distance = dron.GetDistance();
      dron.speed = mainBase.dronSpeed;

      yield return new WaitForSeconds(distance / dron.speed);

      if (dron.movingTo == dron.destination.transform.position)
      {

         if (dron.newMaterial != null) dron.material = dron.newMaterial;

         GameMaterial storageResource = FindGameMaterialInStorage(dron.material.gameMaterialSO.materialName);
         if (storageResource == null) yield return null;

         // check storage quantity and choose dron storage quantity
         dron.material.quantity = storageResource.quantity > mainBase.dronStorage ? mainBase.dronStorage : storageResource.quantity;

         storageResource.quantity += -dron.material.quantity;
      }
      else
      {
         if (dron.detele)
         {
            mainBase.SetDrons(mainBase.drons + 1);
            List<DronData> listDrons = player.selectedGO.GetComponent<Building>().data.setDrons;
            listDrons.Remove(dron.dronData);
            if (dron.row) Destroy(dron.row);
            Destroy(dron.gameObject);
            yield return null;
         }
         List<GameMaterial> addressStorage = dron.destination.GetComponent<Building>().data.storage;
         GameMaterialsEnum gameMaterialsEnum = dron.material.gameMaterialSO.materialName;
         GameMaterial addressR = FindGameMaterialInStorage(gameMaterialsEnum, addressStorage);
         if (addressR != null)
         {
            addressR.quantity += dron.material.quantity;
         }
         else
         {
            addressStorage.Add(new GameMaterial(dron.material.gameMaterialSO, dron.material.quantity));
         }
      }

   }

   GameMaterial FindGameMaterialInStorage(GameMaterialsEnum gameMaterialsEnum, List<GameMaterial> storage = null)
   {
      storage ??= data.storage;
      return storage.Find(x => x.gameMaterialSO.materialName == gameMaterialsEnum);
   }

   public void StartDronV2(Dron dron)
   {
      // Coroutine coroutineInstance = StartCoroutine(StartDronCoroutine(dron));
      dron.dronGoal.AddListener(() => StartCoroutine(StartDronCoroutineV2(dron)));
   }

   public void AddResource(GameMaterialSO gameMaterialSO, int quantity)
   {
      // if (data.storage.Count <= 0) return;

      GameMaterial storagedResource = data.storage.Find(
         resource => resource.gameMaterialSO.materialName == gameMaterialSO.materialName);
      if (storagedResource != null)
      {
         // when storage full stop producing
         if (storagedResource.quantity < data.maxStorage) storagedResource.quantity += quantity;
      }
      else
      {
         data.storage.Add(new GameMaterial(gameMaterialSO, quantity));
      }

   }

   public bool CheckResources(ResourceCombination elementCombination)
   {
      GameMaterial storagedMaterial1 = FindGameMaterialInStorage(elementCombination.resource1.gameMaterialSO.materialName);
      if (storagedMaterial1 == null || storagedMaterial1.quantity < elementCombination.resource1.quantity) return false;

      // if only one element
      if (elementCombination.resource2.gameMaterialSO == null) return true;

      GameMaterial storagedMaterial2 = FindGameMaterialInStorage(elementCombination.resource2.gameMaterialSO.materialName);
      if (storagedMaterial2 == null || storagedMaterial2.quantity < elementCombination.resource2.quantity) return false;

      return true;
   }

   public void RemoveResources(ResourceCombination elementCombination)
   {
      GameMaterial storagedMaterial1 = FindGameMaterialInStorage(elementCombination.resource1.gameMaterialSO.materialName);
      storagedMaterial1.quantity -= elementCombination.resource1.quantity;

      // if only one element
      if (elementCombination.resource2.gameMaterialSO == null) return;

      GameMaterial storagedMaterial2 = FindGameMaterialInStorage(elementCombination.resource2.gameMaterialSO.materialName);
      storagedMaterial2.quantity -= elementCombination.resource2.quantity;
   }

}

public enum BuildingsEnum
{
   MainBase,
   Extractor,
   Storage,
   Foundry,
}
