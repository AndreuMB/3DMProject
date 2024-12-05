using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DataSystem : MonoBehaviour
{
    public static bool newgame = true;
    public static string savefileName;
    public static FileInfo SaveToJson(MainBase mainBase, List<BuildingData> buildings, List<OreData> resources, string savefileName)
    {
        // PlayerData data = new PlayerData(player);
        GameData data = new GameData(mainBase, buildings, Camera.main.transform.parent, resources);

        string json = JsonUtility.ToJson(data, true);
        string filePath = Application.persistentDataPath + "/save/";

        bool exists = Directory.Exists(filePath);
        if (!exists) Directory.CreateDirectory(filePath);

        string file = filePath + savefileName;
        File.WriteAllText(file, json);
        FileInfo fileInfo = new(filePath);
        // Return the FileInfo object
        return fileInfo;
    }

    public static GameData LoadFromJson(string savefileName)
    {
        if (!File.Exists(Application.persistentDataPath + "/save/" + savefileName)) return null;
        string json = File.ReadAllText(Application.persistentDataPath + "/save/" + savefileName);
        GameData data = JsonUtility.FromJson<GameData>(json);
        return data;
    }

    public static void LoadGame(string savefile)
    {
        newgame = false;
        savefileName = savefile;
        // SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        SceneManager.LoadScene(1);
    }
}
