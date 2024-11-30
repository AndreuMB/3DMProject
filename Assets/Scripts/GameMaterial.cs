using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameMaterial
{
   public GameMaterialSO gameMaterialSO;
   public GameMaterialsEnum gameMaterialEnum;
   public int quantity;
   [System.NonSerialized]
   public GameObject HUDGO;

   public GameMaterial(GameMaterialSO gameMaterialSO, int quantitySet)
   {
      this.gameMaterialSO = gameMaterialSO;
      if (gameMaterialSO != null) gameMaterialEnum = gameMaterialSO.materialName;
      quantity = quantitySet;
   }
}
