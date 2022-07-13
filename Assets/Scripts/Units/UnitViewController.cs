using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UnitViewController : MonoBehaviour
{
    public TextMeshProUGUI unitName;
    public TextMeshProUGUI unitNumber;
    public TextMeshProUGUI level;
    public TextMeshProUGUI offense;
    public TextMeshProUGUI defense;
    public TextMeshProUGUI damage;
    public TextMeshProUGUI armour;
    public TextMeshProUGUI penetration;
    public TextMeshProUGUI recovery;
    public TextMeshProUGUI support;
    public Image picture;
    public Image background;
    public Image nameBackground;
    public Image statsBackground;
    public GameObject healthBar;
    public GameObject orgBar;
    public GameObject expBar;

    public UnitController unit;

    public void Fill(UnitController newUnit) {
        if (newUnit == null) {
            return;
        }
        this.unit = newUnit;
        var unitStats = unit.unitStats;

        unitName.text = unitStats.unitType.className;
        unitNumber.text = unit.unitName.text;
        level.text = unitStats.level.ToString();
        offense.text = string.Format("{0:N1}", unitStats.GetOffense());
        defense.text = string.Format("{0:N1}", unitStats.GetDefense());
        damage.text = string.Format("{0:N1}", unitStats.GetDamage());
        armour.text = string.Format("{0:N1}", unitStats.GetArmour());
        penetration.text = string.Format("{0:N1}", unitStats.GetPenetration());
        recovery.text = string.Format("{0:N1}", unitStats.GetRecovery());
        support.text = string.Format("{0:N1}", unitStats.GetSupport());
        picture.sprite = unitStats.unitType.image;

        background.color = new Color(0.25f * unit.player.color.r, 0.25f * unit.player.color.g, 0.25f * unit.player.color.b);
        nameBackground.color = new Color(0.25f * unit.player.color.r, 0.25f * unit.player.color.g, 0.25f * unit.player.color.b);
        statsBackground.color = new Color(0.8f * unit.player.color.r, 0.8f * unit.player.color.g, 0.8f * unit.player.color.b);

        var locPos = healthBar.transform.localPosition;
        healthBar.transform.localScale = new Vector3(unitStats.currentHealth / unitStats.health, 1.0f, 1.0f);
        healthBar.transform.localPosition = new Vector3(-42.5f + 42.5f * (unitStats.currentHealth / unitStats.GetHealth()), locPos.y, locPos.z);
        
        locPos = orgBar.transform.localPosition;
        orgBar.transform.localScale = new Vector3(unitStats.currentOrganization / unitStats.organization, 1.0f, 1.0f);
        orgBar.transform.localPosition = new Vector3(-42.5f + 42.5f * (unitStats.currentOrganization / unitStats.GetOrganization()), locPos.y, locPos.z);
    
        locPos = expBar.transform.localPosition;
        expBar.transform.localScale = new Vector3(unitStats.experience / unitStats.LevelUpNeed(), 1.0f, 1.0f);
        expBar.transform.localPosition = new Vector3(-25.0f + 25.0f * (unitStats.experience / unitStats.LevelUpNeed()), locPos.y, locPos.z);
    
    }

    void Start() {

    }

    void Update() {
        Fill(unit);
    }

}
