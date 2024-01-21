using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField]
    private Vector2 speed;

    // internal variables
    private Vector2 playerInput;
    private Rigidbody2D rb;

    private void Start()
    {
        // get references
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        GetInput();
    }

    private void FixedUpdate()
    {
        ApplyMovement();
    }

    private void GetInput()
    {
        // inputs
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");

        // get input and normalize it, storing the result in the playerInput Vector2 variable
        playerInput = new Vector2(x, y).normalized;
    }

    private void ApplyMovement()
    {
        Vector2 wishVel = playerInput * speed;

        rb.velocity = wishVel;
    }
}
