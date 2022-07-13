using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIHandler
{
    public GameController game;

    public UIHandler(GameController gameController) {
        game = gameController;
    }

    public void MouseOnUnit(UnitController unit) {        
        if (unit != game.selectedUnit) {
            game.mouseOnUnitScreen.SetActive(true);
            FillUnitScreen(unit, game.mouseOnUnitScreen);
        } 
    }

    public void DisableMouseOnUnit() {
        game.mouseOnUnitScreen.SetActive(false);
    }

    public void FillUnitScreen(UnitController unit, GameObject screen) {
        screen.GetComponent<UnitViewController>().Fill(unit);
    }
}
