using UnityEngine;

public class PreviewSystem : MonoBehaviour
{
    [SerializeField]
    private float previewYOffset = 0.06f;
    [SerializeField]
    private GameObject cellIndicator;
    private GameObject previewObject;

    [SerializeField]
    private Material previewMaterialsPrefab;
    private Material previewMaterialInstance;

    private Renderer cellIndicatorRenderer;

    private void Awake()
    {
        previewMaterialInstance = new Material(previewMaterialsPrefab);
        cellIndicator.SetActive(false);
        cellIndicatorRenderer = cellIndicator.GetComponentInChildren<Renderer>();
    }
    public void StartShowingPOlacementPreview(GameObject prefab, Vector2Int size)
    {
        previewObject = Instantiate(prefab);
        PreparePreavie(previewObject);
        PrepareCursor(size);
        cellIndicator.SetActive(true);
    }

    private void PrepareCursor(Vector2Int size)
    {
        if (size.x > 0 || size.y > 0) 
        { 
            cellIndicator.transform.localScale = new Vector3(size.x,1,size.y);
            cellIndicatorRenderer.material.mainTextureScale = size;
        }
    }

    private void PreparePreavie(GameObject previewObject)
    {
        Renderer[] renderers = previewObject.GetComponentsInChildren<Renderer>();
        foreach (Renderer renderer in renderers)
        {
            Material[] materials = renderer.materials;
            for (int i = 0; i < materials.Length; i++)
            {
                materials[i] = previewMaterialInstance;
            }
            renderer.materials = materials;
        }
    }

    public void StopShowingPreview()
    {
        cellIndicator.SetActive(false);
        if( previewObject != null)
        {
            Destroy(previewObject);
        }
    }
    public void UndatePosition(Vector3 position, bool validity)
    {
        if(previewObject != null)
        {
            MovePreview(position);
            ApllyFeedbackToPreview(validity);
        }
        MoveCursor(position);
        ApllyFeedbackToCursor(validity);
    }

    private void ApllyFeedbackToPreview(bool validity)
    {
        Color c = validity ? Color.cyan : Color.red;
        c.a = 0.5f;
        previewMaterialInstance.color = c;
    }
    private void ApllyFeedbackToCursor(bool validity)
    {
        Color c = validity ? Color.cyan : Color.blue;
        c.a = 0.5f;
        cellIndicatorRenderer.material.color = c;
    }

    private void MoveCursor(Vector3 position)
    {
        cellIndicator.transform.position = position;
    }

    private void MovePreview(Vector3 position)
    {
        previewObject.transform.position = new Vector3(position.x, position.y + previewYOffset, position.z);
    }

    internal void StartShowingRemovePreview()
    {
        cellIndicator.SetActive(true);
        PrepareCursor(Vector2Int.one);
        ApllyFeedbackToCursor(false);
    }

    internal void StartShowingPreview()
    {
        cellIndicator.SetActive(true);
        PrepareCursor(Vector2Int.one);
        ApllyFeedbackToCursor(false);
    }
}
