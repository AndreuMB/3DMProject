using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public int drons;
    public List<Resource> resources;
    GameObject optionsMenu;
    GameObject mainRadialMenu;

    void Start(){
        optionsMenu = FindAnyObjectByType<Options>().gameObject;
        optionsMenu.SetActive(false);

        mainRadialMenu = FindAnyObjectByType<RadialMenu>().gameObject;
        mainRadialMenu.SetActive(false);
    }

    void Update(){
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            optionsMenu.SetActive(!optionsMenu.activeInHierarchy);
        }

        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            mainRadialMenu.SetActive(!mainRadialMenu.activeInHierarchy);
            RectTransform canvasRect = FindObjectOfType<Canvas>().GetComponent<RectTransform>();

            Vector3 canvasRectHalf = new Vector3(canvasRect.rect.width / 2, canvasRect.rect.height / 2);
            mainRadialMenu.GetComponent<RectTransform>().anchoredPosition = Input.mousePosition - canvasRectHalf;
        }
    }

}
