using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemovingState : IBuildingtState
{
    private int gameObjectIndex = -1;
    Grid grid;
    PreviewSystem previewSystem;
    GridData floorData;
    GridData buildData;
    ObjectPlacer objectPlacer;

    public RemovingState(Grid grid,
                         PreviewSystem previewSystem,
                         GridData floorData,
                         GridData buildData,
                         ObjectPlacer objectPlacer)
    {
        this.grid = grid;
        this.previewSystem = previewSystem;
        this.floorData = floorData;
        this.buildData = buildData;
        this.objectPlacer = objectPlacer;

        // previewSystem.StartShowingRemovePreview();
    }

    public void EndState()
    {
        // previewSystem.StopShowingPreview();
    }

    public void OnAction(Vector3Int gridPosition)
    {
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
            gameObjectIndex = selectedData.GetRepresentationIndex(gridPosition);
            if (gameObjectIndex == -1) { return; }
            selectedData.RemoveObjectAt(gridPosition);
            objectPlacer.RemoveObjectAt(gameObjectIndex);
        }
        // Vector3 cellPosition = grid.CellToWorld(gridPosition);
        // previewSystem.UndatePosition(cellPosition, CheckIfSelectedIsValid(gridPosition));
    }

    // private bool CheckIfSelectedIsValid(Vector3Int gridPosition)
    // {
    //     return !(buildData.CanPlaceObjectAt(gridPosition, Vector2Int.one) && floorData.CanPlaceObjectAt(gridPosition,Vector2Int.one));
    // }

    public void UpdateState(Vector3Int gridPosition, Vector3 gridPositionFloat)
    {
        // bool validity = CheckIfSelectedIsValid(gridPosition);
        // previewSystem.UndatePosition(grid.CellToWorld(gridPosition), validity);
    }
}
