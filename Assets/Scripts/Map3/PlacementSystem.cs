using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlacementSystem : MonoBehaviour
{
    [SerializeField] private InputManager inputManager;
    [SerializeField] private Grid grid;
    public ObjectsDatabaseSO database;
    [SerializeField] private GameObject gridVisualization;

    public GridData floorData, buildData;
    private Renderer previewRenderer;

    [SerializeField] private ObjectPlacer objectPlacer;

    [SerializeField] private PreviewSystem preview;

    private Vector3Int lastDetectedPosition = Vector3Int.zero;

    public GameState buildingState;
    Vector3Int gridPosition;

    public Vector3 gridPositionFloat;


    private void Start()
    {
        gridVisualization.SetActive(true);
        // StopPlacement();
        if (buildingState == null) BuildingtStateGS();
    }

    private void Update()
    {
        if (buildingState == null) { return; }
        if (FindObjectOfType<RadialMenu>() != null) { return; }
        Vector3 mousePosition = inputManager.GetSelectedMapPosition();

        Vector3 mousePositionCenter = GetCenterPositionAboveMap(mousePosition);

        Vector3Int gridPosition = grid.WorldToCell(mousePositionCenter);
        if (lastDetectedPosition != gridPosition)
        {
            buildingState.UpdateState(gridPosition, mousePositionCenter);
            gridPositionFloat = mousePositionCenter;
            lastDetectedPosition = gridPosition;
        }

    }

    public void Placement(BuildingsEnum bType)
    {
        buildingState.Build(gridPosition, bType, gridPositionFloat, true);
    }

    public void Remove()
    {
        buildingState.Remove(gridPosition);
    }

    public Vector3Int GetCell()
    {
        Vector3 mousePosition = inputManager.GetSelectedMapPosition();
        gridPosition = grid.WorldToCell(mousePosition);
        return gridPosition;
    }

    public Vector3Int SelectCell(bool secondaryIndicator = false)
    {
        Vector3 mousePosition = inputManager.GetSelectedMapPosition();
        gridPosition = grid.WorldToCell(mousePosition);
        buildingState.SelectCell(gridPosition, gridPositionFloat, secondaryIndicator);
        return gridPosition;
    }

    public Vector3Int SelectCellFromVector3(Vector3 position)
    {
        gridPosition = grid.WorldToCell(position);
        buildingState.SelectCell(gridPosition, position, false);
        return gridPosition;
    }

    public GameObject LoadBuildings(Vector3 position, BuildingsEnum bType)
    {
        gridPosition = grid.WorldToCell(position);
        // buildingtState.SelectCell(gridPosition);
        if (buildingState == null) BuildingtStateGS();
        GameObject building = buildingState.Build(gridPosition, bType, position, false);
        return building;
    }

    void BuildingtStateGS()
    {
        floorData = new();
        buildData = new();
        buildingState = new GameState(grid, preview, floorData, buildData, objectPlacer, database);
    }

    public void PlaceOre(GameObject oreGO, Vector3 orePosition)
    {
        gridPosition = grid.WorldToCell(orePosition);
        buildingState.BuildOre(gridPosition, oreGO, orePosition);
    }

    Vector3 GetCenterPositionAboveMap(Vector3 position)
    {
        Vector3 mousePositionCenter = GetCenterPositionCell(position);
        Ray ray = new(mousePositionCenter, Vector3.down);
        RaycastHit hit;
        // calculate y axy center from map
        if (Physics.Raycast(ray, out hit, 100, FindObjectOfType<InputManager>().GetPlacementLayer()))
        {
            mousePositionCenter = new Vector3(mousePositionCenter.x, hit.point.y, mousePositionCenter.z);
        }
        return mousePositionCenter;
    }

    public Vector3 GetCenterPositionCell(Vector3 position)
    {
        Vector3 mousePositionCenter = new Vector3(Mathf.Floor(position.x) + 0.5f, 50, Mathf.Floor(position.z) + 0.5f);
        return mousePositionCenter;
    }

}
