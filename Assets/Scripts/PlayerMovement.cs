using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField]
    private float sensitivity;

    [SerializeField]
    private float moveSpeed;

    [SerializeField]
    private Transform cameraTransform;

    private CharacterController characterController;

    private float xRotation;

    void Start() {
        characterController = GetComponent<CharacterController>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;
        float mouseY = -Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;

        xRotation += Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;

        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        cameraTransform.eulerAngles = new Vector3(-xRotation, transform.eulerAngles.y + mouseY, 0f);
        transform.rotation *= Quaternion.Euler(0, mouseX, 0);


        float moveX = Input.GetAxis("Vertical");
        float moveY = Input.GetAxis("Horizontal");

        Vector3 velocity = transform.TransformDirection(Vector3.forward) * moveX + transform.TransformDirection(Vector3.right) * moveY;

        velocity = velocity.normalized * moveSpeed;

        characterController.Move(velocity * Time.deltaTime);
    }
}
