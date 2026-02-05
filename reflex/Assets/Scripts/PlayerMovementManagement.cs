using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class PlayerMovementManagement : MonoBehaviour
{

    public Vector3 movementDirection;
    public float movementSpeed = 5f;
    public float gravity = 9.81f;
    public float JumpHeight;
    public float acceleration = 10f;
    public float deceleration = 10f;
    public float airAcceleration = 2f;
    public float airDeceleration = 0.5f;
    public float airResistance = 0.5f;
    private Vector3 currentVelocity;

    [SerializeField]private CharacterController playerController;
    private InputAction moveAction;
    private InputAction jumpAction;

    private PlayerInput userInput;

    private Vector3 moveInput;

    private float verticalVelocity;

    [SerializeField] private float verticalVelcocityOffset = 0;

    private void Jump()
    {
        if (playerController.isGrounded)
        {
            verticalVelocity = Mathf.Sqrt(JumpHeight * 2f * gravity);
        }
    }

    private void MovePlayer()
    {
        Vector3 moveDirection = transform.forward * moveInput.y + transform.right * moveInput.x;
        Vector3 targetVelocity = moveDirection * movementSpeed;

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

        Debug.Log("Move Direction: " + moveDirection);
        Debug.Log("Final Velocity: " + finalVelocity);
        Debug.Log("Vertical Velocity: " + verticalVelocity);
    }

    private void ReadInputs()
    {
        moveInput = moveAction.ReadValue<Vector2>();
        if (jumpAction.triggered)
        {
            Jump();
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        userInput = GetComponent<PlayerInput>();
        moveAction = userInput.actions.FindAction("Move");
        jumpAction = userInput.actions.FindAction("Jump");
        moveAction.Enable();
        jumpAction.Enable();
        
        if (JumpHeight <= 0) JumpHeight = 2f;
    }

    // Update is called once per frame
    void Update()
    {
         //Every input detection of user input it will auto move in each frame
        ReadInputs();
        MovePlayer();
    }
}
