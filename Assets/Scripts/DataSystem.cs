using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class DataSystem : MonoBehaviour
{
    public static bool newgame;
    public static void SaveToJson(Player player){
        PlayerData data = new PlayerData(player);
        
        print("Application.dataPath = " + Application.dataPath);
        string json = JsonUtility.ToJson(data,true);
        File.WriteAllText(Application.dataPath + "/PlayerDataFile.json", json);
    }

    public static PlayerData LoadFromJson(){
        string json = File.ReadAllText(Application.dataPath + "/PlayerDataFile.json");
        PlayerData data = JsonUtility.FromJson<PlayerData>(json);
        return data;
    }
}
