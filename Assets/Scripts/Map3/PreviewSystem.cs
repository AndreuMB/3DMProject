using UnityEngine;

public class PreviewSystem : MonoBehaviour
{
    [SerializeField]
    private float previewYOffset = 0.06f;
    [SerializeField] GameObject cellIndicator;
    private GameObject cellSelector;
    [SerializeField] GameObject cellIndicatorSecondary;
    private GameObject cellSelectorSecondary;
    private GameObject previewObject;

    [SerializeField]
    private Material previewMaterialsPrefab;
    private Material previewMaterialInstance;

    private Renderer cellIndicatorRenderer;

    private void Awake()
    {
        previewMaterialInstance = new Material(previewMaterialsPrefab);

        // cellIndicator
        cellSelector = Instantiate(cellIndicator);
        cellIndicator.SetActive(false);
        cellSelector.SetActive(false);

        // cellIndicatorSecondary
        cellSelectorSecondary = Instantiate(cellIndicatorSecondary);
        cellIndicatorSecondary.SetActive(false);
        cellSelectorSecondary.SetActive(false);

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
            cellIndicator.transform.localScale = new Vector3(size.x, 1, size.y);
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

    public void StopShowingPreview(bool secondaryIndicator = false)
    {
        if (secondaryIndicator)
        {
            cellIndicatorSecondary.SetActive(false);
        }
        else
        {
            cellIndicator.SetActive(false);
        }
        if (previewObject != null)
        {
            Destroy(previewObject);
        }
    }
    public void UndatePosition(Vector3 position, bool validity, Color color, bool selector = false, bool secondaryIndicator = false)
    {
        if (previewObject != null && !selector)
        {
            MovePreview(position);
            ApllyFeedbackToPreview(validity, color);
        }
        else if (selector && secondaryIndicator)
        {
            cellIndicatorSecondary.SetActive(true);
            cellIndicatorSecondary.transform.position = position;
        }
        else if (selector)
        {
            cellSelector.SetActive(true);
            cellSelector.transform.position = position;
        }
        else
        {
            MoveCursor(position);
        }
        ApllyFeedbackToCursor(validity, color);
    }

    private void ApllyFeedbackToPreview(bool validity, Color color)
    {
        print("validity = " + validity);
        Color c = validity ? color : Color.red;
        c.a = 0.5f;
        previewMaterialInstance.color = c;
    }
    private void ApllyFeedbackToCursor(bool validity, Color color)
    {
        Color c = validity ? color : Color.blue;
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
        ApllyFeedbackToCursor(false, Color.blue);
    }

    internal void StartShowingPreview()
    {
        cellIndicator.SetActive(true);
        PrepareCursor(Vector2Int.one);
        ApllyFeedbackToCursor(false, Color.blue);
    }
}
