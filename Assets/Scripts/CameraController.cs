using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    // Start is called before the first frame update
    void Start()
    {
        newPosition = transform.position;
        newRotation = transform.rotation;
        newZoom = cameraTransform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {

        HandleKeyboardInput();
        HandleMouseInput();

    }

    void HandleKeyboardInput(){
        HandleMovement();
        HandleRotation();
        // HandleZoom();
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

        transform.position = Vector3.Lerp(transform.position, newPosition, Time.deltaTime * movementTime);
    }

    void HandleRotation(){
        if (Input.GetKey(KeyCode.Q))
        {
            newRotation *= Quaternion.Euler(Vector3.up * rotationAmount);
        }

        if (Input.GetKey(KeyCode.E))
        {
            newRotation *= Quaternion.Euler(Vector3.up * -rotationAmount);
        }

        transform.rotation = Quaternion.Lerp(transform.rotation, newRotation, Time.deltaTime * movementTime);
    }

    void HandleZoom(){
        if (Input.GetKey(KeyCode.R))
        {
            newZoom += zoomAmount;
        }
        if (Input.GetKey(KeyCode.F))
        {
            newZoom -= zoomAmount;
        }

        cameraTransform.localPosition = Vector3.Lerp(cameraTransform.localPosition, newZoom, Time.deltaTime * movementTime);
    }

    void HandleMouseInput(){
        HandleMouseMovement();
        HandleMouseRotation();
        HandleMouseZoom();
    }

    void HandleMouseMovement(){
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
        if (Input.GetAxis("Mouse ScrollWheel") > 0f && maxZoom < newZoom.y) // forward
        {
            newZoom += zoomAmount*4;
        }
        
        if (Input.GetAxis("Mouse ScrollWheel") < 0f && minZoom > newZoom.y) // backward
        {
            newZoom -= zoomAmount*4;
        }
        
        cameraTransform.localPosition = Vector3.Lerp(cameraTransform.localPosition, newZoom, Time.deltaTime * movementTime);
    }
}
