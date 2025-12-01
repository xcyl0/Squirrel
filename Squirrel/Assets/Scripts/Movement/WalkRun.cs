using UnityEngine;
using UnityEngine.InputSystem;
public class WalkRun : MonoBehaviour
{
    public InputAction move;
    public Vector2 moveDirection;
    public float acceleration;
    public float maxSpeed;
    public float jumpForce;
    public Rigidbody2D rb;

    public void Awake()
    {
        move = InputSystem.actions.FindAction("Move");
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        moveDirection = move.ReadValue<Vector2>();
        if (move.ReadValue<Vector2>().x == 0)
        {
            rb.linearVelocityX = Mathf.MoveTowards(rb.linearVelocity.x, 0, acceleration * Time.deltaTime);
            return;
        }
        rb.AddForce(new Vector2(moveDirection.x * acceleration * Time.deltaTime, 0));
        rb.linearVelocityX = Mathf.Clamp(rb.linearVelocity.x, -maxSpeed, maxSpeed);
        Debug.Log(rb.linearVelocity);
    }
}
