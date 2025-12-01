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
    public float jumpForce;
    public Rigidbody2D rb;

    public void Awake()
    {
        move = InputSystem.actions.FindAction("Move");
        jump = InputSystem.actions.FindAction("Jump");
        sprint = InputSystem.actions.FindAction("Sprint");

        

        rb = GetComponent<Rigidbody2D>();
        maxSpeed = walkSpeed;
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
        if (move.ReadValue<Vector2>().x == 0)
        {
            Stop();
            return;
        } else
        {
            Move();
        }
    }

    public void Move()
    {
        rb.AddForce(new Vector2(moveDirection.x * acceleration * Time.deltaTime, 0));
        rb.linearVelocityX = Mathf.Clamp(rb.linearVelocity.x, -maxSpeed, maxSpeed);
        Debug.Log(rb.linearVelocity);
    }

    public void Stop()
    {
        rb.linearVelocityX = Mathf.MoveTowards(rb.linearVelocity.x, 0, 4 * maxSpeed * Time.deltaTime);
    }

    public void Jump(InputAction.CallbackContext context)
    {
        rb.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
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
