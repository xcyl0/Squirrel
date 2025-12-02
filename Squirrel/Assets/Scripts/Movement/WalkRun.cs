using UnityEditor.Timeline;
using UnityEngine;
using UnityEngine.InputSystem;
public class WalkRun : MonoBehaviour
{
    public InputAction move;
    public InputAction jump;
    public InputAction sprint;
    public Vector2 moveDirection;
    public float acceleration;
    public float walkSpeed;
    public float runSpeed;
    public float maxSpeed;
    public float maxClimbSpeed;
    public float jumpForce;
    public float stoppingPower;

    public Collider2D clb;
    public Rigidbody2D rb;

    public bool climbing;
    public bool onsurface;

    public void Awake()
    {
        move = InputSystem.actions.FindAction("Move");
        jump = InputSystem.actions.FindAction("Jump");
        sprint = InputSystem.actions.FindAction("Sprint");

        

        rb = GetComponent<Rigidbody2D>();
        maxSpeed = walkSpeed;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Climbable")
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0);
            rb.gravityScale = 0.5f;
            climbing = true;
        }
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Climbable")
        {
            rb.gravityScale = 1.5f;
            climbing = false;
        }     
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("Collided with " + collision.gameObject.name + " on layer " + collision.gameObject.layer);
        if (collision.gameObject.layer == 3)
            onsurface = true;
    }
    public void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.layer == 3)
            onsurface = false;
    }

    private void OnEnable()
    {
        move.Enable();
        jump.Enable();
        sprint.Enable();

        jump.performed += Jump;
        sprint.started += Sprint;
        sprint.canceled += Walk;
    }

    private void OnDisable()
    {
        move.Disable();
        jump.Disable();
        sprint.Disable();

        jump.performed -= Jump;
        sprint.started -= Sprint;
        sprint.canceled -= Walk;
    }

    private void Update()
    {
        moveDirection = move.ReadValue<Vector2>();
        if (move.ReadValue<Vector2>() == new Vector2(0,0))
        {
            Stop();
            return;
        } else
        {
            Flip();
            Move();
        }
    }

    public void Flip()
    {
        if (moveDirection.x > 0)
            transform.localScale = new Vector3(-1, 1, 1);
        else if (moveDirection.x < 0)
            transform.localScale = new Vector3(1, 1, 1);
    }
    public void Move()
    {
        if (climbing)
        {
            rb.AddForce(new Vector2(moveDirection.x * acceleration * Time.deltaTime, moveDirection.y * acceleration * Time.deltaTime + Mathf.Abs(moveDirection.x) * acceleration * Time.deltaTime / 3));
            rb.linearVelocityY = Mathf.Clamp(rb.linearVelocity.y, -maxClimbSpeed, maxClimbSpeed);
            rb.linearVelocityX = Mathf.Clamp(rb.linearVelocity.x, -maxSpeed, maxSpeed);
        } else
        {
            rb.AddForce(new Vector2(moveDirection.x * acceleration * Time.deltaTime, 0));
            rb.linearVelocityX = Mathf.Clamp(rb.linearVelocity.x, -maxSpeed, maxSpeed);
        }
    }

    public void Stop()
    {
        rb.linearVelocityX = Mathf.MoveTowards(rb.linearVelocity.x, 0, stoppingPower* Time.deltaTime);
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (!onsurface) return;
        if (!climbing)  
        rb.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
        else
            rb.AddForce(new Vector2(jumpForce / 2 * transform.localScale.x, jumpForce * 1.5f), ForceMode2D.Impulse);

    }

    public void Sprint(InputAction.CallbackContext context)
    {
        maxSpeed = runSpeed;
    }

    public void Walk(InputAction.CallbackContext context)
    {
        maxSpeed = walkSpeed;
    }
}
