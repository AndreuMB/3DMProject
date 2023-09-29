using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public int drons;
    public List<Resource> resources;
    GameObject optionsMenu;

    void Start(){
        optionsMenu = FindAnyObjectByType<Options>().gameObject;
        optionsMenu.SetActive(false);
    }

    void Update(){
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            optionsMenu.SetActive(!optionsMenu.activeInHierarchy);
        }
    }

}
