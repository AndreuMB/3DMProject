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
   public Material[] materials;

   // Start is called before the first frame update
   void Start()
   {
      player = FindObjectOfType<Player>();
      mainBase = FindObjectOfType<MainBase>();
      hud = FindObjectOfType<HUD>();
      buildingsUtilsPrefabManager = FindObjectOfType<BuildingsUtilsPrefabManager>();
      Format();
      if (hud && !FindObjectOfType<DronMenu>())
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

   public GameMaterial FindGameMaterialInStorage(GameMaterialsEnum gameMaterialsEnum, List<GameMaterial> storage)
   {
      storage ??= data.storage;
      return storage.Find(x => x.gameMaterialSO.materialName == gameMaterialsEnum);
   }

   public void StartDron(Dron dron)
   {
      // Coroutine coroutineInstance = StartCoroutine(StartDronCoroutine(dron));
      // dron.dronGoal.AddListener(() => StartCoroutine(StartDronCoroutineV2(dron)));

      if (!mainBase) mainBase = FindObjectOfType<MainBase>();
      float distance = dron.GetDistance();
      dron.speed = mainBase.dronSpeed;

      float time = distance / dron.speed;

      // dron.dronGoal.AddListener(() => StartCoroutine(DronTransport(time, dron)));
      dron.duration = time;
      StartCoroutine(dron.DronTransport());
   }

   public void AddResource(GameMaterialSO gameMaterialSO, int quantity)
   {
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

      CheckCompleteBuilding();

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

   bool CheckBuildingInProcess()
   {
      bool buildingInProcess = false;
      foreach (GameMaterial gameMaterial in data.storage)
      {
         if (gameMaterial.quantity < 0) buildingInProcess = true;
      }
      return buildingInProcess;
   }

   void CheckCompleteBuilding()
   {
      if (placingOnGoing)
      {
         placingOnGoing = CheckBuildingInProcess();
         if (!placingOnGoing) CompleteBuilding();
      }
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
