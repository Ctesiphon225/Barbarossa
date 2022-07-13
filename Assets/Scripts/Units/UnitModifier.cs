using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitModifier
{
    public string modifierName = "general modifier";
    public Sprite modifierSprite = null;

    public float offenseAdd        = 0.0f;
    public float defenseAdd        = 0.0f;
    public float damageAdd         = 0.0f;
    public float armourAdd         = 0.0f;
    public float penetrationAdd    = 0.0f;
    public float organizationAdd   = 0.0f;
    public float recoveryAdd       = 0.0f;
    public float healthAdd         = 0.0f;
    public float supportAdd        = 0.0f;
    
    public float offenseMul        = 1.0f;
    public float defenseMul        = 1.0f;
    public float damageMul         = 1.0f;
    public float armourMul         = 1.0f;
    public float penetrationMul    = 1.0f;
    public float organizationMul   = 1.0f;
    public float recoveryMul       = 1.0f;
    public float healthMul         = 1.0f;
    public float supportMul        = 1.0f;

    // movement stats
    public float movementBonus     = 0.0f;

    // tile combat modifiers
    public TileCombatModifier tileCombatModifier = new TileCombatModifier(false);

    public UnitModifier() {}

    public void Add(UnitModifier unitModifier) {
        offenseAdd += unitModifier.offenseAdd;
        defenseAdd += unitModifier.defenseAdd;
        damageAdd += unitModifier.damageAdd;
        armourAdd += unitModifier.armourAdd;
        penetrationAdd += unitModifier.penetrationAdd;
        organizationAdd += unitModifier.organizationAdd;
        healthAdd += unitModifier.healthAdd;
        supportAdd += unitModifier.supportAdd;
        recoveryAdd += unitModifier.recoveryAdd;
        
        offenseMul += unitModifier.offenseMul;
        defenseMul += unitModifier.defenseMul;
        damageMul += unitModifier.damageMul;
        armourMul += unitModifier.armourMul;
        penetrationMul += unitModifier.penetrationMul;
        organizationMul += unitModifier.organizationMul;
        healthMul += unitModifier.healthMul;
        supportMul += unitModifier.supportMul;
        recoveryMul += unitModifier.recoveryMul;

        tileCombatModifier.Add(unitModifier.tileCombatModifier);
    }

    public void Remove(UnitModifier unitModifier) {
        offenseAdd -= unitModifier.offenseAdd;
        defenseAdd -= unitModifier.defenseAdd;
        damageAdd -= unitModifier.damageAdd;
        armourAdd -= unitModifier.armourAdd;
        penetrationAdd -= unitModifier.penetrationAdd;
        organizationAdd -= unitModifier.organizationAdd;
        healthAdd -= unitModifier.healthAdd;
        supportAdd -= unitModifier.supportAdd;
        recoveryAdd -= unitModifier.recoveryAdd;
        
        offenseMul -= unitModifier.offenseMul;
        defenseMul -= unitModifier.defenseMul;
        damageMul -= unitModifier.damageMul;
        armourMul -= unitModifier.armourMul;
        penetrationMul -= unitModifier.penetrationMul;
        organizationMul -= unitModifier.organizationMul;
        healthMul -= unitModifier.healthMul;
        supportMul -= unitModifier.supportMul;
        recoveryMul -= unitModifier.recoveryMul;

        tileCombatModifier.Remove(unitModifier.tileCombatModifier);
    }
}
