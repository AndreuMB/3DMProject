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
    GameObject rmGO;
    HUD HUDR;
    [Header("Dron Settings")]
    public int drons;
    public int dronStorage;
    public float dronSpeed;
    public GameObject selectedGO;
    public UnityEvent<GameObject> selectedGOev;
    public Canvas canvasCPS;
    PlacementSystem ps;

    void Start(){
        optionsMenu = FindAnyObjectByType<Options>().gameObject;
        optionsMenu.SetActive(false);

        // mainRadialMenu = FindAnyObjectByType<RadialMenu>();
        // mainRadialMenu.gameObject.SetActive(false);

        HUDR = FindObjectOfType<HUD>();
        ps = FindObjectOfType<PlacementSystem>();
        rMManager = FindAnyObjectByType<RMManager>();
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
                if (rmGO.GetComponent<RadialMenu>().parent) 
                    Destroy(rmGO.GetComponent<RadialMenu>().parent.gameObject);
                Destroy(rmGO);
                return;
            }
            Vector3Int selectedCell = ps.SelectCell();
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
