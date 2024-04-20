using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlacementSystem : MonoBehaviour
{
    [SerializeField] private InputManager inputManager;
    [SerializeField] private Grid grid;
    [SerializeField] private ObjectsDatabaseSO database;
    [SerializeField] private GameObject gridVisualization;

    public GridData floorData, buildData;
    private Renderer previewRenderer;

    [SerializeField] private ObjectPlacer objectPlacer;

    [SerializeField] private PreviewSystem preview;
    [SerializeField] private PreviewSystem preview2;
    
    private Vector3Int lastDetectedPosition = Vector3Int.zero;

    public GameState buildingtState;
    Vector3Int gridPosition;
    
    Vector3 gridPositionFloat;


    private void Start()
    {
        gridVisualization.SetActive(true);
        // StopPlacement();
        if (buildingtState == null) BuildingtStateGS();
    }
    // public void StartPlacement (int ID)
    // {
    //     StopPlacement();
    //     // gridVisualization.SetActive(true);
    //     buildingtState = new PlacementState (ID,grid,preview,database,floorData,buildData,objectPlacer);
    //     inputManager.OnCliched += PlaceStructure;
    //     inputManager.OnExit += StopPlacement;
    // }
    // public void StartRemoving()
    // {
    //     StopPlacement();
    //     // gridVisualization.SetActive (true);
    //     buildingtState = new RemovingState(grid,preview,floorData,buildData,objectPlacer);

    //     inputManager.OnCliched += PlaceStructure;
    //     inputManager.OnExit += StopPlacement;
    // }
    // private void PlaceStructure()
    // {
    //     if (inputManager.IsPointerOverUI())
    //     {
    //         Debug.Log("Vuelve :" + inputManager.IsPointerOverUI());
    //         return;
    //     }
    //     Vector3 mousePosition = inputManager.GetSelectedMapPosition();
    //     Vector3Int gridPosition = grid.WorldToCell(mousePosition);
    //     buildingtState.OnAction(gridPosition);
    // }

    /*
    private bool CheckPlacementValidity(Vector3Int gridPosition, int selectedObjectIndex)
    {
        GridData selectedData = database.objectsData[selectedObjectIndex].ID == 0 ? floorData : buildData;
        return selectedData.CanPlaceObjectAt(gridPosition, database.objectsData[selectedObjectIndex].Size);
    }
    */
    // private void StopPlacement()
    // {
    //     if (buildingtState == null) { return; }
    //     // gridVisualization.SetActive(false);
    //     buildingtState.EndState();
    //     inputManager.OnCliched -= PlaceStructure;
    //     inputManager.OnExit -= StopPlacement;
    //     lastDetectedPosition = Vector3Int.zero;
    //     buildingtState = null;
    // }

    private void Update()
    {
        if (buildingtState == null) { return; }
        if (FindObjectOfType<RadialMenu>() != null) { return; }
        Vector3 mousePosition = inputManager.GetSelectedMapPosition();

        Vector3 mousePositionCenter = new Vector3(Mathf.Floor(mousePosition.x) + 0.5f,50,Mathf.Floor(mousePosition.z) + 0.5f);
        Ray ray = new (mousePositionCenter, Vector3.down);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit,100, GameObject.FindObjectOfType<InputManager>().GetPlacementLayer()))
        {
            mousePositionCenter = new Vector3(mousePositionCenter.x,hit.point.y,mousePositionCenter.z);
        }

        // Vector3Int gridPosition = grid.WorldToCell(hit.point);
        // gridPositionFloat = grid.WorldToCell(hit.point);

        Vector3Int gridPosition = grid.WorldToCell(mousePositionCenter);
        // gridPositionFloat = gridPosition;
        // gridPositionFloat.y = hit.point.y;

        // print("gridPosition = " + gridPosition);
        // print("gridPositionFloat = " + gridPositionFloat);

        // Vector3Int gridPosition = grid.WorldToCell(mousePositionCenter);

        if (lastDetectedPosition != gridPosition)
        {
            buildingtState.UpdateState(gridPosition,mousePositionCenter);
            gridPositionFloat = mousePositionCenter;
            lastDetectedPosition = gridPosition;
        }

    }

    public void Placement (BuildingsEnum bType)
    {
        buildingtState.Build(gridPosition, bType, gridPositionFloat);
    }

    public void Remove()
    {
        buildingtState.Remove(gridPosition);
    }

    public Vector3Int GetCell(){
        Vector3 mousePosition = inputManager.GetSelectedMapPosition();
        gridPosition = grid.WorldToCell(mousePosition);
        return gridPosition;
    }

    public Vector3Int SelectCell(){
        // GameState buildingtState2 = new GameState(grid,preview2,floorData,buildData,objectPlacer,database);
        Vector3 mousePosition = inputManager.GetSelectedMapPosition();
        gridPosition = grid.WorldToCell(mousePosition);
        buildingtState.SelectCell(gridPosition,gridPositionFloat);
        return gridPosition;
    }

    public GameObject LoadBuildings(Vector3 position, BuildingsEnum bType){
        gridPosition = grid.WorldToCell(position);
        // buildingtState.SelectCell(gridPosition);
        if (buildingtState == null) BuildingtStateGS();
        GameObject building = buildingtState.Build(gridPosition, bType, gridPositionFloat);
        return building;
    }

    void BuildingtStateGS(){
        floorData = new ();
        buildData = new ();
        buildingtState = new GameState(grid,preview,floorData,buildData,objectPlacer,database);
    }

    public Vector3Int GetCellFromPosition(Vector3 position){
        gridPosition = grid.WorldToCell(position);
        return gridPosition;
    }

}
