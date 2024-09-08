using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DataSystem : MonoBehaviour
{
    public static bool newgame = true;
    public static string savefileName;
    public static FileInfo SaveToJson(Player player,List<BuildingData> buildings, List<OreData> resources, string savefileName){
        // PlayerData data = new PlayerData(player);
        GameData data = new GameData(player, buildings, Camera.main.transform.parent, resources);
        
        string json = JsonUtility.ToJson(data,true);
        // string filePath = Path.Combine(Application.dataPath, EnvReader.GetEnvVariable("SAVES_PATH"), savefileName);
        string filePath = Application.dataPath + EnvReader.GetEnvVariable("SAVES_PATH") + savefileName;
        File.WriteAllText(filePath, json);
        FileInfo fileInfo = new(filePath);
        // Return the FileInfo object
        return fileInfo;
    }

    public static GameData LoadFromJson(string savefileName){
        if (!File.Exists(Application.dataPath + EnvReader.GetEnvVariable("SAVES_PATH") + savefileName)) return null;
        string json = File.ReadAllText(Application.dataPath + EnvReader.GetEnvVariable("SAVES_PATH") + savefileName);
        GameData data = JsonUtility.FromJson<GameData>(json);
        return data;
    }

    public static void LoadGame(string savefile){
        newgame = false;
        savefileName = savefile;
        // SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        SceneManager.LoadScene(1);
    }
}
