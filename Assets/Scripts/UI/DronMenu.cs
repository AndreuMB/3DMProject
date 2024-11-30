using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class DronMenu : MonoBehaviour
{
    [SerializeField] GameObject contentGO;
    [SerializeField] GameObject addBtn;
    [SerializeField] GameObject rowPrefab;
    Player player;
    MainBase mainBase;
    GameObject selectedGODron;
    BuildingData dataSelectedGO;
    [SerializeField] GameObject dronPrefab;

    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<Player>();
        mainBase = FindObjectOfType<MainBase>();
        addBtn.GetComponent<Button>().interactable = false;
        ToggleMenu();
    }

    void OnEnable()
    {
        if (!player || !player.GetActiveGO()) return;
        foreach (Transform child in contentGO.transform)
        {
            Destroy(child.gameObject);
        }

        dataSelectedGO = player.GetActiveGO().GetComponent<Building>().data;
        selectedGODron = null;

        List<DronData> listDrons = player.GetActiveGO().GetComponent<Building>().data.setDrons;
        foreach (DronData dron in listDrons)
        {
            // if (dron.dronRef.origin == player.GetActiveGO()) AddRow(dron.dronRef);
            AddRow(dron.dronRef);
        }

        player.SetMouseSelectorStatus(false);
        AddRowBtnStatus();
    }

    void OnDisable()
    {
        if (player == null) return;
        player.placementSystem.buildingState.EndState(true);
        player.SetMouseSelectorStatus(true);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // left click
        {
            if (player.GetClickedGO() && player.GetClickedGO().CompareTag("Building"))
            {
                // if (!BasicDronCheck()) return;
                selectedGODron = player.GetClickedGO();
                AddRowBtnStatus();
            }
        }

    }

    void AddRowBtnStatus()
    {
        addBtn.GetComponent<Button>().interactable = false;
        if (!BasicDronCheck())
        {
            player.placementSystem.buildingState.EndState(true);
            return;
        }
        // show placeholder if the building is capable of storage
        player.placementSystem.SelectCell(true);
        if (player.GetActiveGO().GetComponent<Building>().data.storage.Count <= 0) return;
        // enable button if u can send materials to the building
        addBtn.GetComponent<Button>().interactable = true;
    }

    public void AddDron()
    {
        if (mainBase.drons <= 0) return;
        mainBase.SetDrons(mainBase.drons - 1);

        GameMaterial dronR = new GameMaterial(dataSelectedGO.storage[0].gameMaterialSO, mainBase.dronStorage);
        // GameMaterial dronR = new GameMaterial(MaterialManager.GetGameMaterialSO(GameMaterialsEnum.iron), mainBase.dronStorage);

        GameObject dronGO = Instantiate(dronPrefab);
        Dron dron = dronGO.GetComponent<Dron>();
        dron.SetData(player.GetActiveGO(), selectedGODron, dronR, mainBase.dronSpeed);
        // dron.CreateData();
        dron.transform.position = dron.origin.transform.position;
        dron.whilePlacingOnGoing = selectedGODron.GetComponent<Building>().placingOnGoing;

        player.GetActiveGO().GetComponent<Building>().StartDron(dron);

        AddRow(dron);

        List<DronData> listDrons = player.GetActiveGO().GetComponent<Building>().data.setDrons;
        listDrons.Add(dron.dronData);
    }

    void AddRow(Dron dron)
    {
        GameObject rowGO = Instantiate(rowPrefab);
        rowGO.transform.SetParent(contentGO.transform, false);
        TMP_Dropdown dropdown = rowGO.GetComponentsInChildren<TMP_Dropdown>()[0];
        dron.row = rowGO;
        if (dron.delete) dron.row.GetComponentInChildren<Button>().interactable = false;

        dropdown.ClearOptions();

        foreach (GameMaterial resource in dataSelectedGO.storage)
        {
            dropdown.options.Add(new TMP_Dropdown.OptionData(resource.gameMaterialSO.materialName.ToString()));
        }

        int selected = dropdown.options.FindIndex(o =>
        o.text == dron.GetNextMaterialName()
        );
        dropdown.value = selected;

        dropdown.onValueChanged.AddListener((int selected) => ChangeResource(dron, dropdown));

        rowGO.GetComponentsInChildren<TMP_Text>()[1].text = dron.destination.name;
        rowGO.GetComponentInChildren<Button>().onClick.AddListener(() => RemoveDron(dron));
    }

    void RemoveDron(Dron dron)
    {
        dron.row.GetComponentInChildren<Button>().interactable = false;
        dron.CheckDeleteDron();
    }

    void ChangeResource(Dron dron, TMP_Dropdown rowDropdown)
    {
        // string from dropdown to enum
        GameMaterialsEnum parsed_enum = (GameMaterialsEnum)Enum.Parse(typeof(GameMaterialsEnum), rowDropdown.options[rowDropdown.value].text);
        // enum to gamematerialSO
        GameMaterial newMaterial = new GameMaterial(MaterialManager.GetGameMaterialSO(parsed_enum), mainBase.dronStorage);
        dron.ChangeMaterial(newMaterial);
    }

    public void ToggleMenu()
    {
        gameObject.SetActive(!gameObject.activeSelf);
    }

    bool BasicDronCheck()
    {
        if (!selectedGODron) return false;
        if (selectedGODron == player.GetActiveGO()) return false;
        if (!selectedGODron.GetComponent<Building>().data.storageBool) return false;
        return true;
    }


}
