using System;
using System.Collections.Generic;

public class ButtonData
{
    public string buttonName;
    public Action buttonAction;

    public ButtonData(string buttonName, Action buttonAction)
    {
        this.buttonName = buttonName;
        this.buttonAction = buttonAction;
    }
}
