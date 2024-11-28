using System.Collections;
using System.Collections.Generic;
using System.IO;
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
    MaterialManager rm;
    [SerializeField] GameObject dronPrefab;
    [SerializeField] SavesMenu savesMenu;

    void Start()
    {
        ps = FindObjectOfType<PlacementSystem>();
        hud = FindObjectOfType<HUD>();
        rm = FindObjectOfType<MaterialManager>();

        if (!DataSystem.newgame)
        {
            LoadData();
        }
        else
        {
            Building mainBase = MainBaseSpawn();
            mainBase.buildSet.AddListener(() => mainBase.GetComponent<MainBase>().SetDrons(2));
            Vector3 mainBasePosition = mainBase.transform.position;
            StorageSpawn(mainBasePosition);
            ResourceCellsSpawn();
        }

        // hud.IniHUD();


    }

    public FileInfo SaveGame(string savefileName)
    {
        MainBase mainBase = FindObjectOfType<MainBase>();
        GameObject[] buildingsGO = GameObject.FindGameObjectsWithTag("Building");

        List<BuildingData> buildings = new();
        foreach (GameObject buildingGO in buildingsGO)
        {
            Building building = buildingGO.GetComponent<Building>();
            building.data.parentName = buildingGO.name;
            building.data.parentPosition = buildingGO.transform.position;
            buildings.Add(building.data);
        }
        return DataSystem.SaveToJson(mainBase, buildings, resourceDataList, savefileName);
    }

    public void LoadGame(string savefileName)
    {
        DataSystem.newgame = false;
        DataSystem.savefileName = savefileName;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ExitGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }

    public void LoadData()
    {
        CleanMap();

        GameData data = DataSystem.LoadFromJson(DataSystem.savefileName);
        if (data == null) return;



        // map grid placement
        foreach (BuildingData building in data.buildings)
        {
            GameObject buildingGO = ps.LoadBuildings(building.parentPosition, building.buildingType);
            if (!buildingGO) continue;
            buildingGO.GetComponent<Building>().data = building;
            if (building.buildingType == BuildingsEnum.MainBase)
            {
                buildingGO.AddComponent<Player>();
                buildingGO.GetComponent<Building>().buildSet.AddListener(() => SetDronsUpgrades(buildingGO.GetComponent<MainBase>(), data));
            }
            buildingGO.name = building.parentName;
        }

        // set camera position
        if (data.cameraPosition != Vector3.zero)
        {
            Camera.main.transform.parent.position = data.cameraPosition;
            Camera.main.transform.parent.rotation = data.cameraRotation;
            Camera.main.transform.localPosition = data.zoom;
        }

        // set drons in buildings
        foreach (GameObject buildingGO in GameObject.FindGameObjectsWithTag("Building"))
        {
            Building building = buildingGO.GetComponent<Building>();
            if (building != null) LoadDrons(building.data.setDrons, buildingGO, data);
        }

        // restore ore manage var
        resourceDataList = data.ores;
        // generate and set materials in map
        foreach (OreData oreData in data.ores)
        {
            GameObject oreGO = rm.GenerateOre(oreData);
            ps.PlaceOre(oreGO, oreData.position);
        }
    }

    void CleanMap()
    {
        GameObject[] buildings = GameObject.FindGameObjectsWithTag("Building");
        foreach (var building in buildings)
        {
            // if (building.GetComponent<Building>().data.buildingType != BuildingsEnum.MainBase) Destroy(building);
            Destroy(building);
        }
    }

    Building MainBaseSpawn()
    {
        GameObject mainBase;
        mainBase = ps.LoadBuildings(PositionOnTerrain(), BuildingsEnum.MainBase);
        mainBase.AddComponent<Player>();
        FindObjectOfType<CameraController>().FocusBuilding(mainBase.transform.position);
        return mainBase.GetComponent<Building>();
    }

    void StorageSpawn(Vector3 mainStoragePosition)
    {
        GameObject storage;
        int randomX = Random.Range(0, 2) == 0 ? Random.Range(-4, 0) : Random.Range(1, 4);
        int randomZ = Random.Range(0, 2) == 0 ? Random.Range(-4, 0) : Random.Range(1, 4);
        mainStoragePosition.x += randomX;
        mainStoragePosition.z += randomZ;
        mainStoragePosition = PositionOnTerrain(mainStoragePosition);
        storage = ps.LoadBuildings(mainStoragePosition, BuildingsEnum.Storage);
        storage.GetComponent<Building>().AddGameMaterial(MaterialManager.GetGameMaterialSO(GameMaterialsEnum.iron), 10);
        storage.GetComponent<Building>().AddGameMaterial(MaterialManager.GetGameMaterialSO(GameMaterialsEnum.copper), 5);
    }

    void ResourceCellsSpawn()
    {
        for (int i = 0; i < 20; i++)
        {
            (GameObject oreGO, OreData oreData) = rm.GetRandomResourceGO();
            oreData.position = PositionOnTerrain();
            ps.PlaceOre(oreGO, oreData.position);
            // resourceGO.transform.position = resourceData.position;
            resourceDataList.Add(oreData);
        }
    }

    Vector3 PositionOnTerrain(Vector3? position = null)
    {
        Ray ray;
        if (position == null)
        {
            ray = new Ray(new(Random.Range(-50, 50) + 0.5f, 50, Random.Range(-50, 50) + 0.5f), Vector3.down);
        }
        else
        {
            ray = new Ray(new(position.Value.x, 50, position.Value.z), Vector3.down);
        }
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100, FindObjectOfType<InputManager>().GetPlacementLayer()))
        {
            return hit.point;
        }
        return new(0, 0, 0);
    }

    void OnDisable()
    {
        savesMenu.ClosePanel();
    }

    void LoadDrons(List<DronData> dronDataList, GameObject origin, GameData gameData)
    {
        foreach (DronData dronData in dronDataList)
        {
            GameObject dronGO = Instantiate(dronPrefab, dronData.dronPosition, Quaternion.identity);
            dronGO.AddComponent<Dron>();
            Dron dron = dronGO.GetComponent<Dron>();
            dron.dronData = dronData;
            dron.dronData.dronRef = dron;
            dron.SetData(origin, GameObject.Find(dronData.destination), dronData.material, gameData.dronSpeed);
            origin.GetComponent<Building>().StartDron(dron);
            dron.dronGoal.Invoke();
        }

    }

    void SetDronsUpgrades(MainBase mainBase, GameData data)
    {
        // dron upgrades
        mainBase.drons = data.drons;
        mainBase.dronStorage = data.dronStorage;
        mainBase.dronSpeed = data.dronSpeed;
    }
}
