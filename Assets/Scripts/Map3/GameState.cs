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

    private (bool, Color) CheckIfSelectedIsValid(Vector3Int gridPosition, Vector3 gridPositionFloat)
    {
        if (CheckObstacle(gridPositionFloat)) return (true, Color.red);

        return (!(buildData.CanPlaceObjectAt(gridPosition, Vector2Int.one) && floorData.CanPlaceObjectAt(gridPosition, Vector2Int.one)), Color.cyan);
    }

    public bool CheckObstacle(Vector3 gridPositionFloat)
    {
        Ray ray = new Ray(new(gridPositionFloat.x, 50, gridPositionFloat.z), Vector3.down);
        RaycastHit hit;
        // Check for an obstacle first
        if (Physics.Raycast(ray, out hit, 100f, LayerMask.GetMask("Obstacles")))
        {
            // Ray hit an obstacle, stop further processing
            return true;
        }
        return false;
    }

    public void UpdateState(Vector3Int gridPosition, Vector3 gridPositionFloat)
    {
        if (GameObject.FindObjectOfType<RadialMenu>()) return;

        (bool validity, Color color) = CheckIfSelectedIsValid(gridPosition, gridPositionFloat);

        previewSystem.UndatePosition(gridPositionFloat, validity, color);
    }

    private bool CheckPlacementValidity(Vector3Int gridPosition, Vector2Int size)
    {
        GridData selectedData = floorData;
        if (!selectedData.CanPlaceObjectAt(gridPosition, size)) return false;


        return true;
    }

    public GameObject Build(Vector3Int gridPosition, BuildingsEnum bType, Vector3 gridPositionFloat, bool completeBuilding)
    {
        ObjectData buildingData = database.objectsData.Find(x => x.Type == bType);
        bool placementValidity = CheckPlacementValidity(gridPosition, buildingData.Size);
        if (!placementValidity && bType != BuildingsEnum.Extractor) { return null; }
        // int index = objectPlacer.PlaceObject(database.objectsData[selectedObjectIndex].Prefab, grid.CellToWorld(gridPosition));
        Vector3 positionWorld = grid.CellToWorld(gridPosition);

        (int, GameObject) dataB = objectPlacer.PlaceBuild(buildingData.Prefab, gridPositionFloat, bType, completeBuilding);
        // GridData selectedData = database.objectsData[selectedObjectIndex].ID == 0 ? floorData : buildData;
        GridData selectedData = floorData;
        if (bType == BuildingsEnum.Extractor) Remove(gridPosition);
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
        previewSystem.UndatePosition(gridPositionFloat, true, Color.cyan, true, secondaryIndicator);
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
