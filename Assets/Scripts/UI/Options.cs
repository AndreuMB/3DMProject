using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Options : MonoBehaviour
{
    [SerializeField] GameObject extractor;
    
    void Awake(){
        if (!DataSystem.newgame) LoadData();
    }

    public void SaveGame(){
        Player player = FindObjectOfType<Player>();
        GameObject[] buildingsGO = GameObject.FindGameObjectsWithTag("Building");

        List<BuildingData> buildings = new();
        foreach (GameObject extractor in buildingsGO)
        {
            print(extractor.name);
            Building extractorComp = extractor.GetComponent<Building>();
            extractorComp.data.parentName = extractor.name;
            extractorComp.data.parentPosition = extractor.transform.position;
            buildings.Add(extractorComp.data);
        }
        DataSystem.SaveToJson(player, buildings);
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
        // PlayerData data = DataSystem.LoadFromJson();
        GameData data = DataSystem.LoadFromJson2();
        player.drons = data.drons;
        foreach (BuildingData building in data.buildings)
        {
            GameObject buildingGO = GameObject.Find(building.parentName);
            if (buildingGO)
            {
                buildingGO.GetComponent<Building>().data = building;
            }else{
                Instantiate(extractor);
                extractor.transform.position = building.parentPosition;
                extractor.name = building.parentName;
                extractor.GetComponent<Building>().data = building;
            }
        }
        // player.resources = data.resources;
    }
}
