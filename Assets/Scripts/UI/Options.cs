using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Burst.Intrinsics;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Options : MonoBehaviour
{
    [SerializeField] GameObject buildingPrefab;
    
    void Awake(){
        if (!DataSystem.newgame) LoadData();
    }

    public void SaveGame(){
        Player player = FindObjectOfType<Player>();
        GameObject[] buildingsGO = GameObject.FindGameObjectsWithTag("Building");

        List<BuildingData> buildings = new();
        foreach (GameObject extractor in buildingsGO)
        {
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
        if(data == null) return;
        player.drons = data.drons;
        player.dronStorage = data.dronStorage;
        player.dronSpeed = data.dronSpeed;
        foreach (BuildingData building in data.buildings)
        {
            GameObject buildingGO = GameObject.Find(building.parentName);
            if (buildingGO)
            {
                buildingGO.GetComponent<Building>().data = building;
            }else{
                GameObject newBuilding = Instantiate(buildingPrefab);
                newBuilding.transform.position = building.parentPosition;
                newBuilding.name = building.parentName;
                newBuilding.GetComponent<Building>().data = building;
            }
        }
        // player.resources = data.resources;
    }
}
