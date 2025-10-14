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


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        characterController = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();
    }
    void Update()
    {
        handleInput();
        handleRotation();
        handleAnimation();
        handleGravity();
        handleMovement();
    }

    private void handleInput()
    {
        currentMovementInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        isRunPressed = Input.GetKey(KeyCode.LeftShift);  
    }

    private void handleMovement()
    {
        isMovementPressed = currentMovementInput.x != 0f || currentMovementInput.y != 0f;

        currentMovement.x = currentMovementInput.x;
        currentMovement.z = currentMovementInput.y;

        currentRunMovement.x = currentMovementInput.x * runMultiplier;
        currentRunMovement.z = currentMovementInput.y * runMultiplier;

        if (isRunPressed)
        { characterController.Move(currentRunMovement * Time.deltaTime); }
        else
        { characterController.Move(currentMovement * Time.deltaTime); }
    }


    private void handleGravity()
    {
        if (characterController.isGrounded)
        { 
            currentMovement.y = groundedGravity;
            currentRunMovement.y = groundedGravity;
        }
        else
        {
            currentMovement.y = gravity;
            currentRunMovement.y = gravity;
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
