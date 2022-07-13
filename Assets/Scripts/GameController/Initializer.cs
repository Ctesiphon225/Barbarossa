using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Initializer
{
    public static void InitializeTile(int i, int j, Map map, GameObject tileObject, Transform transform) {
        var tile = map.tiles[i][j];  
        var newTile = MonoBehaviour.Instantiate(tileObject, transform);               
        newTile.transform.localScale *= 1.135f;
        newTile.transform.position = Coords.WorldCoords(tile.coords, 0.0f);
        map.tiles[i][j].tileObject = newTile;  
        newTile.GetComponent<TileController>().tile = map.tiles[i][j];   
        if (newTile.GetComponent<TileController>().text != null) {
            newTile.GetComponent<TileController>().text.text = map.tiles[i][j].text;  
        }
        newTile.GetComponent<TileController>().SetColor(tile.tileType);
    }

    public static GameObject InitializePath(Vector2Int first, Vector2Int second, GameObject pathObject, bool rotate, Map map, Transform transform) {
        var firstTile = map.At(first.x, first.y).tileObject.transform.position;
        var secondTile = map.At(second.x, second.y).tileObject.transform.position;
        var newPath = MonoBehaviour.Instantiate(pathObject, transform);
        newPath.transform.position = 0.5f * (firstTile + secondTile + new Vector3(0.0f, 0.0f, -0.01f));
        if (rotate) {
            newPath.transform.Rotate(0.0f, 0.0f, 90.0f, Space.Self);
        }
        RotateRiver(map.At(first.x, first.y), map.At(second.x, second.y), newPath);
        return newPath;
    }

    public static void RotateRiver(Tile first, Tile second, GameObject river) {
        if (first.coords.y == second.coords.y) {
            return;
        }
        var vector = first.coords - second.coords;
        if (first.coords.y % 2 == 0) {
            vector.x--;
        }

        if ((vector.x == 0 && vector.y == -1) || (vector.x == -1 && vector.y == 1)) {
            river.transform.Rotate(0.0f, 0.0f, 60.0f, Space.Self);
        } else {
            river.transform.Rotate(0.0f, 0.0f, -60.0f, Space.Self);
        }
    }

    public static void InitializeUnit(GameObject unitObject, Vector2Int coords, UnitType unitType, Player owner, int name, Map map, GameController gameController) {
        var newUnit = InitializeUnit(unitObject, coords, unitType, owner, name, map, gameController.gameObject.transform, gameController);
        gameController.units.Add(newUnit);
    }
    
    public static void InitializeUnit(GameObject unitObject, Vector2Int coords, UnitType unitType, Player owner, int name, Map map, EditorController editorController) {
        var newUnit = InitializeUnit(unitObject, coords, unitType, owner, name, map, editorController.gameObject.transform, null);
        editorController.units.Add(newUnit);
    }

    private static UnitController InitializeUnit(GameObject unitObject, Vector2Int coords, UnitType unitType, Player owner, int name, Map map, Transform transform, GameController gameController) {
        var newUnit = MonoBehaviour.Instantiate(unitObject, transform);
        newUnit.transform.position = Coords.WorldCoords(coords, -0.02f);
        newUnit.transform.localScale *= 0.1f;
        var newUnitController = newUnit.GetComponent<UnitController>();        
        newUnitController.gameController = null;
        newUnitController.Init(unitType, owner.generalModifiers, null, owner, name.ToString());        
        var mapTile = map.At(coords.x, coords.y);
        mapTile.unit = newUnitController;
        newUnitController.tile = mapTile;
        return newUnitController;
    }

    public static void InitializeMapObject(GameObject mapObject, Vector2Int coords, Transform transform) {
        var newObject = MonoBehaviour.Instantiate(mapObject, transform);
        newObject.transform.position = Coords.WorldCoords(coords, -0.001f);
    }
}
