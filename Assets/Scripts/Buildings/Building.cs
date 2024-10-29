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
   public HUD hud;
   public IBuilding buildingType;
   BuildingsUtilsPrefabManager buildingsUtilsPrefabManager;
   public bool placingOnGoing = true;
   Material[] materials;

   // Start is called before the first frame update
   void Start()
   {
      player = FindObjectOfType<Player>();
      mainBase = FindObjectOfType<MainBase>();
      hud = FindObjectOfType<HUD>();
      buildingsUtilsPrefabManager = FindObjectOfType<BuildingsUtilsPrefabManager>();
      Format();
      if (hud)
      {
         player.SetActiveGO(gameObject);
      }
   }

   IEnumerator ExtractResource()
   {
      while (isActiveAndEnabled)
      {
         yield return new WaitForSeconds(data.rate);
         AddResource(data.storage[0].gameMaterialSO, data.quantity);
      }
      yield break;
   }

   public void StopDron(Coroutine dronCoroutine)
   {
      StopCoroutine(dronCoroutine);
   }

   public void SetBuildType(BuildingsEnum bType, bool completeBuilding)
   {
      data.buildingType = bType;
      placingOnGoing = completeBuilding;
   }

   void Format()
   {
      print("data.buildingType = " + data.buildingType);
      switch (data.buildingType)
      {
         case BuildingsEnum.MainBase:
            MainBase mainBase = gameObject.AddComponent<MainBase>();
            mainBase.hud = hud;
            buildingType = mainBase;
            break;
         case BuildingsEnum.Extractor:
            data.storageBool = true;
            PlacementSystem placementSystem = FindObjectOfType<PlacementSystem>();
            GameMaterialSO gameMaterialSO = placementSystem.buildingState.GetOreResource(transform.parent.position);
            data.storage.Add(new GameMaterial(gameMaterialSO, 0));
            resourceSO = gameMaterialSO;
            // StartCoroutine(nameof(ExtractResource));
            break;
         case BuildingsEnum.Storage:
            data.storageBool = true;
            break;
         case BuildingsEnum.Refinery:
            SetBlender(GameMaterialTypesEnum.refined);
            break;
         case BuildingsEnum.Foundry:
            SetBlender(GameMaterialTypesEnum.element);
            break;
      }

      if (data.storageBool && placingOnGoing)
      {
         ObjectData buildingInfo = FindObjectOfType<PlacementSystem>().database.objectsData.Find(x => x.Type == data.buildingType);
         List<GameMaterial> gameMaterialsBuild = buildingInfo.GameMaterialsBuild;
         gameMaterialsBuild.ForEach(material => AddResource(material.gameMaterialSO, -material.quantity));
      }
   }

   public void SetModel(GameObject model, Material placeholderMaterial)
   {
      // set model
      transform.GetChild(0).GetComponent<MeshFilter>().mesh = model.GetComponentInChildren<MeshFilter>().sharedMesh;
      materials = model.GetComponentInChildren<MeshRenderer>().sharedMaterials;

      // set placeholder material
      Material[] materialToArrayMaterials = new Material[1];
      materialToArrayMaterials[0] = placeholderMaterial;
      transform.GetChild(0).GetComponent<MeshRenderer>().materials = materialToArrayMaterials;

      transform.GetChild(0).localScale = model.transform.localScale;
      transform.GetChild(0).rotation = model.transform.rotation;
      if (!placingOnGoing) CompleteBuilding();

   }

   public void CompleteBuilding()
   {
      // set materials
      transform.GetChild(0).GetComponent<MeshRenderer>().materials = materials;
      placingOnGoing = false;
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

         GameMaterial storageResource = FindGameMaterialInStorage(dron.material.gameMaterialSO.materialName, data.storage);
         if (storageResource != null && storageResource.quantity > 0)
         {
            if (dron.successDelivery)
            {
               // check storage quantity and choose dron storage quantity
               dron.material.quantity = storageResource.quantity > mainBase.dronStorage ? mainBase.dronStorage : storageResource.quantity;

               storageResource.quantity += -dron.material.quantity;
            }
            dron.successDelivery = true;
         }
         else
         {
            dron.detele = true;
         }



      }
      else
      {
         if (dron.detele)
         {
            mainBase.SetDrons(mainBase.drons + 1);
            List<DronData> listDrons = data.setDrons;
            listDrons.Remove(dron.dronData);
            if (dron.row) Destroy(dron.row);
            Destroy(dron.gameObject);
            yield return null;
         }
         Building addressBuilding = dron.destination.GetComponent<Building>();
         List<GameMaterial> addressStorage = addressBuilding.data.storage;
         GameMaterialsEnum gameMaterialsEnum = dron.material.gameMaterialSO.materialName;
         GameMaterial addressMaterial = FindGameMaterialInStorage(gameMaterialsEnum, addressStorage);
         if (addressBuilding.placingOnGoing)
         {
            if (addressMaterial == null)
            {
               dron.successDelivery = false;
            }
            else
            {
               if (addressMaterial.quantity >= 0)
               {
                  dron.successDelivery = false;
               }
               else
               {
                  addressMaterial.quantity += dron.material.quantity;
                  // check if complete
                  bool complete = true;
                  foreach (GameMaterial gameMaterial in addressStorage)
                  {
                     if (gameMaterial.quantity < 0) complete = false;
                  }
                  if (complete)
                  {
                     dron.detele = true;
                     addressBuilding.CompleteBuilding();
                     if (addressBuilding.data.buildingType == BuildingsEnum.Extractor)
                        addressBuilding.StartCoroutine(nameof(addressBuilding.ExtractResource));
                  }
               }
            }
         }
         else
         {
            if (addressMaterial != null)
            {
               addressMaterial.quantity += dron.material.quantity;
            }
            else
            {
               addressStorage.Add(new GameMaterial(dron.material.gameMaterialSO, dron.material.quantity));
               if (player.selectedGO == dron.destination) hud.ShowGOHUD(player.selectedGO);
            }
         }
      }

   }

   GameMaterial FindGameMaterialInStorage(GameMaterialsEnum gameMaterialsEnum, List<GameMaterial> storage)
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
      GameMaterial storagedMaterial1 = FindGameMaterialInStorage(elementCombination.resource1.gameMaterialSO.materialName, data.storage);
      if (storagedMaterial1 == null || storagedMaterial1.quantity < elementCombination.resource1.quantity) return false;
      // if only one element
      if (elementCombination.resource2.gameMaterialSO == null) return true;
      GameMaterial storagedMaterial2 = FindGameMaterialInStorage(elementCombination.resource2.gameMaterialSO.materialName, data.storage);
      if (storagedMaterial2 == null || storagedMaterial2.quantity < elementCombination.resource2.quantity) return false;

      return true;
   }

   public void RemoveResources(ResourceCombination elementCombination)
   {
      GameMaterial storagedMaterial1 = FindGameMaterialInStorage(elementCombination.resource1.gameMaterialSO.materialName, data.storage);
      storagedMaterial1.quantity -= elementCombination.resource1.quantity;

      // if only one element
      if (elementCombination.resource2.gameMaterialSO == null) return;

      GameMaterial storagedMaterial2 = FindGameMaterialInStorage(elementCombination.resource2.gameMaterialSO.materialName, data.storage);
      storagedMaterial2.quantity -= elementCombination.resource2.quantity;
   }

   void SetBlender(GameMaterialTypesEnum gameMaterialTypesEnum)
   {
      data.storageBool = true;
      Blender blender = gameObject.AddComponent<Blender>();
      buildingType = blender;
      GameObject craftPanel = buildingsUtilsPrefabManager.GetPrefab("BlenderPanel");
      blender.SetFoundryPanel(craftPanel.GetComponent<BlenderPanel>());
      blender.buildCraftingType = gameMaterialTypesEnum;
   }

}

public enum BuildingsEnum
{
   MainBase,
   Extractor,
   Storage,
   Refinery,
   Foundry,
}
