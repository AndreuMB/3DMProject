using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
public class Building : MonoBehaviour
{
   public GameMaterialSO placeholderMaterialSO;
   public BuildingData data;
   Player player;
   MainBase mainBase;
   public HUD hud;
   public IBuilding buildingType;
   BuildingsUtilsPrefabManager buildingsUtilsPrefabManager;
   public bool placingOnGoing = true;
   public Material[] materials;
   public UnityEvent buildComplete = new UnityEvent();

   // Start is called before the first frame update
   void Start()
   {
      player = FindObjectOfType<Player>();
      mainBase = FindObjectOfType<MainBase>();
      hud = FindObjectOfType<HUD>();
      buildingsUtilsPrefabManager = FindObjectOfType<BuildingsUtilsPrefabManager>();
      Format();
      // if (hud && !FindObjectOfType<DronMenu>())
      // {
      //    player.SetActiveGO(gameObject);
      // }
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
            data.storageBool = true;
            MainBase mainBase = gameObject.AddComponent<MainBase>();
            mainBase.hud = hud;
            // interface type
            buildingType = mainBase;
            break;
         case BuildingsEnum.Extractor:
            data.storageBool = true;
            Extractor extractor = gameObject.AddComponent<Extractor>();
            // resourceSO = gameMaterialSO;
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
         gameMaterialsBuild.ForEach(material => AddGameMaterial(material.gameMaterialSO, -material.quantity));
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
      if (hud) hud.UpdateBuildingResourceContainerColor(this);
      // trigger event
      buildComplete.Invoke();
   }

   public GameMaterial FindGameMaterialInStorage(GameMaterialsEnum gameMaterialsEnum, List<GameMaterial> storage)
   {
      storage ??= data.storage;
      return storage.Find(x => x.gameMaterialSO.materialName == gameMaterialsEnum);
   }

   public void StartDron(Dron dron)
   {
      if (!mainBase) mainBase = FindObjectOfType<MainBase>();
      float distance = dron.GetDistance();
      dron.speed = mainBase.dronSpeed;

      float time = distance / dron.speed;

      // dron.dronGoal.AddListener(() => StartCoroutine(DronTransport(time, dron)));
      dron.duration = time;

      StartCoroutine(dron.DronTransport());
   }

   public void AddGameMaterial(GameMaterialSO gameMaterialSO, int quantity)
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

   public void RemoveGameMaterial(GameMaterialsEnum gameMaterialEnum, int quantity)
   {
      GameMaterial storagedResource = data.storage.Find(
      resource => resource.gameMaterialSO.materialName == gameMaterialEnum);
      if (storagedResource != null && storagedResource.quantity - quantity >= 0)
      {
         // when is enough material remove it
         storagedResource.quantity -= quantity;
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
