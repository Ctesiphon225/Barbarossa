using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player
{
    public Color color = Color.red;
    public string name = "player";
    public List<UnitModifier> generalModifiers;
    private List<Player> allies;
    public bool isHuman = false;
    public string team = "";

    public Player(Color color, string name, bool isHuman, string team) {
        this.color = color;
        this.name = name;
        generalModifiers = new List<UnitModifier>();
        allies = new List<Player>();
        this.isHuman = isHuman;
        this.team = team;
    }

    public bool IsAlliedTo(Player other) {
        return team == other.team || allies.Contains(other);
    }
}
