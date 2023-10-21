using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Player : MonoBehaviour
{
    public int drons;
    GameObject optionsMenu;
    GameObject mainRadialMenu;
    // GameObject selectedGO;
    public UnityEvent<GameObject> selectedGO;

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

        GameObject sGO = GetClickedGO();
        if(sGO != null) selectedGO.Invoke(sGO);
    }

    GameObject GetClickedGO(){
        if(Input.GetMouseButtonDown(0)) // left click
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                return hit.transform.gameObject;
            }
        }
        return null;
    }

}

public enum ResourcesEnum
{
    R1,
    R2,
    R3,
    R4,
    R5,
}
