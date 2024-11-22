using System;
using System.Collections.Generic;
using Unity.VisualScripting;

public class ButtonData
{
    public string buttonName;
    public Action buttonAction;
    public List<GameMaterial> actionCost;
    private string v1;
    private object v2;
    private List<GameMaterial> addDronCost;

    public ButtonData(string buttonName, Action buttonAction, List<GameMaterial> actionCost = null)
    {
        this.buttonName = buttonName;
        this.buttonAction = buttonAction;
        this.actionCost = actionCost;
    }

    public ButtonData(string v1, object v2, List<GameMaterial> addDronCost)
    {
        this.v1 = v1;
        this.v2 = v2;
        this.addDronCost = addDronCost;
    }
}
