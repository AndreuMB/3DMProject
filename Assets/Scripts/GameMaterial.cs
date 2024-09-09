using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameMaterial
{
   public GameMaterialSO gameMaterialSO;
   public int quantity;
   [System.NonSerialized]
   public GameObject HUDGO;

   public GameMaterial(GameMaterialSO gameMaterialSO, int quantitySet)
   {
      this.gameMaterialSO = gameMaterialSO;
      quantity = quantitySet;
   }
}
