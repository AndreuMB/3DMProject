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
            print("dron.origin = " + dron.origin);
            if(dron.origin == player.selectedGO.name) AddRow(dron);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0)) // left click
        {
            if (player.GetClickedGO())
            {
                selectedGODron = player.GetClickedGO();
            }
        }

        if (!selectedGODron)
        {
            addBtn.GetComponent<Button>().interactable = false;
            return;
        }
        
        if(selectedGODron.GetComponent<Building>().data.buildingType == BuildingsEnum.Storage){
            addBtn.GetComponent<Button>().interactable = true;
        }else{
            addBtn.GetComponent<Button>().interactable = false;
        }
    }

    public void AddDron(){
        Player player = FindObjectOfType<Player>();
        if(player.drons <= 0) return;
        player.SetDrons(player.drons-1);
        Dron dron = new Dron(player.selectedGO.name,selectedGODron.name);

        dron.coroutine = player.selectedGO.GetComponent<Building>().StartDron(dron.destiny);

        AddRow(dron);

        List<Dron> listDrons = player.selectedGO.GetComponent<Building>().data.setDrons;
        listDrons.Add(dron);

    }

    void AddRow(Dron dron){
        GameObject rowGO = Instantiate(rowPrefab);
        rowGO.transform.SetParent(contentGO.transform,false);
        rowGO.GetComponentsInChildren<TMP_Text>()[1].text = dron.destiny;
        rowGO.GetComponentInChildren<Button>().onClick.AddListener(() => RemoveDron(dron, rowGO));
    }

    void RemoveDron(Dron dron, GameObject rowGO){
        player.SetDrons(player.drons+1);
        Destroy(rowGO);
        player.selectedGO.GetComponent<Building>().StopDron(dron.coroutine);
        List<Dron> listDrons = player.selectedGO.GetComponent<Building>().data.setDrons;
        listDrons.Remove(dron);
    }

    
}
