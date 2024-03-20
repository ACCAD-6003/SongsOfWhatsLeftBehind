using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private Vector2 moveInput;
    [SerializeField] private float jumpHeight = 5f;

    private PlayerMovement playerMovement;
    private Rigidbody2D rbody;

    private void Awake()
    {
        playerMovement = new PlayerMovement();
        rbody = GetComponent<Rigidbody2D>();
        playerMovement.PlayerAcions.Jump.performed += Jump;
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
        moveInput.x = playerMovement.PlayerAcions.Movement.ReadValue<Vector2>().x;       
        rbody.velocity = new Vector2(moveInput.x * speed, rbody.velocity.y);
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
