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

    public GameState buildingtState;
    Vector3Int gridPosition;

    Vector3 gridPositionFloat;


    private void Start()
    {
        gridVisualization.SetActive(true);
        // StopPlacement();
        if (buildingtState == null) BuildingtStateGS();
    }

    private void Update()
    {
        if (buildingtState == null) { return; }
        if (FindObjectOfType<RadialMenu>() != null) { return; }
        Vector3 mousePosition = inputManager.GetSelectedMapPosition();

        Vector3 mousePositionCenter = new Vector3(Mathf.Floor(mousePosition.x) + 0.5f, 50, Mathf.Floor(mousePosition.z) + 0.5f);
        Ray ray = new(mousePositionCenter, Vector3.down);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100, GameObject.FindObjectOfType<InputManager>().GetPlacementLayer()))
        {
            mousePositionCenter = new Vector3(mousePositionCenter.x, hit.point.y, mousePositionCenter.z);
        }

        Vector3Int gridPosition = grid.WorldToCell(mousePositionCenter);
        if (lastDetectedPosition != gridPosition)
        {
            buildingtState.UpdateState(gridPosition, mousePositionCenter);
            gridPositionFloat = mousePositionCenter;
            lastDetectedPosition = gridPosition;
        }

    }

    public void Placement(BuildingsEnum bType)
    {
        buildingtState.Build(gridPosition, bType, gridPositionFloat, true);
    }

    public void Remove()
    {
        buildingtState.Remove(gridPosition);
    }

    public Vector3Int GetCell()
    {
        Vector3 mousePosition = inputManager.GetSelectedMapPosition();
        gridPosition = grid.WorldToCell(mousePosition);
        return gridPosition;
    }

    public Vector3Int SelectCell(bool secondaryIndicator = false)
    {
        // GameState buildingtState2 = new GameState(grid,preview2,floorData,buildData,objectPlacer,database);
        Vector3 mousePosition = inputManager.GetSelectedMapPosition();
        gridPosition = grid.WorldToCell(mousePosition);
        buildingtState.SelectCell(gridPosition, gridPositionFloat, secondaryIndicator);
        return gridPosition;
    }

    public GameObject LoadBuildings(Vector3 position, BuildingsEnum bType)
    {
        gridPosition = grid.WorldToCell(position);
        // buildingtState.SelectCell(gridPosition);
        if (buildingtState == null) BuildingtStateGS();
        GameObject building = buildingtState.Build(gridPosition, bType, position, false);
        return building;
    }

    void BuildingtStateGS()
    {
        floorData = new();
        buildData = new();
        buildingtState = new GameState(grid, preview, floorData, buildData, objectPlacer, database);
    }

    public void PlaceOre(GameObject oreGO, Vector3 orePosition)
    {
        gridPosition = grid.WorldToCell(orePosition);
        buildingtState.BuildOre(gridPosition, oreGO, orePosition);
    }

}
