using System;
using System.Collections;
using System.Collections.Generic;
using System.Resources;
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

    public void EndState(bool secondaryIndicator = false)
    {
        previewSystem.StopShowingPreview(secondaryIndicator);
    }

    public void OnAction(Vector3Int gridPosition)
    {
        focus = !focus;
    }

    private bool CheckIfSelectedIsValid(Vector3Int gridPosition)
    {
        return !(buildData.CanPlaceObjectAt(gridPosition, Vector2Int.one) && floorData.CanPlaceObjectAt(gridPosition, Vector2Int.one));
    }

    public void UpdateState(Vector3Int gridPosition, Vector3 gridPositionFloat)
    {
        if (GameObject.FindObjectOfType<RadialMenu>()) return;

        bool validity = CheckIfSelectedIsValid(gridPosition);
        Vector3 positionWorld = grid.CellToWorld(gridPosition);
        // Debug.Log(gridPositionFloat);
        // Debug.Log(positionWorld);
        // Debug.Log(gridPosition);
        // positionWorld.y = gridPositionFloat.y;
        previewSystem.UndatePosition(gridPositionFloat, validity);
        // previewSystem.UndatePosition(grid.CellToWorld(gridPosition), true);
    }

    private bool CheckPlacementValidity(Vector3Int gridPosition, Vector2Int size)
    {
        GridData selectedData = floorData;
        return selectedData.CanPlaceObjectAt(gridPosition, size);
    }

    public GameObject Build(Vector3Int gridPosition, BuildingsEnum bType, Vector3 gridPositionFloat)
    {
        ObjectData buildingData = database.objectsData.Find(x => x.Type == bType);
        bool placementValidity = CheckPlacementValidity(gridPosition, buildingData.Size);
        if (!placementValidity && bType != BuildingsEnum.Extractor) { return null; }
        // int index = objectPlacer.PlaceObject(database.objectsData[selectedObjectIndex].Prefab, grid.CellToWorld(gridPosition));
        Vector3 positionWorld = grid.CellToWorld(gridPosition);

        (int, GameObject) dataB = objectPlacer.PlaceBuild(buildingData.Prefab, gridPositionFloat, bType);
        // GridData selectedData = database.objectsData[selectedObjectIndex].ID == 0 ? floorData : buildData;
        GridData selectedData = floorData;
        selectedData.AddObjectAt(gridPosition, buildingData.Size, buildingData.ID, dataB.Item1);
        dataB.Item2.GetComponent<Building>().data.id = dataB.Item1;
        SelectCell(gridPosition, gridPositionFloat);
        // previewSystem.UndatePosition(grid.CellToWorld(gridPosition), false, true);
        return dataB.Item2;
    }

    public GameMaterialSO GetOreResource(Vector3 position)
    {
        Vector3Int gridPosition = grid.WorldToCell(position);
        GameObject oreGO = GetGOFromCell(gridPosition);
        if (!oreGO || !oreGO.GetComponent<Ore>())
            return MaterialManager.GetGameMaterialSO(GameMaterialsEnum.copper);
        GameMaterialSO gameMaterialSO = oreGO.GetComponent<Ore>().oreData.gameMaterialSO;
        Remove(gridPosition);
        return gameMaterialSO;
    }

    public void Remove(Vector3Int gridPosition)
    {
        GridData selectedData = null;
        if (buildData.CanPlaceObjectAt(gridPosition, Vector2Int.one) == false)
        {
            selectedData = buildData;
        }
        else if (floorData.CanPlaceObjectAt(gridPosition, Vector2Int.one) == false)
        {
            selectedData = floorData;
        }

        if (selectedData == null) return;

        selectedObjectIndex = selectedData.GetRepresentationIndex(gridPosition);
        if (selectedObjectIndex == -1) { return; }
        selectedData.RemoveObjectAt(gridPosition);
        objectPlacer.RemoveObjectAt(selectedObjectIndex);
    }

    public GameObject GetGOFromCell(Vector3Int gridPosition)
    {
        GridData selectedData = null;
        if (buildData.CanPlaceObjectAt(gridPosition, Vector2Int.one) == false)
        {
            selectedData = buildData;
        }
        else if (floorData.CanPlaceObjectAt(gridPosition, Vector2Int.one) == false)
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
            if (selectedObjectIndex == -1) { return null; }
            return objectPlacer.GetGObjectAt(selectedObjectIndex);
        }
        return null;
    }

    public void SelectCell(Vector3Int gridPosition, Vector3 gridPositionFloat, bool secondaryIndicator = false)
    {
        previewSystem.UndatePosition(gridPositionFloat, true, true, secondaryIndicator);
    }

    public void BuildOre(Vector3Int gridPosition, GameObject oreGO, Vector3 position)
    {
        bool placementValidity = CheckPlacementValidity(gridPosition, new Vector2Int(1, 1));
        if (!placementValidity) return;
        (int, GameObject) data = objectPlacer.PlaceOre(oreGO, position);
        GridData selectedData = floorData;
        selectedData.AddObjectAt(gridPosition, new Vector2Int(1, 1), 0, data.Item1);
    }
}
