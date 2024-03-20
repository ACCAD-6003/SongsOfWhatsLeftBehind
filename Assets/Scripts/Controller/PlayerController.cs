using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private float speed;
    private PlayerMovement playerMovement;
    private Rigidbody2D rbody;
    private Vector2 moveInput;

    private void Awake()
    {
        playerMovement = new PlayerMovement();
        rbody = GetComponent<Rigidbody2D>();
        playerMovement.PlayerAcions.Jump.performed += Jump();
    }

    private void Jump()
    {

    }

    private void FixedUpdate()
    {
        moveInput = playerMovement.PlayerAcions.Movement.ReadValue<Vector2>();
        moveInput.y = 0f;
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
