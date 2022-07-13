using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Combat
{
    public static float disorgedDmgMultiplier = 1.0f;

    private class CombatStats {        
        public static float healthinessImportance = 0.5f;
        public static float cohesionImportance = 0.25f;
        public static float orgDmgMultiplier = 5.0f;
        public static float hpDmgMultiplier = 1.0f;

        public float efficiency;
        public float support;
        public float healthiness;
        public float cohesion;
        public float terrainBonus;
        public UnitStats unitStats;
        public float orgDamage;
        public float hpDamage;
        public float defenderTerrainBonus;
        public float defenseBonus;
        public float damageBonus;

        public CombatStats(UnitController unit, UnitController otherUnit, bool isAttacker) {
            efficiency = Efficiency(unit.unitStats.GetPenetration(), otherUnit.unitStats.GetArmour());            
            support = Support(unit, otherUnit, isAttacker);
            healthiness = Healthiness(unit.unitStats);
            cohesion = Cohesion(unit.unitStats);
            if (isAttacker) {
                terrainBonus = unit.unitStats.tileCombatModifier.modifiers[otherUnit.tile.tileType];
                defenderTerrainBonus = otherUnit.unitStats.tileCombatModifier.modifiers[otherUnit.tile.tileType];
            } else {                
                terrainBonus = unit.unitStats.tileCombatModifier.modifiers[unit.tile.tileType];
                defenderTerrainBonus = terrainBonus;
            }
            unitStats = unit.unitStats;  
            defenseBonus = 1.0f;
            damageBonus = 1.0f;
            if (!isAttacker) {
                if (unit.tile.isFortress) {
                    defenseBonus = 2.0f;
                    damageBonus = 2.0f;
                } else if (unit.tile.trenches.Count != 0) {
                    foreach (var trench in unit.tile.trenches) {
                        if (trench.Item2 == otherUnit.tile) {
                            defenseBonus = 1.5f;
                            damageBonus = 1.25f;
                        }
                    }
                }
            }                      
        }

        public void CalculateDamage(CombatStats other, float terrainBonusRatio) {
            orgDamage = OrganizationDamage(this, other, terrainBonusRatio, defenderTerrainBonus);
            hpDamage = HealthDamage(this, other, terrainBonusRatio, defenderTerrainBonus);
            Debug.Log("orgdamage:" + orgDamage + " hpdamage:" + hpDamage);
        }

        public float HealthDamage(CombatStats attacker, CombatStats defender, float terrainBonusRatio, float defenderTerrainBonus) {
        return (
            hpDmgMultiplier * ((attacker.efficiency * (1.0f + attacker.support) * healthinessImportance * (attacker.healthiness + 1.0f / healthinessImportance) 
            * cohesionImportance * (attacker.cohesion + 1.0f / cohesionImportance) * attacker.unitStats.GetDamage()) / 
            (defenderTerrainBonus * terrainBonusRatio)));
        }

        public float OrganizationDamage(CombatStats attacker, CombatStats defender, float terrainBonusRatio, float defenderTerrainBonus) {
            return (
                orgDmgMultiplier * attacker.efficiency * (1.0f + attacker.support) * healthinessImportance * (attacker.healthiness + 1.0f / healthinessImportance) 
                * cohesionImportance * (attacker.cohesion + 1.0f / cohesionImportance) * attacker.unitStats.GetOffense() * attacker.unitStats.GetDamage() / 
                (defender.unitStats.GetDefense() * defenderTerrainBonus * terrainBonusRatio));
        }

        public float Healthiness(UnitStats unit) {
            return unit.currentHealth / unit.GetHealth(); 
        }
        
        public float Cohesion(UnitStats unit) {
            return unit.currentOrganization / unit.GetOrganization(); 
        }

        public float Support(UnitController attacker, UnitController defender, bool attack) {
            var totalSupport = 0.0f;
            var unit = attacker;
            if (!attack) {
                unit = defender;
            }
            foreach (var tile in defender.tile.neighbours) {
                if (tile.unit != null) {
                    if (unit.player.IsAlliedTo(tile.unit.player)) {
                        totalSupport += tile.unit.unitStats.GetSupport();
                    }
                }
            }
            return totalSupport;
        } 

        public float Efficiency(float attackerPenetration, float defenderArmour) {
            if (defenderArmour == 0.0f) {
                return 1.0f;
            }
            var efficiency = attackerPenetration / defenderArmour;
            if (efficiency > 1.0f) {
                efficiency = 1.0f;
            } else if (efficiency < 0.8f) {
                efficiency = 0.5f;
            } else {
                efficiency = -1.5f + 2.5f * efficiency;
            }
            return efficiency;
        }
    }

    public static void CalculateFight(UnitController attacker, UnitController defender) {
        var attackerStats = new CombatStats(attacker, defender, true);
        var defenderStats = new CombatStats(defender, attacker, false);

        var terrainBonusRatio = attackerStats.terrainBonus / defenderStats.terrainBonus;
        Debug.Log("terrainBonusRatio = " + terrainBonusRatio);

        attackerStats.CalculateDamage(defenderStats, terrainBonusRatio);
        defenderStats.CalculateDamage(attackerStats, terrainBonusRatio);

        bool isAttackerRetreating = attacker.unitStats.TakeOrganizationDamage(defenderStats.orgDamage); 
        bool isAttackerDead = attacker.unitStats.TakeHealthDamage(defenderStats.hpDamage); 
        
        bool isDefenderRetreating = defender.unitStats.TakeOrganizationDamage(attackerStats.orgDamage) && defender.CanRetreat();
        bool isDefenderDead = defender.unitStats.TakeHealthDamage(attackerStats.hpDamage);     

        var defenderTile = defender.gameController.map.At(defender.tile.coords.x, defender.tile.coords.y);
        
        if (isDefenderDead) {
            defender.Delete();
        } else {
            if (isDefenderRetreating) {
                defender.Retreat();
            }
            defender.UpdateHealthbar();
            defender.UpdateOrgbar();
        }             
        
        if (isAttackerDead) {
            attacker.Delete();
        } else {
            if (isDefenderRetreating || isDefenderDead) {
                attacker.Advance(defenderTile);
            }
            attacker.UpdateHealthbar();
            attacker.UpdateOrgbar();  
        }     
    }    
}
