using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class DataSystem : MonoBehaviour
{
    public static bool newgame;
    public static void SaveToJson(Player player,List<BuildingData> buildings, List<OreData> resources){
        // PlayerData data = new PlayerData(player);
        GameData data = new GameData(player, buildings, Camera.main.transform.parent, resources);
        
        string json = JsonUtility.ToJson(data,true);
        File.WriteAllText(Application.dataPath + "/GameDataFile.json", json);
    }

    public static GameData LoadFromJson(){
        if (!File.Exists(Application.dataPath + "/GameDataFile.json")) return null;
        string json = File.ReadAllText(Application.dataPath + "/GameDataFile.json");
        GameData data = JsonUtility.FromJson<GameData>(json);
        return data;
    }
}
