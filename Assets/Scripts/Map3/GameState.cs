using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameState : IBuildingtState
{
    private int selectedObjectIndex = -1;
    Grid grid;
    PreviewSystem previewSystem;
    GridData floorData;
    GridData buildData;
    ObjectPlacer objectPlacer;
    ObjectsDatabaseSO database;
    bool focus;

    public GameState(Grid grid,
                         PreviewSystem previewSystem,
                         GridData floorData,
                         GridData buildData,
                         ObjectPlacer objectPlacer,
                         ObjectsDatabaseSO database)
    {
        this.grid = grid;
        this.previewSystem = previewSystem;
        this.floorData = floorData;
        this.buildData = buildData;
        this.objectPlacer = objectPlacer;
        this.database = database;

        previewSystem.StartShowingPreview();
    }

    public void EndState()
    {
        previewSystem.StopShowingPreview();
    }

    public void OnAction(Vector3Int gridPosition)
    {
        focus = !focus;
    }

    private bool CheckIfSelectedIsValid(Vector3Int gridPosition)
    {
        return !(buildData.CanPlaceObjectAt(gridPosition, Vector2Int.one) && floorData.CanPlaceObjectAt(gridPosition,Vector2Int.one));
    }

    public void UpdateState(Vector3Int gridPosition)
    {
        if (GameObject.FindObjectOfType<RadialMenu>()) return;
        bool validity = CheckIfSelectedIsValid(gridPosition);
        previewSystem.UndatePosition(grid.CellToWorld(gridPosition), validity);
        // previewSystem.UndatePosition(grid.CellToWorld(gridPosition), true);
    }

    private bool CheckPlacementValidity(Vector3Int gridPosition, ObjectData selectedObject)
    {
        GridData selectedData = floorData;
        return selectedData.CanPlaceObjectAt(gridPosition, selectedObject.Size);
    }

    public void Build(Vector3Int gridPosition, int selectedObjectIndex, BuildingsEnum bType){
        ObjectData buildingData = database.objectsData.Find(x => x.Type == bType);
        bool placementValidity = CheckPlacementValidity(gridPosition, buildingData);
        if (!placementValidity) { return; }
        // int index = objectPlacer.PlaceObject(database.objectsData[selectedObjectIndex].Prefab, grid.CellToWorld(gridPosition));
        int index = objectPlacer.PlaceBuild(buildingData.Prefab, grid.CellToWorld(gridPosition),bType);
        // GridData selectedData = database.objectsData[selectedObjectIndex].ID == 0 ? floorData : buildData;
        GridData selectedData = floorData;
        selectedData.AddObjectAt(gridPosition, buildingData.Size, buildingData.ID, index);
        previewSystem.UndatePosition(grid.CellToWorld(gridPosition), false);
    }

    public void Remove(Vector3Int gridPosition){
        GridData selectedData = null;
        if (buildData.CanPlaceObjectAt(gridPosition,Vector2Int.one) == false) 
        {
            selectedData = buildData;
        }
        else if (floorData.CanPlaceObjectAt(gridPosition,Vector2Int.one)== false)
        {
            selectedData = floorData;
        }
        if (selectedData == null)
        {
            //sound
        }
        else
        {
            selectedObjectIndex = selectedData.GetRepresentationIndex(gridPosition);
            if (selectedObjectIndex == -1) { return; }
            selectedData.RemoveObjectAt(gridPosition);
            objectPlacer.RemoveObjectAt(selectedObjectIndex);
        }
    }
}
