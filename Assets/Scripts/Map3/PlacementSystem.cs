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

    private GridData floorData, buildData;
    private Renderer previewRenderer;

    [SerializeField] private ObjectPlacer objectPlacer;

    [SerializeField] private PreviewSystem preview;
    
    private Vector3Int lastDetectedPosition = Vector3Int.zero;

    IBuildingtState buildingtState;

    private void Start()
    {
        StopPlacement();
        floorData = new ();
        buildData = new ();
    }
    public void StartPlacement (int ID)
    {
        StopPlacement();
        gridVisualization.SetActive(true);
        buildingtState = new PlacementState (ID,grid,preview,database,floorData,buildData,objectPlacer);
        inputManager.OnCliched += PlaceStructure;
        inputManager.OnExit += StopPlacement;
    }
    public void StartRemoving()
    {
        StopPlacement();
        gridVisualization.SetActive (true);
        buildingtState = new RemovingState(grid,preview,floorData,buildData,objectPlacer);

        inputManager.OnCliched += PlaceStructure;
        inputManager.OnExit += StopPlacement;
    }
    private void PlaceStructure()
    {
        if (inputManager.IsPointerOverUI())
        {
            Debug.Log("Vuelve :" + inputManager.IsPointerOverUI());
            return;
        }
        Vector3 mousePosition = inputManager.GetSelectedMapPosition();
        Vector3Int gridPosition = grid.WorldToCell(mousePosition);
        buildingtState.OnAction(gridPosition);
    }

    /*
    private bool CheckPlacementValidity(Vector3Int gridPosition, int selectedObjectIndex)
    {
        GridData selectedData = database.objectsData[selectedObjectIndex].ID == 0 ? floorData : buildData;
        return selectedData.CanPlaceObjectAt(gridPosition, database.objectsData[selectedObjectIndex].Size);
    }
    */
    private void StopPlacement()
    {
        if (buildingtState == null) { return; }
        gridVisualization.SetActive(false);
        buildingtState.EndState();
        inputManager.OnCliched -= PlaceStructure;
        inputManager.OnExit -= StopPlacement;
        lastDetectedPosition = Vector3Int.zero;
        buildingtState = null;
    }

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

}
