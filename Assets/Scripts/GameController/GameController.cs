using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class GameController : MonoBehaviour
{    
    public Map map;

    public TextAsset mapTxt;
    public List<GameObject> tileObjects;
    public List<GameObject> unitObjects;
    public List<GameObject> riverObjects;
    public List<GameObject> railroadObjects;
    public GameObject fortressObject;
    public GameObject trenchObject;
    public GameObject airportObject;
    
    public List<Tile> highlightedTiles;
    public GameObject pathHighlight;

    public List<Player> players;

    public List<UnitType> unitTypes;
    public Dictionary<string, UnitType> unitTypesDict;

    public UnitController selectedUnit = null;
    public List<UnitController> units;

    public Player currentPlayer;
    public int currentPlayerIndex;
    public int turnCounter;
    public Text turnCounterField;

    public Camera mainCamera;

    public List<GameObject> highlightedPath;

    private TileCombatModifier baseTileCombatModifier = new TileCombatModifier(true);

    public GameObject selectedUnitScreen;
    public GameObject mouseOnUnitScreen;

    // input
    public Button endTurnButton;

    public MovementManager movementManager;
    public InputHandler inputHandler;
    public UIHandler ui;

    // Start is called before the first frame update
    void Start()
    {
        selectedUnitScreen.SetActive(false);
        mouseOnUnitScreen.SetActive(false);

        highlightedTiles = new List<Tile>();
        highlightedPath = new List<GameObject>();
        map = new Map(mapTxt.text, this);
        movementManager = new MovementManager(this);
        ui = new UIHandler(this);
        inputHandler = new InputHandler(this);

        // initialize tiles
        for (int i = 0; i < map.tiles.Count; i++) {
            for (int j = 0; j < map.tiles[i].Count; j++) {
                Initializer.InitializeTile(i, j, map, tileObjects[5], transform);
            }
        }

        // initalize rivers
        foreach (var river in map.smallRivers) {
            Initializer.InitializePath(river.Item1.coords, river.Item2.coords, riverObjects[0], false, map, transform);
        }
        foreach (var river in map.largeRivers) {
            Initializer.InitializePath(river.Item1.coords, river.Item2.coords, riverObjects[1], false, map, transform);
        }

        // initialize railroads
        foreach (var railroad in map.railroads) {
            Initializer.InitializePath(railroad.Item1.coords, railroad.Item2.coords, railroadObjects[0], true, map, transform);
        }    

        // initialize trenches
        foreach (var trench in map.trenches) {
            Initializer.InitializePath(trench.Item1.coords, trench.Item2.coords, trenchObject, false, map, transform);
        }    
        
        // initialize unit type dictionary
        unitTypesDict = new Dictionary<string, UnitType>();
        foreach (var unitType in unitTypes) {
            unitTypesDict.Add(unitType.className, unitType);
        }

        // initialize units
        units = new List<UnitController>();
        var unitCounter = new Dictionary<Player, int>();
        foreach (var player in map.players) {
            unitCounter.Add(player, 0);
        }
        foreach (var unit in map.units) {
            unitCounter[unit.Item3] += 1;
            Initializer.InitializeUnit(unitObjects[0], unit.Item1, unitTypesDict[unit.Item2], unit.Item3, unitCounter[unit.Item3], map, this);
        }

        // initializte other map objects
        foreach (var fortress in map.fortresses) {
            Initializer.InitializeMapObject(fortressObject, fortress.coords, transform);
        }
        foreach (var airport in map.airports) {
            Initializer.InitializeMapObject(airportObject, airport.coords, transform);
        }

        // initialize players
        currentPlayer = map.players[0];
        currentPlayerIndex = 0;
        turnCounter = 1;
        turnCounterField.text = turnCounter.ToString();

        // UI
        endTurnButton.onClick.AddListener(EndTurn);
    }    

    private GameObject tileTypeToGameObject(TileType tileType) {
        if (tileType == TileType.Plains) {
            return tileObjects[0];
        } else if (tileType == TileType.Forest) {
            return tileObjects[1];
        } else if (tileType == TileType.Hills) {
            return tileObjects[2];
        } else if (tileType == TileType.Swamp) {
            return tileObjects[3];
        } else if (tileType == TileType.Mountains) {
            return tileObjects[4];
        } else if (tileType == TileType.City) {
            return tileObjects[5];
        }
        return tileObjects[6];
    }

    // Update is called once per frame
    void Update()
    {
        inputHandler.Handle();        
    }

    public void EndTurn() {
        if (currentPlayer.isHuman) {
            NextPlayer();
        }
    }

    void NextPlayer() {
        if (currentPlayerIndex >= map.players.Count - 1) {
            turnCounter++;
            turnCounterField.text = turnCounter.ToString();
            currentPlayerIndex = -1;
        }
        currentPlayerIndex++;
        currentPlayer = map.players[currentPlayerIndex];
        NewTurnForUnits();

        if (!currentPlayer.isHuman) {
            AITurn();
            NextPlayer();
        }
    }

    void AITurn() {

    }

    void NewTurnForUnits() {
        foreach (var unit in units) {
            if (unit.player == currentPlayer) {
                unit.NewTurn();
            }
        }
    }        
}
