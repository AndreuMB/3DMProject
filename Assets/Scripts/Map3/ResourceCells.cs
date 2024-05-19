using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// [ExecuteInEditMode]
public class ResourceCells : MonoBehaviour
{
    public List<ResourceCell> resourceCells;
    [SerializeField] GameObject markPrefab;
    [SerializeField] Image resourceSprite;
    int lastLength;
    // Start is called before the first frame update
    void Start()
    {
        lastLength = resourceCells.Count;
        // foreach (ResourceCell rc in resourceCells)
        // {
        //     Instantiate(markPrefab, rc.cellPosition, Quaternion.identity);
        // }
    }

    // Update is called once per frame
    void Update()
    {
        // if (lastLength != resourceCells.Count)
        // {
            Clean();
            foreach (ResourceCell rc in resourceCells)
            {
                // GameObject rcGO = Instantiate(markPrefab, rc.cellPosition, Quaternion.identity, gameObject.transform);
                // rcGO.AddComponent<Image>().sprite = resourceSprite;
                Image rcImage = Instantiate(resourceSprite, FindObjectOfType<Canvas>().transform);
                rcImage.transform.position = Camera.main.WorldToScreenPoint(rc.cellPosition);
            }
            // lastLength = resourceCells.Count;
        // }
        
    }

    void Clean(){
        GameObject[] rCells = GameObject.FindGameObjectsWithTag("ResourceCell");
        foreach (GameObject rc in rCells)
        {
            DestroyImmediate(rc);
        }
    }
}
