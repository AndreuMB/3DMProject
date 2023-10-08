using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RadialMenu", menuName = "RadialMenu/RadialMenu", order = 1)]
public class RadialMenuSO : ScriptableObject
{
    public RadialElementSO[] elements;
}
