using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Resource
{
    public string name;
    public string quantity;
    [System.NonSerialized]
    public GameObject HUDGO;
}
