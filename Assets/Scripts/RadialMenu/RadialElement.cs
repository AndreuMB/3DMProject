using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Windows;
using Unity.VisualScripting;


public class RadialElement : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public delegate void RadialElementDelegate(RadialElement sElement);
    [SerializeField] public TMP_Text label;
    [SerializeField] Image background;
    float shadow = 0.1f;
    RadialElementDelegate callback;
    Color bgColor;

    void Awake(){
        bgColor = background.color;
    }

    void OnEnable(){
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

    public void SetData(RadialElementSO data){
        label.text = data.elementName;
        callback = data.callback;
    }

    public void setCallback(RadialElementDelegate sCallback){
        callback = sCallback;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (callback != null) callback.Invoke(this);
    }

    public void Build(RadialElement sRadialElement){
        print("Build");
    }

    public void DestroyBuilding(RadialElement sRadialElement){
        print("DestroyBuilding");
    }
}
