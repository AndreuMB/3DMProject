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
    void Start()
    {
        player = FindObjectOfType<Player>();
        addBtn.GetComponent<Button>().interactable = false;
    }

    void OnEnable(){
        if (!player || !player.selectedGO) return;
        foreach (Transform child in contentGO.transform) {
            Destroy(child.gameObject);
        }

        dataSGO = player.selectedGO.GetComponent<Building>().data;
        selectedGODron = null;

        List<DronData> listDrons = player.selectedGO.GetComponent<Building>().data.setDrons;
        foreach (DronData dron in listDrons)
        {
            if(dron.dronRef.origin == player.selectedGO) AddRow(dron.dronRef);
        }

        
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
        dron.SetData(player.selectedGO,selectedGODron, dronR, selectedGODron.transform.position);
        dron.CreateData();
        dron.transform.position = dron.originV;
        player.selectedGO.GetComponent<Building>().StartDronV2(dron);

        AddRow(dron);

        List<DronData> listDrons = player.selectedGO.GetComponent<Building>().data.setDrons;
        listDrons.Add(dron.dronData);
    }

    void AddRow(Dron dron){
        GameObject rowGO = Instantiate(rowPrefab);
        rowGO.transform.SetParent(contentGO.transform,false);
        TMP_Dropdown dropdown = rowGO.GetComponentsInChildren<TMP_Dropdown>()[0];
        dron.row = rowGO;
        if (dron.detele) dron.row.GetComponentInChildren<Button>().interactable = false;

        dropdown.ClearOptions();

        foreach (Resource resource in dataSGO.storage)
        {
            dropdown.options.Add(new TMP_Dropdown.OptionData(resource.name.ToString()));
        }

        int selected = dropdown.options.FindIndex(o => o.text == dron.resource.name.ToString());
        dropdown.value = selected;

        dropdown.onValueChanged.AddListener((int selected) => ChangeResource(dron,dropdown));

        rowGO.GetComponentsInChildren<TMP_Text>()[1].text = dron.destination.name;
        rowGO.GetComponentInChildren<Button>().onClick.AddListener(() => RemoveDron(dron));
    }

    void RemoveDron(Dron dron){
        dron.row.GetComponentInChildren<Button>().interactable = false;
        dron.detele = true;
    }

    void ChangeResource(Dron dron, TMP_Dropdown rowDropdown){
        ResourcesEnum parsed_enum = (ResourcesEnum)Enum.Parse( typeof(ResourcesEnum), rowDropdown.options[rowDropdown.value].text );
        dron.newResource = new Resource (parsed_enum,0);
        // player.selectedGO.GetComponent<Building>().StopDron(dron.coroutine);
        // dron.coroutine = player.selectedGO.GetComponent<Building>().StartDron(dron);
        
        // Resource dronR = new Resource(ResourcesEnum.rowDropdown.options[rowDropdown.value].text, player.dronStorage);
        // Resource r = dataSGO.storage.Find(r => r.name.ToString() == rowDropdown.options[rowDropdown.value].text);

    }

    public void LoadDrons(List<DronData> dronDataList, GameObject origin){
        foreach (DronData dronData in dronDataList)
        {
            // player.SetDrons(player.drons-1);
            GameObject dronGO = Instantiate(dronPrefab,dronData.dronPosition,Quaternion.identity);
            dronGO.AddComponent<Dron>();
            Dron dron = dronGO.GetComponent<Dron>();
            dron.dronData = dronData;
            dron.dronData.dronRef = dron;
            dron.SetData(origin,GameObject.Find(dronData.destination), dronData.resource, dronData.movingTo);
            origin.GetComponent<Building>().StartDronV2(dron);
            dron.dronGoal.Invoke();
        }
        
    }

    
}
