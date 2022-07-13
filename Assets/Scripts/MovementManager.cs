using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MovementManager
{
    public GameController gameController;
    public Map map;
    
    public MovementManager(GameController gameController) {
        this.gameController = gameController;
        map = gameController.map;
    }

    public void SetTiles(Tile start, Tile goal) {
        var tmp = goal.unit;
        goal.unit = start.unit;
        start.unit = tmp;
        goal.unit.tile = goal;
        if (start.unit != null) {
            start.unit.tile = start;
        }
    }

    public void Move(Tile start, Tile goal) {
        if (goal.unit == null) {
            MoveToEmptyTile(start, goal);
        } else {
            if (start.unit.player == goal.unit.player) {
                Switch(start, goal);
            } else if (!start.unit.player.IsAlliedTo(goal.unit.player)) {
                Attack(start, goal);
            } else {
                Debug.LogWarning("Moving to a tile with an allied unit!");
            }
        }
    }

    private void MoveToEmptyTile(Tile start, Tile goal) {
        start.unit.SetPath(map.FindPath(map.At(start.coords.x, start.coords.y), map.At(goal.coords.x, goal.coords.y)));
        SetTiles(start, goal);
    }

    private void Attack(Tile start, Tile goal) {
        start.unit.SetPath(map.FindPath(map.At(start.coords.x, start.coords.y), map.At(goal.coords.x, goal.coords.y)));

        int pathSize = start.unit.pathOfTiles.Count;
        if (pathSize > 1) {        
            var penultimateTile = start.unit.pathOfTiles.ElementAt(pathSize - 2);
            SetTiles(start, penultimateTile);
        }
    }

    private void Switch(Tile start, Tile goal) {
        var startQueue = new Queue<Tile>();
        startQueue.Enqueue(goal);
        start.unit.SetPath(startQueue);
        
        var goalQueue = new Queue<Tile>();
        goalQueue.Enqueue(start);
        goal.unit.SetPath(goalQueue);
        
        SetTiles(start, goal);
    }
}
