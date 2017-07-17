using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    //PUBLIC VARIABLES
    //Float types
    public float verticalSpeed;
    public float horizontalSpeed;
    public float verticalRunningSpeed;
    public float horizontalRunningSpeed;
    public float degreesPerSecondRotation;
    public float jumpSpeed;
    public float maxJumpTime;
    [Range(0.0f, 100.0f)]
    public float initialStamina;
    public float staminaConsumptionPerSecond;
    
    //PRIVATE VARIABLES
    //Unity types:
    private Rigidbody rb;
    private Collider coll;
    private Vector3 jumpVelocity;
    private Vector3 verticalMovement;
    private Vector3 horizontalMovement;

    //Float types:
    private float turn;
    private float jumpTimer;
    private float verticalCurrentSpeed;
    private float horizontalCurrentSpeed;
    [Range(0.0f, 100.0f)]
    private float currentStamina;

    //Bool types
    private bool walking;
    private bool running;
    private bool rotating;
    private bool jumping;
    private bool isGround;

    void Start()
    {
        //Init boolean vars.
        walking = false;
        rotating = false;

        //Init the float vars.
        turn = 0;
        currentStamina = initialStamina;
        verticalMovement = Vector3.zero;
        horizontalMovement = Vector3.zero;

        //Init the Rigidbody var.
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        //Walk & Run system function is called.
        Movement();
        
        //Rotate system function is called.
        Rotate();

        //Jump system function is called.
        Jump();
    }

    /// <summary>
    /// The walking and running system is controlled by this function.
    /// </summary>
    void Movement()
    {
        //Sets the vertical and horizontal speed depending if the player is running or not.
        if (Input.GetButton("Run") && currentStamina > 0)
        {
            running = true;
            verticalCurrentSpeed = verticalRunningSpeed;
            horizontalCurrentSpeed = horizontalRunningSpeed;
            currentStamina -= staminaConsumptionPerSecond * Time.fixedDeltaTime;
            currentStamina = Mathf.Clamp(currentStamina, 0.0f, 100.0f);
        }
        else
        {
            running = false;
            verticalCurrentSpeed = verticalSpeed;
            horizontalCurrentSpeed = horizontalSpeed;
        }

        if (isGround)
        {
            //Gets the vertical and horizontal Input.
            verticalMovement = transform.forward * Input.GetAxis("Vertical") * verticalCurrentSpeed;
            horizontalMovement = transform.right * Input.GetAxis("Horizontal") * horizontalCurrentSpeed;  
        }

        //Moves the player using the Input and the rotation of the player. If it's looking south and the vertical axis is 1 it will move south.
        transform.position += verticalMovement * Time.fixedDeltaTime;
        transform.position += horizontalMovement * Time.fixedDeltaTime;

        //Updates the walking bool var depending if the player is walking or not. (Walking != Moving)
        if (running || (verticalMovement == Vector3.zero && verticalMovement == Vector3.zero))
            walking = false;
        else
            walking = true;
    }

    /// <summary>
    /// The player rotation system is controlled by this function. (The camera rotation is not included here, instead it's in the camera system script).
    /// </summary>
    void Rotate()
    {
        //Gets the rotation Input.
        if (!Input.GetMouseButton(0))
        {
            //Gets the turn value from the Mouse X Axis.
            turn = Input.GetAxis("Mouse X");
            //Rotates the player x(degreesPerSecond).
            transform.Rotate(Vector3.up * turn * degreesPerSecondRotation * Time.fixedDeltaTime);
        }
            
        //Updates the rotating boold var depending if the player is rotating or not.
        if (turn != 0)
            rotating = true;
        else
            rotating = false;
    }

    /// <summary>
    /// Jumping system is controlled by this function.
    /// </summary>
    void Jump()
    {
        //If the Jump button is pressed down the jump starts.
        if (Input.GetButtonDown("Jump") && isGround)
        {
            rb.velocity += Vector3.up * jumpSpeed;
            jumpVelocity = rb.velocity;
            jumpTimer = maxJumpTime;
            jumping = true;
        }

        //If the player keeps the jump button, the jump is higher.
        if (Input.GetButton("Jump") && jumping == true)
        {
            //While the jumperTimer is positive the player keeps jumping, after that it starts falling.
            if (jumpTimer > 0)
            {
                rb.velocity = jumpVelocity;
                jumpTimer -= Time.fixedDeltaTime;
            }
        }

        //When the player stops pressing the jump button, the jumping bool is false;
        if (Input.GetButtonUp("Jump"))
            jumping = false;
    }

    void OnCollisionStay(Collision collision)
    {
        //The "isGround" bool keeps true while the player is in colliding with a floor.
        if (!isGround && collision.gameObject.CompareTag("Ground"))
            isGround = true;
    }

    void OnCollisionExit(Collision collision)
    {
        //WHen the player exits colliding the floor "isGround" changes to false.
        if (collision.gameObject.CompareTag("Ground"))
            isGround = false;
    }
}