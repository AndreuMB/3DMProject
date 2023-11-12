using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Player : MonoBehaviour
{
    GameObject optionsMenu;
    GameObject mainRadialMenu;
    HUD HUDR;
    [Header("Dron Settings")]
    public int drons;
    public int dronStorage;
    public float dronSpeed;
    public GameObject selectedGO;
    public UnityEvent<GameObject> selectedGOev;

    void Start(){
        optionsMenu = FindAnyObjectByType<Options>().gameObject;
        optionsMenu.SetActive(false);

        mainRadialMenu = FindAnyObjectByType<RadialMenu>().gameObject;
        mainRadialMenu.SetActive(false);

        HUDR = FindObjectOfType<HUD>();
    }

    void Update(){
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            optionsMenu.SetActive(!optionsMenu.activeInHierarchy);
        }

        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            mainRadialMenu.SetActive(!mainRadialMenu.activeInHierarchy);
            RectTransform canvasRect = FindObjectOfType<Canvas>().GetComponent<RectTransform>();

            Vector3 canvasRectHalf = new Vector3(canvasRect.rect.width / 2, canvasRect.rect.height / 2);
            mainRadialMenu.GetComponent<RectTransform>().anchoredPosition = Input.mousePosition - canvasRectHalf;
        }

        if(HUDR.DMMenu.activeInHierarchy) return;

        if(Input.GetMouseButtonDown(0)) // left click
        {
            // selectedGO = GetClickedGO();
            SetClickedGO();
        }

    }

    public GameObject GetClickedGO(){
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            return hit.transform.gameObject;
        }
        return null;
    }

    public void SetDrons(int dronsNew){
        drons = dronsNew;
        HUDR.UpdateDronsHUD();
    }

    void SetClickedGO(){
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            selectedGO = hit.transform.gameObject;
            print("selectedGO = " + selectedGO.name);
        }
        if(selectedGO != null) selectedGOev.Invoke(selectedGO);
    }

}

public enum ResourcesEnum
{
    R1,
    R2,
    R3,
    R4,
    R5,
}
