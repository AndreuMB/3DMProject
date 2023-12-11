using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Burst.Intrinsics;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Options : MonoBehaviour
{
    [SerializeField] GameObject buildingPrefab;
    
    void Start(){
        if (!DataSystem.newgame) LoadData();
        gameObject.SetActive(false);
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
        CleanMap();
        Player player = FindObjectOfType<Player>();
        if (!player) return;
        // PlayerData data = DataSystem.LoadFromJson();
        GameData data = DataSystem.LoadFromJson2();
        if(data == null) return;

        // dron upgrades
        player.drons = data.drons;
        player.dronStorage = data.dronStorage;
        player.dronSpeed = data.dronSpeed;

        // map grid placement
        PlacementSystem ps = FindObjectOfType<PlacementSystem>();
        HUD hud = FindObjectOfType<HUD>();
        DronMenu dm = hud.DMMenu.GetComponent<DronMenu>();

        foreach (BuildingData building in data.buildings)
        {
            if (building.buildingType == BuildingsEnum.MainBase) continue;
            // if (buildingGO)
            // {
            //     buildingGO.GetComponent<Building>().data = building;
            // }else{
            // GameObject newBuilding = Instantiate(buildingPrefab);
            // newBuilding.transform.position = building.parentPosition;
            // newBuilding.name = building.parentName;
            GameObject buildingGO = ps.LoadBuildings(building.parentPosition, building.buildingType);
            if (buildingGO) buildingGO.GetComponent<Building>().data = building;
            // print("dm = " + dm);
            // dm.LoadDrons(building.setDrons,buildingGO);
            // }
        }

        foreach (GameObject buildingGO in GameObject.FindGameObjectsWithTag("Building"))
        {
            Building building = buildingGO.GetComponent<Building>();
            if (building) dm.LoadDrons(building.data.setDrons,buildingGO);
        }

        // hud.UpdateDronsHUD();
        // player.resources = data.resources;
    }
    
    void CleanMap(){
        GameObject[] buildings = GameObject.FindGameObjectsWithTag("Building");
        foreach (var building in buildings)
        {
            if (building.GetComponent<Building>().data.buildingType != BuildingsEnum.MainBase) Destroy(building);
        }
    }
}
