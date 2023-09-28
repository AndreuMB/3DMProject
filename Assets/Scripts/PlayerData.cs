using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;

[System.Serializable]
public class PlayerData
{
    public int drons;
    public int resource1;
    public int resource2;
    public int resource3;

    public PlayerData(Player player){
        drons = player.drons;
        resource1 = player.resource1;
        resource2 = player.resource2;
        resource3 = player.resource3;
    }
}
