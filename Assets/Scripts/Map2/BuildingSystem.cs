using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BuildingSystem : MonoBehaviour
{
    public static BuildingSystem current;
    
    public GridLayout gridLayout;
    private Grid grid;

    [SerializeField] private Tilemap MainTilemap;
    [SerializeField] private TileBase WhiteTile;

    public GameObject prefab1;
    public GameObject prefab2;

    private PlaceableObject objectToPlace;

    #region unity methods
    private void Awake()
    {
        current = this;
        grid = gridLayout.gameObject.GetComponent<Grid>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A)) { InicializeWithObject(prefab1); }
        else if (Input.GetKeyDown(KeyCode.B)) { InicializeWithObject(prefab2); }
        if (!objectToPlace)
        {
            return;
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if(CanBePlaced(objectToPlace))
            {
                objectToPlace.Place();
                Vector3Int start = gridLayout.WorldToCell(objectToPlace.GetStartPosition());
                TakeArea(start, objectToPlace.Size);
            }
            else
            {
                Destroy(objectToPlace.gameObject);
            }
        }
    }

    #endregion
    #region Utils

    public static Vector3 GetMouseWoldPosition() {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray,out RaycastHit raycastHit))
        {
            return raycastHit.point;
        }
        else { return Vector3.zero; }
    }
    public Vector3 SnapCoordinateToGrid(Vector3 position)
    {
        Vector3Int cellPos = gridLayout.WorldToCell(position);
        position = grid.GetCellCenterWorld(cellPos);
        return position;
    }

    private static TileBase[] GetTilesBlock(BoundsInt area, Tilemap tilemap)
    {
        TileBase[] array = new TileBase[area.size.x * area.size.y*area.size.z];
        int counter = 0;

        foreach(var v in area.allPositionsWithin){
           Vector3Int pos = new Vector3Int(v.x, v.y, z:0);
            array[counter] = tilemap.GetTile(pos);
            counter++;
        }
        return array;
    }
    #endregion
    #region Building Placement
    public void InicializeWithObject (GameObject prefab)
    {
        Vector3 position = SnapCoordinateToGrid(Vector3.zero);
        GameObject obj = GameObject.Instantiate(prefab,position,Quaternion.identity);
        objectToPlace = obj.GetComponent<PlaceableObject>();
        obj.AddComponent<ObjectDrag>();
    }
    private bool CanBePlaced(PlaceableObject placeabeObject)
    {
        BoundsInt area = new BoundsInt();
        area.position = gridLayout.WorldToCell(objectToPlace.GetStartPosition());
        area.size = placeabeObject.Size;

        TileBase[] baseArray = GetTilesBlock(area, MainTilemap);
        
        foreach (var b in baseArray){
            if (b == WhiteTile)
            {
                return false;
            }
        }
        return true;
    }
    public void TakeArea (Vector3Int start, Vector3Int size)
    {
        MainTilemap.BoxFill(start,WhiteTile, startX:start.x, startY:start.y,endX:start.x+size.x,endY:start.y+size.y);
    }
    #endregion

}
