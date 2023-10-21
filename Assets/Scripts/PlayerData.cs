using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;

[System.Serializable]
public class PlayerData
{
    public int drons;
    // public List<Resource> resources;

    public PlayerData(Player player){
        drons = player.drons;
        // resources = player.resources;
    }
}
