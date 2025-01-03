using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BuildingData
{
   public BuildingsEnum buildingType;
   public int quantity;
   public float rate;
   public bool storageBool;
   public List<GameMaterial> storage;
   // float inventory;
   public float maxStorage;
   public string parentName;
   public Vector3 parentPosition;
   public List<DronData> setDrons;
   public int id;

   // public Building(ResourcesEnum nameSet, float quantitySet)
   // {
   //    name = nameSet;
   //    quantity = quantitySet;
   // }
}
