using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Options : MonoBehaviour
{
    
    void Awake(){
        if (!DataSystem.newgame) LoadGame();
    }

    public void SaveGame(){
        Player player = FindObjectOfType<Player>();
        DataSystem.SaveToJson(player);
    }

    public void LoadGame(){
        Player player = FindObjectOfType<Player>();
        PlayerData data = DataSystem.LoadFromJson();
        player.drons = data.drons;
        player.resource1 = data.resource1;
        player.resource2 = data.resource2;
        player.resource3 = data.resource3;
    }

    public void ExitGame(){
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }
}
