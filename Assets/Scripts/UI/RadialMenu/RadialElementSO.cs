using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "RadialElement", menuName = "RadialMenu/RadialElement", order = 2)]
public class RadialElementSO : ScriptableObject
{
    public string elementName;
    // public Sprite icon;
    // public RadialMenuSO subMenu;
    // public RadialElement.RadialElementDelegate callback;
    // public UnityEvent customFunction;
    public string customFunctionName;
    public int buildId;
    public BuildingsEnum buildingType;
    public RadialMenuSO parent;

}
