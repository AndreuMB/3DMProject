using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class Player : MonoBehaviour
{
    GameObject optionsMenu;
    RMManager rMManager;
    public GameObject rmGO;
    GameObject selectedGO;
    public UnityEvent<GameObject> selectedGOev = new();
    public Canvas canvasCPS;
    public PlacementSystem placementSystem;
    CameraController cameraController;
    bool mouseSelector = true;
    float lastClickTime = 0f;
    readonly float doubleClickThreshold = 0.3f; // Time in seconds within which two clicks are considered a double-click

    void Awake()
    {
        placementSystem = FindObjectOfType<PlacementSystem>();
        rMManager = FindAnyObjectByType<RMManager>();
        optionsMenu = FindAnyObjectByType<Options>().gameObject;
        canvasCPS = GameObject.Find("CanvasCPS").GetComponent<Canvas>();
        cameraController = FindAnyObjectByType<CameraController>();
    }

    void Start()
    {
        optionsMenu.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            GameObject menuGO = GameObject.FindWithTag("Menu");
            if (menuGO != null)
            {
                menuGO.SetActive(false);
                return;
            }
            optionsMenu.SetActive(!optionsMenu.activeInHierarchy);
        }
        if (optionsMenu.activeSelf) return;

        // right click
        if (Input.GetKeyUp(KeyCode.Mouse1))
        {
            if (rmGO)
            {
                Destroy(rmGO);
                return;
            }
            Vector3Int selectedCell = placementSystem.GetCell();
            bool voidCell = placementSystem.floorData.IsCellVoid(selectedCell);
            RadialMenuSO RMSO = null;
            if (placementSystem.buildingState.CheckObstacle(placementSystem.gridPositionFloat)) return;
            if (voidCell)
            {
                RMSO = rMManager.RMSOs.Find(x => x.name == "VoidCell");
            }
            else
            {
                switch (GetClickedGO().tag)
                {
                    case "Building":
                        RMSO = rMManager.RMSOs.Find(x => x.name == "BuildCell");
                        break;
                    case "Ore":
                        RMSO = rMManager.RMSOs.Find(x => x.name == "OreCell");
                        break;
                }
            }
            if (RMSO == null) return;

            RectTransform canvasRect = canvasCPS.GetComponent<RectTransform>();

            Vector3 canvasRectHalf = new Vector3(canvasRect.rect.width / 2, canvasRect.rect.height / 2);
            try
            {
                rmGO = rMManager.NewRM(RMSO, Input.mousePosition - canvasRectHalf);
            }
            catch (Exception)
            {
                throw new Exception("RMSO not found");
            }

        }

        // if mouse selector false not left click interaction
        if (!mouseSelector) return;
        if (Input.GetMouseButtonUp(0) && !cameraController.GetHasCameraMove()) // left click
        {
            // if button click not select cell
            if (EventSystem.current.IsPointerOverGameObject()) return;
            // if radial menu open not select cell
            if (rmGO) return;

            // Check for double-click
            if (Time.time - lastClickTime < doubleClickThreshold)
            {
                // Double-click detected
                if (selectedGO) cameraController.FocusBuilding(selectedGO.transform.position, true);
            }
            else
            {
                // Single-click detected, reset lastClickTime
                lastClickTime = Time.time;
                SetClickedGOFromMouse(Input.mousePosition);
                placementSystem.SelectCell();
            }
        }


    }

    public GameObject GetClickedGO()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            return hit.transform.gameObject;
        }
        return null;
    }

    public void SetClickedGOFromMouse(Vector3 mousePosition)
    {
        // Vector3 selectedPositionFromCameraPerspective = Camera.main.ScreenToWorldPoint(mousePosition);
        Ray ray = Camera.main.ScreenPointToRay(mousePosition);
        // Vector3 mousePositionCenter = placementSystem.GetCenterPositionCell(selectedPositionFromCameraPerspective);
        // Ray ray = new(mousePositionCenter, Vector3.down);

        if (Physics.Raycast(ray, out RaycastHit hit, 100, FindObjectOfType<InputManager>().GetPlacementLayer()))
        {
            Vector3 mousePositionCenter = placementSystem.GetCenterPositionCell(hit.point);
            SetClickedGOFromVector3(mousePositionCenter);
        }
        if (selectedGO != null) selectedGOev.Invoke(selectedGO);
    }

    public void SetActiveGO(GameObject sGO)
    {
        selectedGO = sGO;
        selectedGOev.Invoke(selectedGO);
    }

    public GameObject GetActiveGO()
    {
        return selectedGO;
    }

    public bool OptionsStatus()
    {
        return optionsMenu.activeInHierarchy;
    }

    public void SetMouseSelectorStatus(bool status)
    {
        mouseSelector = status;
    }

    public bool GetMouseSelectorStatus()
    {
        return mouseSelector;
    }

    public void SetClickedGOFromVector3(Vector3 position, bool selectCell = false)
    {
        Vector3 mousePositionCenter = placementSystem.GetCenterPositionCell(position);
        Ray ray = new(mousePositionCenter, Vector3.down);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (!hit.transform.gameObject.CompareTag("Building")) return;
            selectedGO = hit.transform.gameObject;
        }
        if (selectedGO != null)
        {
            selectedGOev.Invoke(selectedGO);
            if (selectCell) placementSystem.SelectCellFromVector3(position);
        }
    }

}