using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Foundry : MonoBehaviour, IBuilding
{
    // [SerializeField] GameObject hud;
    List<string> buttonsList = new();
    HUD hud;
    void Start()
    {
        hud = FindObjectOfType<HUD>();
        buttonsList.Add("Craft1");
        buttonsList.Add("Craft2");
        GenerateButtons();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ShowHUD(){
        print("show hud foundry");
        // hud.SetActive(true);
    }

    public void GenerateButtons(){
        foreach (string buttonName in buttonsList)
        {
            GameObject buttonGO = Instantiate(hud.buttonPrefab, hud.buildingButtpnsPanel);
            buttonGO.GetComponentInChildren<TMP_Text>().text = buttonName;
        }
    }
}
