using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst.Intrinsics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class RadialMenu : MonoBehaviour
{
    public RadialMenuSO data;
    public RadialMenu parent;
    public GameObject radialElementPrefab;
    [SerializeField] GameObject radialMenuPrefab;
    PlacementSystem ps;
    RadialElement clickedRE;
    Player player;


    void Start()
    {
        // BuildRM();
        ps = FindObjectOfType<PlacementSystem>();
        player = FindObjectOfType<Player>();
    }

    public void BuildRM(){

        foreach (Transform child in transform) {
            Destroy(child.gameObject);
        }

        float elementRadian = Mathf.PI * 2 / data.elements.Length;

        // need an index for use the arrange function
        for (int i = 0; i < data.elements.Length; i++)
        {
            GameObject elementGO = Instantiate(radialElementPrefab, transform);
            RadialElement radialElement = elementGO.GetComponent<RadialElement>();
            radialElement.SetData(data.elements[i]);
            // radialElement.SetCallback(TestDelegate);
            if (radialElement.parent)
            {
                elementGO.GetComponent<Button>().onClick.AddListener(() => NewRM(radialElement.parent));
            }else{
                elementGO.GetComponent<Button>().onClick.AddListener(() => FunctionInvoke(radialElement));
            }
            Arrange(elementGO,elementRadian,i);
        }
    }

    void Arrange(GameObject element, float elementRadian, float order){
        // size of the circle
        const float SEPARATION = 100f;
        // convert radian to coordinates, don't ask
        float x = Mathf.Cos(elementRadian * order) * SEPARATION;
        float y = Mathf.Sin(elementRadian * order) * SEPARATION;

        element.GetComponent<RectTransform>().anchoredPosition = new Vector3(x,y,0);
    }

    // void TestDelegate(RadialElement sRadialElement){
    //     print("test = " + sRadialElement.label.text);
    // }

    // void ButtonFunction(string functionName){
    //     // sRadialElement.
    //     switch (functionName)
    //     {
            
    //         default:
    //             print("default");
    //             break;
    //     }
    // }

    public void Build(){
        
    }

    private void FunctionInvoke(RadialElement radialElement){
        clickedRE = radialElement;
        Invoke(radialElement.customFunctionName,0);
        Destroy(gameObject);
    }

    public void BuildBuilding(){
        ps.Placement(5,clickedRE.buildingType);
    }

    public void DestroyBuilding(){
        ps.Remove();
    }

    public void NewRM(RadialMenuSO rmSO){
        // RectTransform canvasRect = FindObjectOfType<Canvas>().GetComponent<RectTransform>();

        // Vector3 canvasRectHalf = new Vector3(canvasRect.rect.width / 2, canvasRect.rect.height / 2);
        GameObject rmGO = Instantiate(radialMenuPrefab);
        rmGO.transform.SetParent(transform.parent,false);
        rmGO.GetComponent<RadialMenu>().data=rmSO;
        rmGO.GetComponent<RadialMenu>().BuildRM();
        rmGO.SetActive(true);
        rmGO.GetComponent<RectTransform>().anchoredPosition = GetComponent<RectTransform>().anchoredPosition;
        parent = rmGO.GetComponent<RadialMenu>();
        player.rmGO = rmGO;
        // gameObject.SetActive(!gameObject.activeInHierarchy);
        Destroy(gameObject);
    }


}
