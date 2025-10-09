using System;
using UnityEngine;

public class Player : MonoBehaviour
{
    CharacterController characterController;
    Animator animator;
    Vector3 currentMovement;
    Vector3 currentRunMovement;
    Vector2 currentMovementInput;
    bool isMovementPressed;

    float rotationFactorPerFrame = 15.0f;

    float runMultiplier = 3f;
    bool isRunPressed = false;

    float gravity = -9.81f;
    float groundedGravity = -0.5f;

    bool isJumpPressed = false;
    float initialJumpVelocity;
    float maxJumpHeight = 1.0f;
    float maxJumpTime = 0.5f;
    bool isJumping = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        characterController = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();

        setupJumpVariables();
    }
    void Update()
    {
        handleInput();
        handleRotation();
        handleAnimation();
        handleMovement();
        handleGravity();
        handleJump();
        
    }

    private void handleInput()
    {
        currentMovementInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        isRunPressed = Input.GetKey(KeyCode.LeftShift);

        if(Input.GetKeyDown(KeyCode.Space))
        {
            isJumpPressed = true;
            Debug.Log(isJumpPressed);
        }
        else if (Input.GetKeyUp(KeyCode.Space))
        { 
            isJumpPressed = false;
            Debug.Log(isJumpPressed);
        }

          
    }

    private void handleMovement()
    {
        isMovementPressed = currentMovementInput.x != 0f || currentMovementInput.y != 0f;

        currentMovement.x = currentMovementInput.x;
        currentMovement.z = currentMovementInput.y;

        currentRunMovement.x = currentMovementInput.x * runMultiplier;
        currentRunMovement.z = currentMovementInput.y * runMultiplier;

      if(isRunPressed) 
        { characterController.Move(currentRunMovement * Time.deltaTime); }
      else  
        { characterController.Move(currentMovement * Time.deltaTime); }

        //Debug.Log(String.Format("CurrentMovementInput x = {0} , y = {1} , z = {2}", currentMovementInput.x, currentMovementInput.y,"0"));
        //characterController.Move(currentMovement * movementMultiplier * Time.deltaTime);
    }

    private void setupJumpVariables()
    {
        float timeToApex = maxJumpTime / 2;
        gravity = (-2 * maxJumpHeight) / (timeToApex * timeToApex);
        initialJumpVelocity = (2 * maxJumpHeight) / timeToApex;

    }

    void handleJump()
    {
        if (!isJumping && characterController.isGrounded && isJumpPressed)
        {
            isJumping = true;
            currentMovement.y = initialJumpVelocity;
            currentRunMovement.y = initialJumpVelocity;
        }
        else if (!isJumpPressed && isJumping && characterController.isGrounded)
        {
            isJumping = false;
        }
    }

    // Update is called once per frame


    private void handleGravity()
    {
        if (characterController.isGrounded)
        { 
            currentMovement.y = groundedGravity;
            currentRunMovement.y = groundedGravity;
        }
        else
        {
            float previousVelocity = currentMovement.y;
            float newYVelocity = currentMovement.y + (gravity * Time.deltaTime);
            float nextYVelocity = (previousVelocity + newYVelocity) * .5f;
            currentMovement.y += nextYVelocity;
            currentRunMovement.y += nextYVelocity;
        }
    }

    

    private void handleRotation()
    {
        Vector3 positionToLookAt;
        positionToLookAt.x = currentMovement.x;
        positionToLookAt.y = 0f;
        positionToLookAt.z = currentMovement.z;

        Quaternion currentRotation = transform.rotation;

        if (isMovementPressed)
        {
            Quaternion targetRotation = Quaternion.LookRotation(positionToLookAt);
            transform.rotation = Quaternion.Slerp(currentRotation, targetRotation, rotationFactorPerFrame * Time.deltaTime);
        }


    }

    private void handleAnimation()
    {
        bool isWalking = animator.GetBool("isWalking");
        bool isRunning = animator.GetBool("isRunning");

        if (isMovementPressed && !isWalking) animator.SetBool("isWalking", true);

        else if (!isMovementPressed && isWalking) animator.SetBool("isWalking", false);

        if ((isMovementPressed &&isRunPressed) && !isRunning) animator.SetBool("isRunning", true);

        else if ((!isMovementPressed || !isRunPressed) && isRunning) animator.SetBool("isRunning", false);

    }




}
