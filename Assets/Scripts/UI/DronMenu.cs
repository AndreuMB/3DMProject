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
    GameObject selectedGODron;
    BuildingData dataSGO;
    [SerializeField] GameObject dronPrefab;

    // Start is called before the first frame update
    void Awake()
    {
        player = FindObjectOfType<Player>();
        addBtn.GetComponent<Button>().interactable = false;
    }

    void OnEnable(){
        if (!player.selectedGO) return;
        foreach (Transform child in contentGO.transform) {
            Destroy(child.gameObject);
        }

        List<Dron> listDrons = player.selectedGO.GetComponent<Building>().data.setDrons;
        foreach (Dron dron in listDrons)
        {
            if(dron.origin == player.selectedGO) AddRow(dron);
        }
        dataSGO = player.selectedGO.GetComponent<Building>().data;
        selectedGODron = null;
        AddRowBtnStatus();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0)) // left click
        {
            if (player.GetClickedGO())
            {
                selectedGODron = player.GetClickedGO();
                AddRowBtnStatus();
            }
        }

        
    }

    void AddRowBtnStatus(){
        addBtn.GetComponent<Button>().interactable = false;
        if (player.selectedGO.GetComponent<Building>().data.storage.Count <= 0) return;
        if (!selectedGODron) return;
        if (selectedGODron == player.selectedGO) return;
        if(selectedGODron.GetComponent<Building>().data.buildingType == BuildingsEnum.Storage){
            addBtn.GetComponent<Button>().interactable = true;
        }
    }

    public void AddDron(){
        if(player.drons <= 0) return;
        player.SetDrons(player.drons-1);

        Resource dronR = new Resource(dataSGO.storage[0].name, player.dronStorage);

        GameObject dronGO = Instantiate(dronPrefab);
        dronGO.AddComponent<Dron>();
        Dron dron = dronGO.GetComponent<Dron>();
        dron.SetData(player.selectedGO,selectedGODron, dronR);
        dron.transform.position = dron.origin.transform.position;
        dron.coroutine = player.selectedGO.GetComponent<Building>().StartDron(dron);

        AddRow(dron);

        List<Dron> listDrons = player.selectedGO.GetComponent<Building>().data.setDrons;
        listDrons.Add(dron);
    }

    void AddRow(Dron dron){
        GameObject rowGO = Instantiate(rowPrefab);
        rowGO.transform.SetParent(contentGO.transform,false);
        TMP_Dropdown dropdown = rowGO.GetComponentsInChildren<TMP_Dropdown>()[0];
        dropdown.ClearOptions();

        foreach (Resource resource in dataSGO.storage)
        {
            dropdown.options.Add(new TMP_Dropdown.OptionData(resource.name.ToString()));
        }

        int selected = dropdown.options.FindIndex(o => o.text == dron.resource.name.ToString());
        dropdown.value = selected;

        dropdown.onValueChanged.AddListener((int selected) => ChangeResource(dron,dropdown));

        rowGO.GetComponentsInChildren<TMP_Text>()[1].text = dron.destiny.name;
        rowGO.GetComponentInChildren<Button>().onClick.AddListener(() => RemoveDron(dron, rowGO));
    }

    void RemoveDron(Dron dron, GameObject rowGO){
        player.SetDrons(player.drons+1);
        Destroy(rowGO);
        Destroy(dron.gameObject);
        player.selectedGO.GetComponent<Building>().StopDron(dron.coroutine);
        List<Dron> listDrons = player.selectedGO.GetComponent<Building>().data.setDrons;
        listDrons.Remove(dron);
    }

    void ChangeResource(Dron dron, TMP_Dropdown rowDropdown){
        ResourcesEnum parsed_enum = (ResourcesEnum)Enum.Parse( typeof(ResourcesEnum), rowDropdown.options[rowDropdown.value].text );
        dron.resource.name = parsed_enum;
        player.selectedGO.GetComponent<Building>().StopDron(dron.coroutine);
        dron.coroutine = player.selectedGO.GetComponent<Building>().StartDron(dron);
        
        // Resource dronR = new Resource(ResourcesEnum.rowDropdown.options[rowDropdown.value].text, player.dronStorage);
        // Resource r = dataSGO.storage.Find(r => r.name.ToString() == rowDropdown.options[rowDropdown.value].text);

    }

    
}
