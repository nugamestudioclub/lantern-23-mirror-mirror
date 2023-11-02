using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField]
    private Transform cameraTransform;

    [SerializeField]
    private float sensitivity;

    [SerializeField]
    private float walkSpeed;

    [SerializeField]
    private float sprintSpeed;
    
    [SerializeField]
    private float slowSpeed;

    [SerializeField]
    private float sprintTime;

    [SerializeField]
    private float sprintRecoveryRate;

    [SerializeField]
    private float exhaustionRecoveryRate;


    private CharacterController characterController;

    private float xRotation;

    private float moveSpeed;

    private float sprintTimer;

    enum MoveState{
        Walk,
        Sprint,
        Slow
    };

    private MoveState moveState;


    void Start() {
        characterController = GetComponent<CharacterController>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        moveSpeed = walkSpeed;
        sprintTimer = sprintTime;
    }

    // Update is called once per frame
    void Update()
    {
        MouseLook();
        MoveStateUpdate();
        MovePlayer();
    }

    void MouseLook() {
        float mouseX = Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;
        float mouseY = -Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;

        xRotation += Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;

        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        cameraTransform.eulerAngles = new Vector3(-xRotation, transform.eulerAngles.y + mouseY, 0f);
        transform.rotation *= Quaternion.Euler(0, mouseX, 0);
    }

    void MoveStateUpdate() {
        if (moveState == MoveState.Slow) {
            moveSpeed = slowSpeed;
            if (sprintTimer >= sprintTime) {
                sprintTimer = sprintTime;
                moveState = MoveState.Walk;
            } else {
                sprintTimer += exhaustionRecoveryRate * Time.deltaTime;
            }
        } else {
            if (Input.GetKey(KeyCode.LeftShift)) {
                if (sprintTimer > 0f) {
                    moveSpeed = sprintSpeed;
                    sprintTimer -= Time.deltaTime;
                } else {
                    sprintTimer = 0f;
                    moveState = MoveState.Slow;
                    moveSpeed = slowSpeed;
                }
            } else {
                moveSpeed = walkSpeed;
                if (sprintTimer < sprintTime) {
                    sprintTimer += Mathf.Min(sprintTime - sprintTimer, sprintRecoveryRate * Time.deltaTime);
                }
            }
        }
    }

    void MovePlayer() {
        float moveX = Input.GetAxis("Vertical");
        float moveY = Input.GetAxis("Horizontal");

        Vector3 velocity = transform.TransformDirection(Vector3.forward) * moveX + transform.TransformDirection(Vector3.right) * moveY;

        velocity = velocity.normalized * moveSpeed;

        characterController.Move(velocity * Time.deltaTime);
    }
}
