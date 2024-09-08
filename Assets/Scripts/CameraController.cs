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

    void HandleKeyboardInput(){
        HandleMovement();
        HandleKeyRotation();
    }

    void HandleMovement(){
        if (Input.GetKey(KeyCode.LeftShift))
        {
            movementSpeed = fastSpeed;
        }else{
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

        
        // if (Vector3.Distance(transform.position, newPosition) > 0.01f)
        // {
        //     CheckTerrainHeight();
        //     transform.position = newZoom;
        // }
    }

    void HandleKeyRotation(){
        if (Input.GetKey(KeyCode.Q))
        {
            newRotation *= Quaternion.Euler(Vector3.up * rotationAmount);
        }

        if (Input.GetKey(KeyCode.E))
        {
            newRotation *= Quaternion.Euler(Vector3.up * -rotationAmount);
        }
    }

    void HandleMouseInput(){
        HandleMouseMovement();
        HandleMouseRotation();
        HandleMouseZoom();
    }

    void HandleMouseMovement(){
        if (!FindObjectOfType<Player>()) return;
        if (FindObjectOfType<Player>().OptionsStatus()) return;
        if (Input.GetMouseButtonDown(0)){
            Plane plane = new Plane(Vector3.up, Vector3.zero);

            Ray ray =  Camera.main.ScreenPointToRay(Input.mousePosition);

            float entry;

            if (plane.Raycast(ray, out entry)){
                dragStartPosition=ray.GetPoint(entry);
            }
        }

        if (Input.GetMouseButton(0)){
            Plane plane = new Plane(Vector3.up, Vector3.zero);

            Ray ray =  Camera.main.ScreenPointToRay(Input.mousePosition);

            float entry;

            if (plane.Raycast(ray, out entry)){

                dragEndPosition=ray.GetPoint(entry);
               
                newPosition = transform.position + dragStartPosition - dragEndPosition;
            }
        }
    }

    void HandleMouseRotation(){
        if (Input.GetMouseButtonDown(2)){
            rotateStartPosition=Input.mousePosition;
        }

        if (Input.GetMouseButton(2)){
            rotateEndPosition=Input.mousePosition;
            Vector3 difference = rotateStartPosition - rotateEndPosition;

            rotateStartPosition = rotateEndPosition;
            newRotation *= Quaternion.Euler(Vector3.up * (-difference.x / 0.5f));
        }
    }

    void HandleMouseZoom(){
        // float zoomAmount2 = 1f;
        // if (Input.GetAxis("Mouse ScrollWheel") > 0f && newZoom.y > maxZoom) // forward
        // {
        //     // newZoom += zoomAmount*4;
        //     newZoom += cameraTransform.forward * zoomAmount2;
        //     // CheckTerrainHeight();
        // }
        
        // if (Input.GetAxis("Mouse ScrollWheel") < 0f && newZoom.y < minZoom) // backward
        // {
        //     newZoom -= cameraTransform.forward * zoomAmount2;
        //     // newZoom -= zoomAmount*4;
        //     // CheckTerrainHeight();
        // }

        // cameraTransform.localPosition = Vector3.Lerp(cameraTransform.localPosition, newZoom, Time.deltaTime * movementTime);

        float scroll = Input.GetAxis("Mouse ScrollWheel");

        if (scroll > 0f && newZoom.y > maxZoom) // forward
        {
            newZoom += scroll * zoomAmount;
        }
        
        if (scroll < 0f && newZoom.y < minZoom) // backward
        {
            newZoom += scroll * zoomAmount;
        }
        // if (scroll != 0f)
        // {
        //     print(scroll);
        //     newZoom += scroll * zoomAmount;
        //     newZoom.y = Mathf.Clamp(newZoom.y, maxZoom, minZoom);
        // }
    }

    void CheckTerrainHeight() {
        terrainHeight = terrain.SampleHeight(new Vector3(transform.position.x, 0,transform.position.z)) + terrain.transform.position.y;

        // Vector3 position = transform.position;

        // print("newZoom.y = " + newZoom.y);
        // print("terrainHeight + maxZoom, max y = " + terrainHeight + maxZoom);
        // print("minZoom = " + minZoom);

        // Ensure the camera stays above the terrain plus the minimum height
        // newZoom.y = Mathf.Clamp(newZoom.y, terrainHeight + maxZoom, minZoom);
        float newYCameraPosition = Mathf.Clamp(0, terrainHeight + 1, 30);
        transform.position = new(transform.position.x, newYCameraPosition, transform.position.z);

        // if (smoothZoomFix) {
        //     // smooth movement
        //     cameraTransform.localPosition = Vector3.Lerp(cameraTransform.localPosition, newZoom, Time.deltaTime * movementTime);
        // } else {
        //     //  instant movement
        //     cameraTransform.localPosition = newZoom;
        // }

        // float newZoomFix = Mathf.Clamp(newZoom.y, terrainHeight + maxZoom, minZoom);
        
        // if (newZoom.y != newZoomFix) {
        //     print("fixed");
        //     newZoom.y = newZoomFix;
        // } else {
        //     print("right");
        // }

        // Apply the new position to the camera
        // cameraTransform.localPosition = cameraTransform.InverseTransformPoint(newZoom);
        // cameraTransform.localPosition = newZoom;
        
        // cameraTransform.localPosition = Vector3.Lerp(cameraTransform.localPosition, position, Time.deltaTime * movementTime);

    }

    public void FocusBuilding(Vector3 position) {
        transform.position = position;
    }
}
