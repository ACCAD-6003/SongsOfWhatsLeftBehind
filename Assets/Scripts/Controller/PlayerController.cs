using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private Vector2 moveInput;
    [SerializeField] private float jumpHeight = 5f;
    private Animator anim;

    private PlayerMovement playerMovement;
    private Rigidbody2D rbody;

    private void Awake()
    {
        playerMovement = new PlayerMovement();
        rbody = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        playerMovement.PlayerAcions.Jump.performed += Jump;
    }

    private void Jump(InputAction.CallbackContext ctx)
    {
        if (rbody.velocity.y == 0)
        {
            anim.SetBool("isJumping", true);
            // Debug.Log("Bruh");
        }
    }
    public void AddJumpForce()
    {
        rbody.AddForce(Vector2.up * jumpHeight, ForceMode2D.Impulse);
    }

    public void Update()
    {
        bool flipped = rbody.velocity.x < 0;
        bool notFlipped = rbody.velocity.x > 0;
        if (flipped)
        {
            transform.rotation = Quaternion.Euler(new Vector3(0f, 180f, 0f));
        }
        else if (notFlipped)
        {
            transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
        }

        if (rbody.velocity.y > 0)
        {
            //Debug.Log("LAMO");
            Physics2D.IgnoreLayerCollision(0, 3, true);
        }
        //else the collision will not be ignored
        else
        {
            Physics2D.IgnoreLayerCollision(0, 3, false);
        }
    }

    private void FixedUpdate()
    {
        moveInput.x = playerMovement.PlayerAcions.Movement.ReadValue<Vector2>().x;
        rbody.velocity = new Vector2(moveInput.x * speed, rbody.velocity.y);
        if (rbody.velocity.x != 0)
        {
            anim.SetBool("isRunning", true);
        }
        else
        {
            anim.SetBool("isRunning", false);
        }

        if (rbody.velocity.y < 0)
        {
            anim.SetBool("isJumping", false);
            anim.SetBool("isRising", false);
            anim.SetBool("isFalling", true);
        }
        else if (rbody.velocity.y >= 0)
        {
            anim.SetBool("isFalling", false);
            if (rbody.velocity.y > 0)
            {
                anim.SetBool("isRising", true);
            }
        }
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
