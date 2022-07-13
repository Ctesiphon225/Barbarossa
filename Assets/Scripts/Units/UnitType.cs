using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TerrainStat 
{
    public float terrainOffense = 1.0f;
    public float terrainDefense = 1.0f;
    public float terrainAttrition = 0.0f;

    public TerrainStat(float terrainOffense, float terrainDefense, float terrainAttrition) {
        this.terrainOffense = terrainOffense;
        this.terrainDefense = terrainDefense;
        this.terrainAttrition = terrainAttrition;
    }

    public TerrainStat() { }
}

public enum UnitClass {
    Infantry,
    Cavalry,
    Motorized,
    Mechanized,
    Armour,
    Artillery,
    General
}

public enum Country {
    Germany,
    Italy,
    Japan,
    Slovakia,
    Hungary,
    Romania,
    Finland,
    Bulgaria,
    Poland,
    Czechoslovakia,
    Yugoslavia,
    Greece,
    China,
    Norway,
    Denmark,
    Sweden,
    Soviet_Union,
    USA,
    France,
    Great_Britain,
    Canada,
    South_Africa,
    New_Zealand,
    Australia,
    British_Raj
}

[CreateAssetMenu(fileName = "Unit", menuName = "Scriptable Objects/UnitType", order = 1)]
public class UnitType : ScriptableObject
{
    public string className     = "empty";
    public UnitClass unitClass  = UnitClass.Infantry;

    // combat stats
    public float offense        = 1.0f;
    public float defense        = 1.0f;
    public float damage         = 1.0f;
    public float armour         = 0.0f;
    public float penetration    = 0.0f;
    public float organization   = 100.0f;
    public float recovery       = 1.0f;
    public float health         = 100.0f;
    public float support        = 0.1f;

    public float movementSpeed  = 2.0f;
    public int lineOfSight      = 2;

    public Sprite image;
    public Sprite icon;

    public Country country;

    // terrain stats
    public Dictionary<string, TerrainStat> terrainStats = new Dictionary<string, TerrainStat>{
        {"plains", new TerrainStat()},
        {"forest", new TerrainStat()},
        {"swamp", new TerrainStat()},
        {"hills", new TerrainStat()},
        {"mountains", new TerrainStat()},
        {"water", new TerrainStat()},
        {"city", new TerrainStat()}
    };
}
