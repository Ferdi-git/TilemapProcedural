using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] float speed;
    private Rigidbody2D rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        float xInput = Input.GetAxisRaw("Horizontal");
        float yInput = Input.GetAxisRaw("Vertical");

        if (xInput == 0 && yInput == 0)
        {
            rb.linearVelocity = Vector2.zero;
        }
        else
        {
            Vector2 moveVector = new Vector2(xInput, yInput).normalized * speed;
            rb.linearVelocity = moveVector;
        }
    }
}
