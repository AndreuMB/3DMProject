using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Options : MonoBehaviour
{
    
    void Awake(){
        if (!DataSystem.newgame) LoadData();
    }

    public void SaveGame(){
        Player player = FindObjectOfType<Player>();
        DataSystem.SaveToJson(player);
    }

    public void LoadGame(){
        DataSystem.newgame = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ExitGame(){
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }

    public void LoadData(){
        Player player = FindObjectOfType<Player>();
        if (!player) return;
        PlayerData data = DataSystem.LoadFromJson();
        player.drons = data.drons;
        player.resources = data.resources;
    }
}
