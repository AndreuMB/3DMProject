using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Player : MonoBehaviour
{
    GameObject optionsMenu;
    // RadialMenu mainRadialMenu;
    RMManager rMManager;
    public GameObject rmGO;
    HUD HUDR;
    [Header("Dron Settings")]
    public int drons;
    public int dronStorage;
    public float dronSpeed;
    public GameObject selectedGO;
    public UnityEvent<GameObject> selectedGOev = new();
    public Canvas canvasCPS;
    PlacementSystem ps;

    void Awake() {
        ps = FindObjectOfType<PlacementSystem>();
        rMManager = FindAnyObjectByType<RMManager>();
        HUDR = FindAnyObjectByType<HUD>();
        optionsMenu = FindAnyObjectByType<Options>().gameObject;
        canvasCPS = GameObject.Find("CanvasCPS").GetComponent<Canvas>();
    }

    void Start(){

        // mainRadialMenu = FindAnyObjectByType<RadialMenu>();
        // mainRadialMenu.gameObject.SetActive(false);

        
        optionsMenu.SetActive(false);
    }

    void Update(){
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            optionsMenu.SetActive(!optionsMenu.activeInHierarchy);
        }

        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            // if (mainRadialMenu.parent)
            // {
            //     Destroy(mainRadialMenu.parent.gameObject);
            //     return;
            // }
            if (rmGO) {
                // if (rmGO.GetComponent<RadialMenu>().parent) 
                    // Destroy(rmGO.GetComponent<RadialMenu>().parent.gameObject);
                Destroy(rmGO);
                return;
            }
            Vector3Int selectedCell = ps.GetCell();
            bool content = ps.floorData.VoidCell(selectedCell);
            RadialMenuSO RMSO;
            if(content){
                RMSO = rMManager.RMSOs.Find(x => x.name == "VoidCell");
            }else{
                RMSO = rMManager.RMSOs.Find(x => x.name == "BuildCell");
            }

            // GameObject mainRadialMenuGO = mainRadialMenu.gameObject;

            // mainRadialMenuGO.SetActive(!mainRadialMenuGO.activeInHierarchy);
            RectTransform canvasRect = canvasCPS.GetComponent<RectTransform>();

            Vector3 canvasRectHalf = new Vector3(canvasRect.rect.width / 2, canvasRect.rect.height / 2);
            // mainRadialMenu.GetComponent<RectTransform>().anchoredPosition = Input.mousePosition - canvasRectHalf;
            rmGO = rMManager.NewRM(RMSO, Input.mousePosition - canvasRectHalf);

            // mainRadialMenu.GetComponent<RectTransform>().anchoredPosition = Input.mousePosition;
        }

        if(HUDR.DMMenu.activeInHierarchy) return;

        if(Input.GetMouseButtonDown(0) && !rmGO) // left click
        {
            // selectedGO = GetClickedGO();
            SetClickedGO();
            ps.SelectCell();
        }

    }

    public GameObject GetClickedGO(){
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (!hit.transform.gameObject.CompareTag("Building")) return null;
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
            if (!hit.transform.gameObject.CompareTag("Building")) return;
            selectedGO = hit.transform.gameObject;
        }
        if(selectedGO != null) selectedGOev.Invoke(selectedGO);
    }

    public void SetActiveGO(GameObject sGO){
        selectedGO = sGO;
        selectedGOev.Invoke(selectedGO);
    }

    public bool OptionsStatus() {
        return optionsMenu.activeInHierarchy;
    }

}