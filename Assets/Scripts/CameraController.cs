using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CameraController : MonoBehaviour
{
    public Transform cameraTransform;

    [Header("Keyboard Settings")]
    public float normalSpeed;
    public float fastSpeed;
    float movementSpeed;
    public float movementTime;
    public float rotationAmount;
    public Vector3 zoomAmount;

    [Header("Mouse Settings")]
    Vector3 dragStartPosition;
    Vector3 dragEndPosition;
    Vector3 rotateStartPosition;
    Vector3 rotateEndPosition;

    Vector3 newPosition;
    Quaternion newRotation;
    Vector3 newZoom;
    [SerializeField] float minZoom;
    [SerializeField] float maxZoom;
    // bool smoothZoomFix;

    Terrain terrain; // Reference to the terrain
    float terrainHeight;
    GameObject optionsMenu;
    bool hasCameraMoved = false;
    Vector3 cameraStartPosition;

    // Start is called before the first frame update
    void Start()
    {
        newPosition = transform.position;
        newRotation = transform.rotation;
        newZoom = cameraTransform.localPosition;
        terrain = Terrain.activeTerrain;
        optionsMenu = FindObjectOfType<Options>().gameObject;
    }

    // Update is called once per frame
    void Update()
    {

        if (optionsMenu.activeSelf) return;

        HandleKeyboardInput();
        HandleMouseInput();

        // update rotation
        transform.rotation = Quaternion.Lerp(transform.rotation, newRotation, Time.deltaTime * movementTime);

        // update zoom
        cameraTransform.localPosition = Vector3.Lerp(cameraTransform.localPosition, newZoom, Time.deltaTime * movementTime);

        // update movement
        transform.position = Vector3.Lerp(transform.position, newPosition, Time.deltaTime * movementTime);

        CheckTerrainHeight();

    }

    void HandleKeyboardInput()
    {
        HandleMovement();
        HandleKeyRotation();
    }

    void HandleMovement()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            movementSpeed = fastSpeed;
        }
        else
        {
            movementSpeed = normalSpeed;
        }

        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            newPosition += transform.forward * movementSpeed;
        }

        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            newPosition -= transform.forward * movementSpeed;
        }

        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            newPosition += transform.right * movementSpeed;
        }

        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            newPosition -= transform.right * movementSpeed;

        }
    }

    void HandleKeyRotation()
    {
        if (Input.GetKey(KeyCode.Q))
        {
            newRotation *= Quaternion.Euler(Vector3.up * rotationAmount);
        }

        if (Input.GetKey(KeyCode.E))
        {
            newRotation *= Quaternion.Euler(Vector3.up * -rotationAmount);
        }
    }

    void HandleMouseInput()
    {
        HandleMouseMovement();
        HandleMouseRotation();
        HandleMouseZoom();
    }

    void HandleMouseMovement()
    {
        if (!FindObjectOfType<Player>()) return;
        if (FindObjectOfType<Player>().OptionsStatus()) return;
        if (Input.GetMouseButtonDown(0))
        {
            ResetHasCameraMove();
            Plane plane = new Plane(Vector3.up, Vector3.zero);

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            float entry;

            if (plane.Raycast(ray, out entry))
            {
                dragStartPosition = ray.GetPoint(entry);
            }
            cameraStartPosition = transform.position;
        }

        if (Input.GetMouseButton(0))
        {
            Plane plane = new Plane(Vector3.up, Vector3.zero);

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            float entry;

            if (plane.Raycast(ray, out entry))
            {

                dragEndPosition = ray.GetPoint(entry);
                newPosition = transform.position + dragStartPosition - dragEndPosition;
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            float distanceMoved = Vector3.Distance(cameraStartPosition, transform.position);
            // Check if the distance moved exceeds the threshold
            if (distanceMoved > 1)
            {
                hasCameraMoved = true;
            }
        }
    }

    void HandleMouseRotation()
    {
        if (Input.GetMouseButtonDown(2))
        {
            rotateStartPosition = Input.mousePosition;
        }

        if (Input.GetMouseButton(2))
        {
            rotateEndPosition = Input.mousePosition;
            Vector3 difference = rotateStartPosition - rotateEndPosition;

            rotateStartPosition = rotateEndPosition;
            newRotation *= Quaternion.Euler(Vector3.up * (-difference.x / 0.5f));
        }
    }

    void HandleMouseZoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        if (scroll > 0f && newZoom.y > maxZoom) // forward
        {
            newZoom += scroll * zoomAmount;
        }

        if (scroll < 0f && newZoom.y < minZoom) // backward
        {
            newZoom += scroll * zoomAmount;
        }
    }

    void CheckTerrainHeight()
    {
        terrainHeight = terrain.SampleHeight(new Vector3(transform.position.x, 0, transform.position.z)) + terrain.transform.position.y;
        float newYCameraPosition = Mathf.Clamp(0, terrainHeight + 1, 30);
        transform.position = new(transform.position.x, newYCameraPosition, transform.position.z);
    }

    public void FocusBuilding(Vector3 position)
    {
        transform.position = position;
    }

    public bool GetHasCameraMove()
    {
        return hasCameraMoved;
    }

    public void ResetHasCameraMove()
    {
        hasCameraMoved = false;
    }
}
