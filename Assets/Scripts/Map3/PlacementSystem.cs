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
        Vector3 mousePosition = inputManager.GetSelectedMapPosition();
        Vector3Int gridPosition = grid.WorldToCell(mousePosition);
        if (lastDetectedPosition != gridPosition)
        {
            buildingtState.UpdateState(gridPosition);
            lastDetectedPosition = gridPosition;
        }

    }

    public void Placement (BuildingsEnum bType)
    {
        // buildingtState = new PlacementState (ID,grid,preview,database,floorData,buildData,objectPlacer);
        buildingtState.Build(gridPosition, bType);
    }

    public void Remove()
    {
        // buildingtState = new RemovingState(grid,preview,floorData,buildData,objectPlacer);
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
        buildingtState.SelectCell(gridPosition);
        return gridPosition;
    }

    public GameObject LoadBuildings(Vector3 position, BuildingsEnum bType){
        gridPosition = grid.WorldToCell(position);
        // buildingtState.SelectCell(gridPosition);
        if (buildingtState == null) BuildingtStateGS();
        GameObject building = buildingtState.Build(gridPosition, bType);
        return building;
    }

    void BuildingtStateGS(){
        floorData = new ();
        buildData = new ();
        buildingtState = new GameState(grid,preview,floorData,buildData,objectPlacer,database);
    }

}
