using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField]
    private Transform cameraTransform;

    [SerializeField]
    private Transform monsterTransform;

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

    [SerializeField]
    private float turnSpeed;


    private CharacterController characterController;

    private float xRotation;

    private float moveSpeed;

    private float sprintTimer;

    private bool exhausted;

    private bool monsterVisible;

    void Start() {
        characterController = GetComponent<CharacterController>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        moveSpeed = walkSpeed;
        sprintTimer = sprintTime;

        monsterVisible = false;
    }

    // Update is called once per frame
    void Update()
    {
        MonsterIsVisible();
        MouseLook();
        if (monsterVisible) {
            EnemyLookAway();
        }
        MoveStateUpdate();
        MovePlayer();
    }

    void MonsterIsVisible() {
        Vector3 monsterDirection = monsterTransform.position - this.gameObject.transform.position;
        monsterVisible = !Physics.Raycast(transform.position + transform.forward, monsterDirection, monsterDirection.magnitude - 2f);
        Debug.Log(monsterVisible);
    }

    void MouseLook() {
        float mouseX = Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;
        float mouseY = -Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;

        xRotation += Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;

        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        cameraTransform.eulerAngles = new Vector3(-xRotation, transform.eulerAngles.y + mouseY, 0f);
        transform.rotation *= Quaternion.Euler(0, mouseX, 0);
        
        if (monsterVisible) {
            Vector3 playerBearing = this.gameObject.transform.forward;
            Vector3 monsterBearing = Vector3.ProjectOnPlane(monsterTransform.position - this.gameObject.transform.position, Vector3.up);
            float angleBetween = Vector3.Angle(playerBearing, monsterBearing);
            if (angleBetween <= 50f) {
                transform.rotation *= Quaternion.Euler(0, -mouseX, 0);
            }    
        }    
    }

    void EnemyLookAway() {
        Vector3 playerBearing = this.gameObject.transform.forward;
        Vector3 monsterBearing = Vector3.ProjectOnPlane(monsterTransform.position - this.gameObject.transform.position, Vector3.up);
        float angleBetween = Vector3.Angle(playerBearing, monsterBearing);
        if (angleBetween <= 50f) {
            float relativeBearing = Vector3.Dot(monsterBearing, this.gameObject.transform.right);
            if (relativeBearing >= 0f) {
                transform.rotation *= Quaternion.Euler(0, -turnSpeed * Time.deltaTime, 0);
            } else {
                transform.rotation *= Quaternion.Euler(0, turnSpeed * Time.deltaTime, 0);
            }

            //Vector3 monsterRelation = Vector3.ProjectOnPlane(monsterBearing, this.gameObject.transform.right);
            //Vector3 monsterRelationDifference = monsterRelation - this.gameObject.transform.right;
            //Debug.Log(monsterRelationDifference);
            //transform.rotation *= Quaternion.Euler(0, turnSpeed * Time.deltaTime, 0);
        }
    }

    void MoveStateUpdate() {
        if (exhausted) {
            moveSpeed = slowSpeed;
            if (sprintTimer >= sprintTime) {
                sprintTimer = sprintTime;
                exhausted = false;
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
                    exhausted = true;
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
