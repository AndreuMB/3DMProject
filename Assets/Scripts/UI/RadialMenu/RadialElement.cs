using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Windows;
using Unity.VisualScripting;
using UnityEngine.Events;


public class RadialElement : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public delegate void RadialElementDelegate(RadialElement sElement);
    public TMP_Text label;
    [SerializeField] Image background;
    float shadow = 0.1f;
    RadialElementDelegate callback;
    Color bgColor;
    public RadialElementFunction customFunctionName;
    public int buildId;
    public BuildingsEnum buildingType;
    public RadialMenuSO parent;

    void Awake()
    {
        bgColor = background.color;
    }

    void OnEnable()
    {
        background.color = bgColor;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Color hoverColor = new Color(background.color.r - shadow, background.color.g - shadow, background.color.b - shadow);
        background.color = hoverColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        background.color = bgColor;
    }

    public void SetData(RadialElementSO data)
    {
        label.text = data.elementName;
        // callback = data.callback;
        customFunctionName = data.customFunctionName;
        // buildId = data.buildId;
        buildingType = data.buildingType;
        parent = data.parent;
    }
}
