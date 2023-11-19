using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class RadialMenu : MonoBehaviour
{
    public RadialMenuSO data;
    public RadialMenu parent;
    public GameObject radialElementPrefab;


    void Start()
    {
        // Mathf.PI * 2 radians = 360º
        float elementRadian = Mathf.PI * 2 / data.elements.Length;

        // need an index for use the arrange function
        for (int i = 0; i < data.elements.Length; i++)
        {
            GameObject elementGO = Instantiate(radialElementPrefab, transform);
            RadialElement radialElement = elementGO.GetComponent<RadialElement>();
            radialElement.SetData(data.elements[i]);
            // radialElement.SetCallback(TestDelegate);
            elementGO.GetComponent<Button>().onClick.AddListener(() => Invoke(radialElement.customFunctionName,0));

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
        print("Build");
    }

    public void DestroyBuilding(){
        print("DestroyBuilding");
    }


}
