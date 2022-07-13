using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileCombatModifier
{
    public Dictionary<TileType, float> modifiers;
    public TileCombatModifier(bool basic) {
        if (basic) {
            modifiers = new Dictionary<TileType, float>() {
                {   TileType.Plains,       1.0f    },
                {   TileType.Forest,       1.3f    },
                {   TileType.Hills,        1.5f    },
                {   TileType.Mountains,    2.0f    },
                {   TileType.Swamp,        1.5f    },
                {   TileType.Water,        1.0f    },
                {   TileType.City,         1.8f    }
            };
        } else {
            modifiers = new Dictionary<TileType, float>() {
                {   TileType.Plains,       0.0f    },
                {   TileType.Forest,       0.0f    },
                {   TileType.Hills,        0.0f    },
                {   TileType.Mountains,    0.0f    },
                {   TileType.Swamp,        0.0f    },
                {   TileType.Water,        0.0f    },
                {   TileType.City,         0.0f    }
            };
        }
    }

    public void Add(TileCombatModifier tileCombatModifier) {
        foreach (var kvp in tileCombatModifier.modifiers) {
            modifiers[kvp.Key] += kvp.Value;
        }
    }

    public void Remove(TileCombatModifier tileCombatModifier) {
        foreach (var kvp in tileCombatModifier.modifiers) {
            modifiers[kvp.Key] -= kvp.Value;
        }
    }
}
