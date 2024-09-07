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
    BuildingData dataSGO;
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
        if (!player || !player.selectedGO) return;
        foreach (Transform child in contentGO.transform)
        {
            Destroy(child.gameObject);
        }

        dataSGO = player.selectedGO.GetComponent<Building>().data;
        selectedGODron = null;

        List<DronData> listDrons = player.selectedGO.GetComponent<Building>().data.setDrons;
        foreach (DronData dron in listDrons)
        {
            if (dron.dronRef.origin == player.selectedGO) AddRow(dron.dronRef);
        }

        player.SetMouseSelectorStatus(false);
        AddRowBtnStatus();
    }

    void OnDisable()
    {
        player.SetMouseSelectorStatus(true);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // left click
        {
            if (player.GetClickedGO() && player.GetClickedGO().CompareTag("Building"))
            {
                selectedGODron = player.GetClickedGO();
                AddRowBtnStatus();
            }
        }


    }

    void AddRowBtnStatus()
    {
        addBtn.GetComponent<Button>().interactable = false;
        if (player.selectedGO.GetComponent<Building>().data.storage.Count <= 0) return;
        if (!selectedGODron) return;
        if (selectedGODron == player.selectedGO) return;
        if (selectedGODron.GetComponent<Building>().data.buildingType == BuildingsEnum.Storage)
        {
            addBtn.GetComponent<Button>().interactable = true;
        }
    }

    public void AddDron()
    {
        if (mainBase.drons <= 0) return;
        mainBase.SetDrons(mainBase.drons - 1);

        Resource dronR = new Resource(dataSGO.storage[0].resourceEnum, mainBase.dronStorage);

        GameObject dronGO = Instantiate(dronPrefab);
        Dron dron = dronGO.GetComponent<Dron>();
        dron.SetData(player.selectedGO, selectedGODron, dronR, selectedGODron.transform.position);
        dron.CreateData();
        dron.transform.position = dron.originV;
        player.selectedGO.GetComponent<Building>().StartDronV2(dron);

        AddRow(dron);

        List<DronData> listDrons = player.selectedGO.GetComponent<Building>().data.setDrons;
        listDrons.Add(dron.dronData);
    }

    void AddRow(Dron dron)
    {
        GameObject rowGO = Instantiate(rowPrefab);
        rowGO.transform.SetParent(contentGO.transform, false);
        TMP_Dropdown dropdown = rowGO.GetComponentsInChildren<TMP_Dropdown>()[0];
        dron.row = rowGO;
        if (dron.detele) dron.row.GetComponentInChildren<Button>().interactable = false;

        dropdown.ClearOptions();

        foreach (Resource resource in dataSGO.storage)
        {
            dropdown.options.Add(new TMP_Dropdown.OptionData(resource.resourceEnum.ToString()));
        }

        int selected = dropdown.options.FindIndex(o => o.text == dron.resource.resourceEnum.ToString());
        dropdown.value = selected;

        dropdown.onValueChanged.AddListener((int selected) => ChangeResource(dron, dropdown));

        rowGO.GetComponentsInChildren<TMP_Text>()[1].text = dron.destination.name;
        rowGO.GetComponentInChildren<Button>().onClick.AddListener(() => RemoveDron(dron));
    }

    void RemoveDron(Dron dron)
    {
        dron.row.GetComponentInChildren<Button>().interactable = false;
        dron.detele = true;
    }

    void ChangeResource(Dron dron, TMP_Dropdown rowDropdown)
    {
        ResourcesEnum parsed_enum = (ResourcesEnum)Enum.Parse(typeof(ResourcesEnum), rowDropdown.options[rowDropdown.value].text);
        dron.newResource = new Resource(parsed_enum.ToString().FirstCharacterToUpper(), 0);
    }

    public void ToggleMenu()
    {
        gameObject.SetActive(!gameObject.activeSelf);
    }


}
