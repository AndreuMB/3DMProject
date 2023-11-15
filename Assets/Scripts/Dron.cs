using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Dron
{
    public string origin;
    public string destiny;
    public Resource resource;
    public Coroutine coroutine;
    
    public Dron(string origin, string destiny, Resource resource){
        this.origin = origin;
        this.destiny = destiny;
        this.resource = resource;
    }
}
