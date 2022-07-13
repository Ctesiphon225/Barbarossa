using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputHandler
{
    public GameController game;
    public UIHandler ui;

    private GameObject target;
    private Queue<Tile> currentPath;
    private Vector2Int tileCoords;
    private Vector2Int goalCoords;

    public InputHandler(GameController gameController) {
        this.game = gameController;
        this.ui = gameController.ui;
    }

    public void Handle() {
        if (game.currentPlayer.isHuman) {
            if (Input.GetMouseButtonDown(0)) {
                SelectObjectWithClick("leftMouse");
            }
            if (Input.GetMouseButtonDown(1) || Input.GetMouseButton(1)) {
                SelectObjectWithClick("rightMouse");
            }
            if (Input.GetMouseButtonUp(1)) {
                SetPaths();
            }     
            if (Input.GetKeyDown(KeyCode.Space)) {
                game.EndTurn();
            } 
        }
    }

    void SelectObjectWithClick(string input) {
        Ray ray = game.mainCamera.ScreenPointToRay(Input.mousePosition);

        if (input == "leftMouse") {
            DisableHighlightedTiles();
            if (Physics.Raycast(ray, out RaycastHit hitInfo)) {
                var target = hitInfo.collider.gameObject;
                if (target.GetComponent<UnitController>() != null) {
                    SelectUnit(target);
                }                
            } else {
                UnselectUnit();
            }
        }
        if (input == "rightMouse") {
            if (Physics.Raycast(ray, out RaycastHit hitInfo)) {
                target = hitInfo.collider.gameObject;
                if (
                    (target.GetComponent<TileController>() != null || target.GetComponent<UnitController>() != null) 
                    && game.selectedUnit != null && target.GetComponent<UnitController>() != game.selectedUnit) {
                    
                    MoveUnit(game.selectedUnit, target);
                }
            }
        }
    }

    void SelectUnit(GameObject target) {
        if (game.selectedUnit != null) {
            game.mouseOnUnitScreen.transform.position = game.mouseOnUnitScreen.transform.position + new Vector3(-300.0f, 0.0f, 0.0f);
        }
        game.selectedUnit = target.GetComponent<UnitController>();
        if (game.selectedUnit.player != game.currentPlayer) {
            game.selectedUnit = null;
            return;
        }
        game.selectedUnit.OnSelect();
        HighlightPaths();
        game.selectedUnitScreen.SetActive(true);
        game.mouseOnUnitScreen.SetActive(false);
        game.mouseOnUnitScreen.transform.position = game.mouseOnUnitScreen.transform.position + new Vector3(300.0f, 0.0f, 0.0f); 
        ui.FillUnitScreen(game.selectedUnit, game.selectedUnitScreen);
    }

    void UnselectUnit() {
        if (game.selectedUnit != null) {            
            game.mouseOnUnitScreen.transform.position = game.mouseOnUnitScreen.transform.position + new Vector3(-300.0f, 0.0f, 0.0f);
            game.selectedUnit = null;
        }
        game.selectedUnitScreen.SetActive(false);
    }

    void MoveUnit(UnitController unit, GameObject target) {
        if (!((target.GetComponent<TileController>() != null || target.GetComponent<UnitController>() != null) && game.selectedUnit != null)) {
            return;
        }

        tileCoords = Coords.MapCoords(unit.gameObject);

        if (Coords.MapCoords(target) == goalCoords) {
            return;
        }

        goalCoords = Coords.MapCoords(target);
        if (!IsValidTarget(goalCoords)) {
            return;
        }

        DisableHighlightedPath();
        currentPath = game.map.FindPath(game.map.At(tileCoords.x, tileCoords.y), game.map.At(goalCoords.x, goalCoords.y));
        HighlightPath();
    }

    private void SetPaths() {
        if (game.selectedUnit == null || goalCoords == tileCoords || game.selectedUnit.unitStats.currentMovement == 0.0f) {
            if (goalCoords == tileCoords) {
                DisableHighlightedPath();
            }
            return;
        }
        
        var otherUnit = game.map.At(goalCoords.x, goalCoords.y).unit;
        
        if (!IsValidTarget(goalCoords)) {
            return;
        }  

        game.movementManager.Move(game.map.At(tileCoords.x, tileCoords.y), game.map.At(goalCoords.x, goalCoords.y));

        DisableHighlightedTiles();
        DisableHighlightedPath();
        UnselectUnit();
        currentPath = null;
    }

    private bool IsValidTarget(Vector2Int coords) {
        return game.map.At(coords.x, coords.y).tileObject.GetComponent<TileController>().isHighlighted;
    }    

    public void DisableHighlightedTiles() {
        foreach (var tile in game.highlightedTiles) {
            tile.tileObject.GetComponent<TileController>().DisableHighlight();
        }
        game.highlightedTiles.Clear();
    }

    public void HighlightPaths() {
        game.highlightedTiles.Clear();
        if (game.selectedUnit == null) {
            return;
        }
        var coords = Coords.MapCoords(game.selectedUnit.gameObject);
        foreach (var tile in game.map.BFS(game.selectedUnit.unitStats.currentMovement, game.map.At(coords.x, coords.y))) {
            tile.tileObject.GetComponent<TileController>().EnableHighlight(game.currentPlayer);
            game.highlightedTiles.Add(tile);
        }
    }

    public void HighlightPath() {
        Tile prev = null;
        foreach (var tile in currentPath) {
            if (prev != null) {
                game.highlightedPath.Add(Initializer.InitializePath(prev.coords, tile.coords, game.pathHighlight, true, game.map, game.transform));
            }
            prev = tile;
        }
        game.highlightedPath.Add(Initializer.InitializePath(prev.coords, game.selectedUnit.tile.coords, game.pathHighlight, true, game.map, game.transform));
    }

    public void DisableHighlightedPath() {
        foreach (var pathHighlight in game.highlightedPath) {            
            MonoBehaviour.Destroy(pathHighlight);
        }
        game.highlightedPath.Clear();
    }
}
