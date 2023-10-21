using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;

[System.Serializable]
public class GameData
{
    public int drons;
    // public List<Extractor> extractors;
    public List<BuildingData> buildings;
    // public Building building;

    public GameData(Player player,List<BuildingData> buildings){
        drons = player.drons;
        // building = extractor.data;
        // this.extractors = extractors;
        this.buildings = buildings;
        // buildings.Add(extractors[0].data);
        // foreach (Extractor extractor in extractors)
        // {
        //     Debug.Log("extractor.name = " + extractor.name);
        //     Debug.Log("extractor.data = " + extractor.data);
        //     buildings.Add(extractor.data);
        // }
        // resources = player.resources;
    }
}
