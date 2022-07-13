using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TileType {
    Plains,
    Forest,
    Hills,
    Swamp,
    Mountains,
    City,
    Water
}

public class Tile {
    public TileType tileType = TileType.Plains;
    public string text = "";
    public Vector2Int coords = new Vector2Int(0, 0);
    public List<Tile> neighbours;
    public GameObject tileObject;
    // public GameObject unit = null;
    public UnitController unit = null;
    public bool isFortress = false;
    public List<(Tile, Tile)> trenches;

    public Tile(TileType tileType, string text, Vector2Int coords) {
        this.tileType = tileType;
        this.text = text;
        this.coords = coords;
        this.neighbours = new List<Tile>();
        this.trenches = new List<(Tile, Tile)>();
    }

    public Tile() {}
}

public class Map
{
    public List<List<Tile>> tiles;
    public List<(Tile, Tile)> smallRivers;
    public List<(Tile, Tile)> largeRivers;
    public List<(Tile, Tile)> railroads;
    public List<(Tile, Tile)> trenches;
    public List<Tile> fortresses;
    public List<Tile> airports;

    public List<Player> players;
    public List<(Vector2Int, string, Player)> units;
    public GameController gameController;

    private static List<Vector2Int> neighbourhoodOdd = new List<Vector2Int>() {
        new Vector2Int(0, -1),
        new Vector2Int(-1, 1),
        new Vector2Int(-1, 0),
        new Vector2Int(1, 0),
        new Vector2Int(0, 1),
        new Vector2Int(1, 1)
    };
    private static List<Vector2Int> neighbourhoodEven = new List<Vector2Int>() {
        new Vector2Int(-1, -1),
        new Vector2Int(-1, 0),
        new Vector2Int(0, -1),
        new Vector2Int(0, 1),
        new Vector2Int(1, -1),
        new Vector2Int(1, 0)
    };
    private List<Vector2Int> getNeighbourhood(int x) {
        if (x % 2 == 0) {
            return neighbourhoodEven;
        }
        return neighbourhoodOdd;
    }

    private static Dictionary<TileType, int> tileCosts = new Dictionary<TileType, int>() {
        {TileType.Plains, 1},
        {TileType.Forest, 2},
        {TileType.Hills, 2},
        {TileType.Mountains, 2},
        {TileType.Swamp, 2},
        {TileType.Water, 4},
        {TileType.City, 1}
    };

    public Map(string mapTxt, GameController gameController) {        
        this.gameController = gameController;
        tiles = new List<List<Tile>>();
        smallRivers = new List<(Tile, Tile)>();
        largeRivers = new List<(Tile, Tile)>();
        railroads = new List<(Tile, Tile)>();
        players = new List<Player>();
        units = new List<(Vector2Int, string, Player)>();
        trenches = new List<(Tile, Tile)>();
        fortresses = new List<Tile>();
        airports = new List<Tile>();

        using (var reader = new System.IO.StringReader(mapTxt)) {            
            string line;
            int currentLine = 0;

            bool finishedTiles = false;
            bool finishedRivers = false;
            bool finishedRailroads = false;
            bool finishedPlayers = false;
            bool finishedUnits = false;
            bool finishedTrenches = false;
            bool finishedFortresses = false;
            bool finishedAirports = false;

            for (currentLine = 0; (line = reader.ReadLine()?.Trim()) != null; ++currentLine)
            {
                if (line.Length == 0) { continue; } // Skip empty lines since they are allowed in definition

                if (line == "Rivers") {
                    finishedTiles = true;
                } else if (line == "Railroads") {
                    finishedRivers = true;
                } else if (line == "Players") {
                    finishedRailroads = true;
                } else if (line == "Units") {
                    finishedPlayers = true;
                } else if (line == "Trenches") {
                    finishedUnits = true;
                } else if (line == "Fortresses") {
                    finishedTrenches = true;
                } else if (line == "Airports") {
                    finishedFortresses = true;
                } else {
                    if (!finishedTiles) {
                        ReadRowOfTiles(line, currentLine);
                    } else if (!finishedRivers) {
                        ReadRiver(line);
                    } else if (!finishedRailroads) {
                        ReadRailroad(line);
                    } else if (!finishedPlayers) {
                        ReadPlayer(line);
                    } else if (!finishedUnits) {
                        ReadUnit(line);
                    } else if (!finishedTrenches) {
                        ReadTrench(line);
                    } else if (!finishedFortresses) {
                        ReadFortress(line);
                    } else if (!finishedAirports) {
                        ReadAirport(line);
                    } 
                }
            }            
        }

        AddNeighbours();
    }

    public Tile At(int x, int y) {
        return tiles[y][x];
    }

    private void ReadRowOfTiles(string line, int currentLine) {
        var newRow = new List<Tile>();
        int currentCol = 0;
        foreach (var tile in line.Split(',')) {                    
            string tileType = "";
            string text = "";                    
            foreach (var part in tile.Split(':')) {
                if (tileType == "") {
                    tileType = part;
                } else {
                    text = part.Substring(1, part.Length - 2);
                }
            }                    
            newRow.Add(new Tile(stringToTileType(tileType), text, new Vector2Int(currentCol, currentLine)));
            currentCol++;
        }
        tiles.Add(newRow);
    }

    private void ReadRiver(string line) {
        var parts = line.Split(' ');
        var first = parts[0].Split(',');
        var second = parts[1].Split(',');
        var riverType = parts[2];

        if (riverType == "small") {
            smallRivers.Add((At(int.Parse(first[0]), int.Parse(first[1])), At(int.Parse(second[0]), int.Parse(second[1]))));
        } else {
            largeRivers.Add((At(int.Parse(first[0]), int.Parse(first[1])), At(int.Parse(second[0]), int.Parse(second[1]))));
        }
    }
    
    private void ReadRailroad(string line) {
        var parts = line.Split(' ');
        var first = parts[0].Split(',');
        var second = parts[1].Split(',');

        railroads.Add((At(int.Parse(first[0]), int.Parse(first[1])), At(int.Parse(second[0]), int.Parse(second[1]))));        
    }
    
    private void ReadPlayer(string line) {
        var parts = line.Split(' ');
        players.Add(new Player(new Color(float.Parse(parts[1]), float.Parse(parts[2]), float.Parse(parts[3]), 1.0f), parts[0], parts[4] == "human", parts[5]));
    }
    
    private void ReadUnit(string line) {
        var parts = line.Split(' ');
        var coords = parts[0].Split(',');
        units.Add((new Vector2Int(int.Parse(coords[0]), int.Parse(coords[1])), parts[1], players[int.Parse(parts[2])]));
    }

    private void ReadTrench(string line) {
        var parts = line.Split(' ');
        var first = parts[0].Split(',');
        var second = parts[1].Split(',');

        trenches.Add((At(int.Parse(first[0]), int.Parse(first[1])), At(int.Parse(second[0]), int.Parse(second[1])))); 
        At(int.Parse(first[0]), int.Parse(first[1])).trenches.Add(trenches[trenches.Count - 1]);
    }

    private void ReadFortress(string line) {
        var parts = line.Split(' ');
        var coords = parts[0].Split(',');
        At(int.Parse(coords[0]), int.Parse(coords[1])).isFortress = true;
        fortresses.Add(At(int.Parse(coords[0]), int.Parse(coords[1])));
    }

    private void ReadAirport(string line) {
        var parts = line.Split(' ');
        var coords = parts[0].Split(',');
        airports.Add(At(int.Parse(coords[0]), int.Parse(coords[1])));
    }

    private void AddNeighbours() {
        for (int i = 0; i < tiles.Count; i++) {
            for (int j = 0; j < tiles[0].Count; j++) {
                foreach (var neighbour in getNeighbourhood(i)) {
                    var coords = new Vector2Int(i + neighbour.x, j + neighbour.y);
                    if (coords.x < 0 || coords.y < 0 || coords.x >= tiles.Count || coords.y >= tiles[0].Count) {
                        continue;
                    }
                    tiles[i][j].neighbours.Add(tiles[coords.x][coords.y]);
                }
            }
        }
    }

    private TileType stringToTileType(string text) {
        TileType result = TileType.Plains;
        if (text == "forest") {
            result = TileType.Forest;
        } else if (text == "hills") {
            result = TileType.Hills;
        } else if (text == "swamp") {
            result = TileType.Swamp;
        } else if (text == "mountains") {
            result = TileType.Mountains;
        } else if (text == "city") {
            result = TileType.City;
        } else if (text == "water") {
            result = TileType.Water;
        }
        return result;
    }

    public List<Tile> DFS(int limit, Tile initialNode) {
        var openNodes = new List<Tile>();
        DFS(limit, initialNode, openNodes);
        return openNodes;
    }

    private void DFS(int limit, Tile node, List<Tile> openNodes) {
        foreach (var neighbour in node.neighbours) {
            if (openNodes.Contains(neighbour)) {
                continue;
            }
            int distance = tileCosts[neighbour.tileType];
            if (distance <= limit) {
                openNodes.Add(neighbour);
                DFS(limit - distance, neighbour, openNodes);
            }
        }
    }

    public float Distance(Tile first, Tile second) {
        if (railroads.Contains((first, second)) || railroads.Contains((second, first))) {
            return 0.5f;
        } else if (smallRivers.Contains((first, second)) || smallRivers.Contains((second, first))) {
            return 2.0f;
        } else if (largeRivers.Contains((first, second)) || largeRivers.Contains((second, first))) {
            return 4.0f;
        }
        return tileCosts[second.tileType];
    }

    public Tile Dequeue(List<Tile> openNodes, Dictionary<Tile, float> distances) {
        var minDistance = float.MaxValue;
        Tile result = null;
        foreach (var node in openNodes) {
            if (distances[node] < minDistance) {
                result = node;
                minDistance = distances[node];
            }
        }
        openNodes.Remove(result);
        return result;
    }

    public List<Tile> BFS(float limit, Tile initialNode) {
        var openNodes = new List<Tile>();
        openNodes.Add(initialNode);

        var closedNodes = new List<Tile>();
        var distances = new Dictionary<Tile, float>();
        distances.Add(initialNode, 0);
        
        while(openNodes.Count > 0) {
            var currentNode = Dequeue(openNodes, distances);

            foreach(var node in currentNode.neighbours) {
                // stop when you reach an enemy unit
                if (currentNode.unit != null && currentNode != initialNode) {
                    break;
                }
                // dont use tiles occupied by allies
                if (initialNode.unit == null) {
                    Debug.Log("WARNING");
                }
                if (node.unit != null && node.unit.player.IsAlliedTo(initialNode.unit.player)) {
                    continue;
                }
                // skip node if in open/closed lists
                if (openNodes.Contains(node) || closedNodes.Contains(node)) {
                    continue;
                }
                // if undiscovered, add distance
                if (!distances.ContainsKey(node)) {
                    distances.Add(node, distances[currentNode] + Distance(currentNode, node));
                }
                // relax distance if necessary
                if (distances[currentNode] + Distance(currentNode, node) < distances[node]) {
                    distances[node] = distances[currentNode] + Distance(currentNode, node);
                }
                // add node only if not too far and add friendly units only on neighbouring tiles
                if (distances[node] <= limit && ((node.unit == null || 
                                                 initialNode.neighbours.Contains(node) || 
                                                 node.unit.player != initialNode.unit.player))) {
                    openNodes.Add(node);
                }
            }
            if (currentNode != initialNode) {
                closedNodes.Add(currentNode);
            }
        }
        return closedNodes;
    }

    float EuclideanDistance(Tile a, Tile b) {
        return Mathf.Sqrt(Mathf.Pow(a.coords.x - b.coords.x, 2) + Mathf.Pow(a.coords.y - b.coords.y, 2));
    }

    private Tile Dequeue(List<Tile> openNodes, List<float> estimatedDistances, Dictionary<Tile, int> nodeIndices) {
        float min = float.MaxValue;
        var result = openNodes[0];
        foreach (var node in openNodes) {
            if (estimatedDistances[nodeIndices[node]] < min) {
                min = estimatedDistances[nodeIndices[node]];
                result = node;
            }
        }
        openNodes.Remove(result);
        return result;
    }

    public Queue<Tile> FindPath(Tile initialNode, Tile goal) {
        if (initialNode == goal) {
            return new Queue<Tile>();
        }

        var nodes = new List<Tile>();
        foreach (var row in tiles) { 
            foreach (var tile in row) { 
                nodes.Add(tile); 
            } 
        }

        var distances = new List<float>();
        var estimatedDistances = new List<float>();
        var nodeIndices = new Dictionary<Tile, int>();
        for (int i = 0; i < nodes.Count; i++) { 
            if (nodes[i] == initialNode) {
                distances.Add(0.0f);
                estimatedDistances.Add(0.0f);
            } else {
                distances.Add(float.MaxValue);
                estimatedDistances.Add(float.MaxValue); 
            }
            nodeIndices.Add(nodes[i], i);            
        }

        var parents = new List<Tile>();
        for (int i = 0; i < nodes.Count; i++) { parents.Add(null); }

        var openNodes = new List<Tile>();
        var closedNodes = new List<Tile>();
        var currentNode = initialNode;

        int iterationLimit = 0;
        while (currentNode != goal) {
            iterationLimit++;
            if (iterationLimit >= 1000) { 
                Debug.Log(">>> SAFETY CUTOFF <<<");
                return new Queue<Tile>();
            }

            closedNodes.Add(currentNode);

            foreach (var node in currentNode.neighbours) {
                if (closedNodes.Contains(node) || (node.unit != null && node != goal)) {
                    continue;
                }
                float distance = distances[nodeIndices[currentNode]] + Distance(currentNode, node);
                if (distance < distances[nodeIndices[node]]) {
                    distances[nodeIndices[node]] = distance;
                }

                float estimatedDistance = distance + EuclideanDistance(node, goal);
                if (estimatedDistance < estimatedDistances[nodeIndices[node]]) {
                    estimatedDistances[nodeIndices[node]] = estimatedDistance;
                    openNodes.Add(node);
                    parents[nodeIndices[node]] = currentNode;
                }
            }
            float min = float.MaxValue;
            foreach (var node in openNodes) {
                if (estimatedDistances[nodeIndices[node]] < min) {
                    min = estimatedDistances[nodeIndices[node]];
                    currentNode = node;
                }
            }
            openNodes.Remove(currentNode);
        }

        var path = new Queue<Tile>();
        while (currentNode != initialNode) {
            path.Enqueue(currentNode);
            currentNode = parents[nodeIndices[currentNode]];
        }
        return path;
    }
}
