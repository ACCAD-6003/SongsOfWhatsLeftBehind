using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private float speed;
    private PlayerMovement playerMovement;
    private Rigidbody2D rbody;
    private Vector2 moveInput;
    private float jumpHeight = 5f;

    private void Awake()
    {
        playerMovement = new PlayerMovement();
        rbody = GetComponent<Rigidbody2D>();
    }

    private void Jump(InputAction.CallbackContext ctx)
    {
        if (rbody.velocity.y == 0)
        {
            Debug.Log("Bruh");
            rbody.AddForce(Vector2.up * jumpHeight, ForceMode2D.Impulse);
        }
    }

    private void FixedUpdate()
    {
        moveInput = playerMovement.PlayerAcions.Movement.ReadValue<Vector2>();       
        if (rbody.velocity.y == 0)
        {
            rbody.AddForce(Vector2.up * jumpHeight, ForceMode2D.Impulse);
        }
        else
        {
            moveInput.y = 0;
        }
        rbody.velocity = moveInput * speed;
    }

    private void OnEnable()
    {
        playerMovement.PlayerAcions.Enable();
    }

    private void OnDisable()
    {
        playerMovement.PlayerAcions.Disable();
    }
}
