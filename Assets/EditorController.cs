using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EditorController : MonoBehaviour
{
    public List<Button> buttons;
    
    public List<GameObject> windows;

    public Map map;
    public List<GameObject> tileObjects;
    public List<GameObject> unitObjects;
    public List<GameObject> riverObjects;
    public List<GameObject> railroadObjects;
    public GameObject fortressObject;
    public GameObject trenchObject;
    public GameObject airportObject;
    public List<Player> players;
    public List<UnitType> unitTypes;
    public Dictionary<string, UnitType> unitTypesDict;    
    public List<UnitController> units;


    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < buttons.Count; i++) {     
            int j = i; 
            buttons[i].onClick.AddListener(delegate{ OpenWindow(j); });
        }
        CloseWindows();
    }

    void OpenWindow(int idx) {
        CloseWindows();
        windows[idx].SetActive(true);
    }

    void CloseWindows() {
        foreach (var window in windows) {
            window.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            CloseWindows();
        }
    }

    public void LoadMap(string mapTxt) {
        map = new Map(mapTxt, null);

        // initialize tiles
        for (int i = 0; i < map.tiles.Count; i++) {
            for (int j = 0; j < map.tiles[i].Count; j++) {
                Initializer.InitializeTile(i, j, map, tileObjects[0], transform);
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
    }
}
