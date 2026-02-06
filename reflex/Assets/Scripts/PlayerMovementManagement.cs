using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class PlayerMovementManagement : MonoBehaviour
{

    public Vector3 movementDirection;
    public float movementSpeed = 5f;
    public float sprintSpeed = 8f;
    public float gravity = 9.81f;
    public float JumpHeight;
    public float acceleration = 10f;
    public float deceleration = 10f;
    public float airAcceleration = 2f;
    public float airDeceleration = 0.5f;
    public float airResistance = 0.5f;
    public float deadZone;
    private Vector3 currentVelocity;

    [SerializeField]private CharacterController playerController;
    [SerializeField]private CinemachinePositionComposer camera;

    private InputAction moveAction;
    private InputAction jumpAction;
    private InputAction sprintAction;

    private PlayerInput userInput;

    private Vector3 moveInput;

    private float verticalVelocity;

    private bool isSprinting;

    [SerializeField] private float verticalVelcocityOffset = 0;

    private void Jump()
    {
        if (playerController.isGrounded)
        {
            verticalVelocity = Mathf.Sqrt(JumpHeight * 2f * gravity);
        }
    }

    private float GetCurrentSpeed()
    {
        if (sprintAction != null && sprintAction.IsPressed())
        {
            return sprintSpeed;
        }
        return movementSpeed;
    }

    private void MovePlayer()
    {
        Vector3 moveDirection = transform.forward * moveInput.y + transform.right * moveInput.x;
        Vector3 targetVelocity = moveDirection * GetCurrentSpeed();

        float tempAccel = playerController.isGrounded ? acceleration : airAcceleration;
        float tempDecel = playerController.isGrounded ? deceleration : airDeceleration;

        if (moveDirection.magnitude > 0.1f)
        {
            currentVelocity = Vector3.MoveTowards(currentVelocity, targetVelocity, tempAccel * Time.deltaTime);
        }
        else
        {
            currentVelocity = Vector3.MoveTowards(currentVelocity, Vector3.zero, tempDecel * Time.deltaTime);
        }

        if (!playerController.isGrounded)
        {
            currentVelocity = Vector3.Lerp(currentVelocity, Vector3.zero, airResistance * Time.deltaTime);
        }
        
        
        if (playerController.isGrounded && verticalVelocity < 0) {

            if (verticalVelcocityOffset > 0) {
                float temp = verticalVelcocityOffset * -1f;
                verticalVelcocityOffset = temp;
            }

            verticalVelocity = verticalVelcocityOffset;

        } else {
            verticalVelocity -= gravity * Time.deltaTime;
        }

        Vector3 finalVelocity = currentVelocity + (Vector3.up * verticalVelocity); 
        playerController.Move(finalVelocity * Time.deltaTime);
    }

    private void ReadInputs()
    {
        moveInput = moveAction.ReadValue<Vector2>();
        if (jumpAction.triggered)
        {
            Jump();
        }

        isSprinting = sprintAction.IsPressed();
        if (isSprinting) {
            Debug.Log("Sprinting");
        }
    }

    private void FOVChangeWhenRunning()
    {
        if (isSprinting)
        {
            camera.DeadZoneDepth = deadZone; // Adjust this value to increase/decrease the FOV change
            Debug.Log("FOV Change: " + camera.DeadZoneDepth);
        } else
        {
            camera.DeadZoneDepth = 0.0f;
            Debug.Log("FOV Reset: " + camera.DeadZoneDepth);
        }
    }

    // Start is called once
    // {} before the first execution of Update after the MonoBehaviour is created
    void Start() {
        userInput = GetComponent<PlayerInput>();
        moveAction = userInput.actions.FindAction("Move");
        jumpAction = userInput.actions.FindAction("Jump");
        sprintAction = userInput.actions.FindAction("Sprint");
        moveAction.Enable();
        jumpAction.Enable();
        sprintAction?.Enable();
        
        if (JumpHeight <= 0) JumpHeight = 2f;
    }

    // Update is called once per frame
    void Update() {
         //Every input detection of user input it will auto move in each frame
        ReadInputs();
        MovePlayer();
        FOVChangeWhenRunning();
    }
}