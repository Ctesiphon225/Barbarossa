using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class General
{
    public string name = "General";
    public Sprite sprite = null;
    
    public float offense = 0.0f;
    public float defense = 0.0f;
    public float leadership = 0.0f;
    public float logistics = 0.0f;
    public TileCombatModifier tileCombatModifier = new TileCombatModifier(false);  
    public float[] skillProbabilities = { 0.25f, 0.25f, 0.25f, 0.25f };

    public float experience = 0.0f;
    public int level = 1;  

    public float baseLevelUpCost = 1000.0f;
    public float levelUpCostModifier = 1.3f;

    public General() {
        
    }

    public void AddExperience(float newExperience) {
        experience += newExperience;
        while (Mathf.Log(experience / baseLevelUpCost, levelUpCostModifier) + 1.0f >= level) {
            LevelUp();
        }
    }

    public void LevelUp() {
        level++;
        for (int i = 0; i < 3; i++) {
            var value = Random.value;
            if (value <= skillProbabilities[0]) {
                offense++;
            } else if (value <= skillProbabilities[1]) {
                defense++;
            } else if (value <= skillProbabilities[2]) {
                leadership++;
            } else {
                logistics++;
            }
        }
    }
}
