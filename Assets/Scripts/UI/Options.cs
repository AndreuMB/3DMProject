using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Burst.Intrinsics;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Options : MonoBehaviour
{
    [SerializeField] GameObject buildingPrefab;
    public List<OreData> resourceDataList;
    PlacementSystem ps;
    HUD hud;
    ResourceManager rm;
    
    void Start(){
        ps = FindObjectOfType<PlacementSystem>();
        hud = FindObjectOfType<HUD>();
        rm = FindObjectOfType<ResourceManager>();

        if (!DataSystem.newgame) {
            LoadData();
        } else {
            MainBaseSpawn();
            ResourceCellsSpawn();
            // hud.IniHUD();
        }

        hud.IniHUD();


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
        DataSystem.SaveToJson(player, buildings, resourceDataList);
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
        
        GameData data = DataSystem.LoadFromJson();
        if(data == null) return;

        

        // map grid placement
        PlacementSystem ps = FindObjectOfType<PlacementSystem>();

        foreach (BuildingData building in data.buildings)
        {
            GameObject buildingGO = ps.LoadBuildings(building.parentPosition, building.buildingType);
            if (buildingGO) buildingGO.GetComponent<Building>().data = building;
            if (building.buildingType == BuildingsEnum.MainBase) buildingGO.AddComponent<Player>();
        }

        Player player = FindObjectOfType<Player>();
        if (!player) return;

        // dron upgrades
        player.drons = data.drons;
        player.dronStorage = data.dronStorage;
        player.dronSpeed = data.dronSpeed;

        // set camera position
        if (data.cameraPosition != Vector3.zero) {
            Camera.main.transform.parent.position = data.cameraPosition;
            Camera.main.transform.parent.rotation = data.cameraRotation;
            Camera.main.transform.localPosition = data.zoom;
        }

        // set drons in buildings
        DronMenu dm = hud.DMMenu.GetComponent<DronMenu>();
        foreach (GameObject buildingGO in GameObject.FindGameObjectsWithTag("Building"))
        {
            Building building = buildingGO.GetComponent<Building>();
            if (building) dm.LoadDrons(building.data.setDrons,buildingGO);
        }

        // generate and set materials in map
        foreach (OreData oreData in data.ores)
        {
            rm.GenerateOre(oreData);
        }

        // hud.UpdateDronsHUD();
        // player.resources = data.resources;
    }
    
    void CleanMap(){
        GameObject[] buildings = GameObject.FindGameObjectsWithTag("Building");
        foreach (var building in buildings)
        {
            // if (building.GetComponent<Building>().data.buildingType != BuildingsEnum.MainBase) Destroy(building);
            Destroy(building);
        }
    }

    void MainBaseSpawn(Vector3? position = null) {
        GameObject mainBase;
        mainBase = position == null ? ps.LoadBuildings(RandomCell(), BuildingsEnum.MainBase)
            : ps.LoadBuildings(position.Value, BuildingsEnum.MainBase);
        mainBase.AddComponent<Player>();
        FindObjectOfType<CameraController>().FocusBuilding(mainBase.transform.position);
    }

    void ResourceCellsSpawn() {
        for (int i = 0; i < 20; i++)
        {
            (GameObject resourceGO, OreData resourceData) = rm.GetRandomResourceGO();
            resourceData.position = RandomCell();
            resourceGO.transform.position = resourceData.position;
            resourceDataList.Add(resourceData);
        }
    }

    Vector3 RandomCell() {
        
        Ray ray = new Ray(new(Random.Range(-50, 50)+0.5f,50,Random.Range(-50, 50)+0.5f), Vector3.down);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit,100, FindObjectOfType<InputManager>().GetPlacementLayer()))
        {
            return hit.point;
        }
        return new(0,0,0);
    }
}
