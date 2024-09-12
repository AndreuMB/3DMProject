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
    public RadialMenuSO parent;
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

    public void BuildRM()
    {

        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        // create backBtn if not exist
        RadialElementSO backRESO = FindObjectOfType<RMManager>().backRESO;
        bool existBBtn = data.elements.Find(x => x.elementName == backRESO.elementName);

        if (parent || parent != null)
        {
            backRESO.parent = parent;
            if (!existBBtn) data.elements.Add(backRESO);
        }

        float elementRadian = Mathf.PI * 2 / data.elements.Count;

        // need an index for use the arrange function
        for (int i = 0; i < data.elements.Count; i++)
        {
            GameObject elementGO = Instantiate(radialElementPrefab, transform);
            RadialElement radialElement = elementGO.GetComponent<RadialElement>();
            radialElement.SetData(data.elements[i]);
            // radialElement.SetCallback(TestDelegate);
            if (radialElement.parent)
            {
                RadialElementSO dataRE = data.elements[i];
                elementGO.GetComponent<Button>().onClick.AddListener(() => NewRM(radialElement.parent, dataRE));
            }
            else
            {
                elementGO.GetComponent<Button>().onClick.AddListener(() => FunctionInvoke(radialElement));
            }
            Arrange(elementGO, elementRadian, i);
        }
    }

    void Arrange(GameObject element, float elementRadian, float order)
    {
        // size of the circle
        const float SEPARATION = 100f;
        // convert radian to coordinates, don't ask
        float x = Mathf.Cos(elementRadian * order) * SEPARATION;
        float y = Mathf.Sin(elementRadian * order) * SEPARATION;

        element.GetComponent<RectTransform>().anchoredPosition = new Vector3(x, y, 0);
    }

    private void FunctionInvoke(RadialElement radialElement)
    {
        clickedRE = radialElement;
        Invoke(radialElement.customFunctionName.ToString(), 0);
        Destroy(gameObject);
    }

    public void BuildBuilding()
    {
        ps.Placement(clickedRE.buildingType);
    }

    public void DestroyBuilding()
    {
        ps.Remove();
    }

    public void NewRM(RadialMenuSO rmSO, RadialElementSO reSO)
    {
        // RectTransform canvasRect = FindObjectOfType<Canvas>().GetComponent<RectTransform>();

        // Vector3 canvasRectHalf = new Vector3(canvasRect.rect.width / 2, canvasRect.rect.height / 2);
        RadialMenu newRM = Instantiate(radialMenuPrefab).GetComponent<RadialMenu>();
        newRM.transform.SetParent(transform.parent, false);
        newRM.data = rmSO;

        const string BACK_RE = "Back";
        newRM.parent = reSO.elementName != BACK_RE ? data : null;

        newRM.BuildRM();

        newRM.GetComponent<RectTransform>().anchoredPosition = GetComponent<RectTransform>().anchoredPosition;
        newRM.gameObject.SetActive(true);
        // parent = rmGO.GetComponent<RadialMenu>();
        player.rmGO = newRM.gameObject;
        // gameObject.SetActive(!gameObject.activeInHierarchy);
        Destroy(gameObject);
    }


}
