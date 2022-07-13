using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitStats
{
    public UnitController unitController;
    public UnitType unitType;
    public List<UnitModifier> generalModifiers;
    public List<UnitModifier> unitModifiers;
    public UnitModifier currentModifier;

    public float currentOrganization   = 100.0f;
    public float currentHealth         = 100.0f;
    public float currentMovement       = 2.0f;

    public float offense        = 1.0f;
    public float defense        = 1.0f;
    public float damage         = 1.0f;
    public float armour         = 0.0f;
    public float penetration    = 0.0f;
    public float organization   = 100.0f;
    public float recovery       = 1.0f;
    public float health         = 100.0f;
    public float movementSpeed  = 2.0f;
    public float support        = 0.1f;

    public bool blocked = false;
    public bool encircled = false;
    public int lineOfSight = 2;

    public float experience = 0.0f;
    public int level = 0;
    public float baseLevelUpCost = 1000.0f;
    public float levelUpCostModifier = 1.3f;

    public General general = null;
    
    public TileCombatModifier tileCombatModifier = new TileCombatModifier(true);

    public UnitStats(UnitType unitType, List<UnitModifier> generalModifiers, UnitController unitController) {
        this.unitController = unitController;
        this.unitType = unitType;
        offense = unitType.offense;
        defense = unitType.defense;
        damage = unitType.damage;
        armour = unitType.armour;
        penetration = unitType.penetration;
        organization = unitType.organization;
        health = unitType.health;
        movementSpeed = unitType.movementSpeed;        
        support = unitType.support;

        unitModifiers = new List<UnitModifier>();
        this.generalModifiers = generalModifiers;
        currentModifier = new UnitModifier();
        foreach (var generalModifier in generalModifiers) {
            AddModifier(generalModifier);
        }
    }

    public float GetOffense() {
        return currentModifier.offenseAdd + currentModifier.offenseMul * offense;
    }
    public float GetDefense() {
        return currentModifier.defenseAdd + currentModifier.defenseMul * defense;
    }
    public float GetDamage() {
        return currentModifier.damageAdd + currentModifier.damageMul * damage;
    }
    public float GetArmour() {
        return currentModifier.armourAdd + currentModifier.armourMul * armour;
    }
    public float GetPenetration() {
        return currentModifier.penetrationAdd + currentModifier.penetrationMul * penetration;
    }
    public float GetHealth() {
        return currentModifier.healthAdd + currentModifier.healthMul * health;
    }
    public float GetOrganization() {
        return currentModifier.organizationAdd + currentModifier.organizationMul * organization;
    }
    public float GetSupport() {
        return currentModifier.supportAdd + currentModifier.supportMul * support;
    }
    public float GetRecovery() {
        return currentModifier.recoveryAdd + currentModifier.recoveryMul * recovery;
    }

    public void AddModifier(UnitModifier unitModifier) {
        currentModifier.Add(unitModifier);
    }

    public void RemoveModifier(UnitModifier unitModifier) {
        currentModifier.Remove(unitModifier);
    }

    public void Reset() {
        currentHealth = GetHealth();
        currentOrganization = GetOrganization();
        currentMovement = movementSpeed;
    }

    public void NewTurn(bool isEncircled) {
        currentMovement = movementSpeed;
        if (!isEncircled) {
            currentOrganization = Mathf.Min(currentOrganization + 0.1f * GetRecovery() * GetOrganization(), GetOrganization());
        }
    }

    public bool TakeHealthDamage(float dealtDamage) {
        Debug.Log("unit: " + unitType.className + " has taken " + dealtDamage + " HP damage.");
        currentHealth -= dealtDamage;
        AddExperience(5.0f * dealtDamage);
        return currentHealth <= 0.0f;
    }

    public bool TakeOrganizationDamage(float dealtDamage) {
        Debug.Log("unit: " + unitType.className + " has taken " + dealtDamage + " ORG damage.");
        AddExperience(2.5f * dealtDamage);
        if (currentOrganization == 0.0f) {
            TakeHealthDamage(dealtDamage);
            return true;
        }
        currentOrganization = Mathf.Max(currentOrganization - dealtDamage, 0.0f);
        return currentOrganization == 0.0f;
    }

    public float LevelUpNeed() {
        return baseLevelUpCost * (Mathf.Pow(levelUpCostModifier, level + 1));
    }

    public void AddExperience(float newExperience) {
        experience += newExperience;
        while (Mathf.Log(experience / baseLevelUpCost, levelUpCostModifier) + 1.0f >= level) {
            LevelUp();
        }
    }

    public void LevelUp() {
        level++;
    }
    
}
