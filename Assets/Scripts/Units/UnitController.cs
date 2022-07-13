using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UnitController : MonoBehaviour
{
    public GameObject bg_circle;
    public GameController gameController;
    public UnitType unitType;
    public UnitStats unitStats;

    public Stack<Vector3> path;
    public Stack<Tile> pathOfTiles;
    public Vector3 waypoint;

    public SpriteRenderer bgRenderer;
    public Player player;

    public Tile tile;

    public GameObject healthBar;
    public GameObject orgBar;
    public TextMeshProUGUI movementCounter;
    public TextMeshProUGUI unitName;

    public GameObject encircledIcon;

    public SpriteRenderer icon;
    
    // Start is called before the first frame update
    void Start()
    {
        path = new Stack<Vector3>();
        pathOfTiles = new Stack<Tile>();
        waypoint = transform.position;      
        encircledIcon.SetActive(false);  
    }

    public void Delete() {
        gameController.units.Remove(this);
        gameController.unitObjects.Remove(gameObject);
        tile.unit = null;
        Destroy(gameObject);
    }

    public void Init(UnitType newUnitType, List<UnitModifier> newUnitModifiers, GameController newGameController, Player owner, string newName) {
        unitType = newUnitType;
        unitStats = new UnitStats(unitType, newUnitModifiers, this);
        gameController = newGameController;
        unitName.text = newName;
        SetPlayer(owner);
        UpdateHealthbar();
        UpdateOrgbar();
        UpdateMovementCounter();
        icon.sprite = unitType.icon;
    }

    // Update is called once per frame  
    void Update()
    {
        if (gameController == null) {
            return;
        }

        if ((this != gameController.selectedUnit) && bg_circle.activeSelf) {
            bg_circle.SetActive(false);
        }

        if (waypoint != null) {
            if (transform.position == waypoint) {
                if (path.Count > 0) {
                    waypoint = path.Pop();
                    var nextTile = pathOfTiles.Pop();
                    if (nextTile.unit != null && nextTile.unit.player != player) {
                        waypoint = transform.position;
                        Attack(nextTile.unit);
                    }
                } 
            }            
            transform.position = Vector3.MoveTowards(
                transform.position,
                waypoint,
                0.05f);
        }
    }

    public void NewTurn() {
        if (!IsEncircled()) {
            encircledIcon.SetActive(false);
        } else {            
            encircledIcon.SetActive(true);
        }        
        unitStats.NewTurn(IsEncircled());
        UpdateHealthbar();
        UpdateOrgbar();
        UpdateMovementCounter();
    }

    public void Attack(UnitController otherUnit) {        
        Combat.CalculateFight(this, otherUnit);
        SetMovement(0.0f);
    }    

    public bool IsEncircled() {
        bool neighboursAreCutOff = false;
        foreach (var neighbour in tile.neighbours) {
            if (neighbour.unit != null && neighbour.unit.player.IsAlliedTo(player)) {
                neighboursAreCutOff = neighboursAreCutOff || neighbour.unit.CanRetreat();
            }
        }
        return !CanRetreat() && !neighboursAreCutOff && tile.tileType != TileType.City;
    }

    public bool CanRetreat() {
        return FindRetreatableTiles().Count > 0;
    }

    public List<Tile> FindRetreatableTiles() {
        var neighbours = tile.neighbours;
        var result = new List<Tile>();
        foreach (var neighbour in neighbours) {
            if (neighbour.unit != null || gameController.map.Distance(tile, neighbour) > unitStats.movementSpeed) {
                continue;
            }
            bool isClosed = false;
            foreach (var otherNeighbour in neighbour.neighbours) {
                if (otherNeighbour == neighbour || !neighbours.Contains(otherNeighbour)) {
                    continue;
                }
                isClosed = isClosed || (otherNeighbour.unit != null && !otherNeighbour.unit.player.IsAlliedTo(player));
            }
            if (!isClosed) {
                result.Add(neighbour);
            }
        }
        return result;
    }

    public void Retreat() {
        var retreatableTiles = FindRetreatableTiles();
        var tileIndex = (int) Mathf.Floor(Random.value * retreatableTiles.Count);
        Advance(retreatableTiles[tileIndex]);
    }

    public void Advance(Tile destination) {
        gameController.movementManager.Move(tile, destination);
    }    

    public void UpdateHealthbar() {
        healthBar.transform.localScale = new Vector3(unitStats.currentHealth / unitStats.health, 1.0f, -1.0f);
        healthBar.transform.localPosition = new Vector3(-0.5f + 0.5f * (unitStats.currentHealth / unitStats.GetHealth()), 0.0f, -1.0f);
    }

    public void UpdateOrgbar() {
        orgBar.transform.localScale = new Vector3(unitStats.currentOrganization / unitStats.organization, 1.0f, -1.0f);
        orgBar.transform.localPosition = new Vector3(-0.5f + 0.5f * (unitStats.currentOrganization / unitStats.GetOrganization()), 0.0f, -1.0f);
    }

    public void SetMovement(float newMovement) {
        unitStats.currentMovement = newMovement;
        UpdateMovementCounter();
    }

    public void UpdateMovementCounter() {
        if (player != null && player.isHuman) {
            movementCounter.text = string.Format("{0:N1}", unitStats.currentMovement);
        }
    }

    public void SetPath(Queue<Tile> newPath) {
        if (newPath == null) {
            return;
        }
        float distance = 0.0f;
        Tile prev = null;
        foreach(var node in newPath) {
            var position = node.tileObject.transform.position;
            path.Push(new Vector3(position.x, position.y, transform.position.z));
            pathOfTiles.Push(node);
            if (prev != null) {
                distance += gameController.map.Distance(node, prev);
            }
            prev = node;
        }
        distance += gameController.map.Distance(tile, prev);
        unitStats.currentMovement -= distance;
        UpdateMovementCounter();
    }

    void OnMouseEnter() {
        if (gameController != null) {
            gameController.ui.MouseOnUnit(this);
        }
        transform.localScale = 1.1f * transform.localScale;
    }

    void OnMouseExit() {
        if (gameController != null) {
            gameController.ui.DisableMouseOnUnit();
        }
        transform.localScale = (10.0f / 11.0f) * transform.localScale;
    }

    public void OnSelect() {
        bg_circle.SetActive(true);
    }

    public void SetPlayer(Player newPlayer) {
        player = newPlayer;
        bgRenderer.color = newPlayer.color;
    }
}
