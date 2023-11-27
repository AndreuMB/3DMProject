using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RMManager : MonoBehaviour
{
    public Canvas RMCanvas;
    public List<RadialMenuSO> RMSOs; 
    [SerializeField] GameObject rmPrefab;
    [SerializeField] GameObject rePrefab;
    
    public GameObject NewRM(RadialMenuSO rmSO, Vector2 mcPosition){
        // RectTransform canvasRect = FindObjectOfType<Canvas>().GetComponent<RectTransform>();

        // Vector3 canvasRectHalf = new Vector3(canvasRect.rect.width / 2, canvasRect.rect.height / 2);
        GameObject rmGO = Instantiate(rmPrefab);
        rmGO.transform.SetParent(RMCanvas.transform,false);
        rmGO.GetComponent<RectTransform>().anchoredPosition = mcPosition;
        rmGO.GetComponent<RadialMenu>().data=rmSO;
        rmGO.GetComponent<RadialMenu>().BuildRM();
        return rmGO;
        // rmGO.SetActive(true);
        // rmGO.GetComponent<RectTransform>().anchoredPosition = GetComponent<RectTransform>().anchoredPosition;
        // parent = rmGO.GetComponent<RadialMenu>();
        // gameObject.SetActive(!gameObject.activeInHierarchy);
    }
}
