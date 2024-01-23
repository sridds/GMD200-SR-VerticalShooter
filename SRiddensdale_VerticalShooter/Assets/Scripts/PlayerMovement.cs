using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField]
    private Vector2 _speed = new Vector2(10, 10);

    [Header("Dash Settings")]
    [SerializeField]
    private bool _allowDash = true;
    [SerializeField]
    private float _dashSpeed = 15.0f;
    [SerializeField]
    private float _dashTime = 0.5f;
    [SerializeField]
    private float _dashCooldownTime = 1.5f;

    // internal variables
    private Vector2 playerInput;
    private Rigidbody2D rb;

    private bool dashing;
    private Vector2 lastPlayerInput;

    private Coroutine activeDashCoroutine = null;

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

        if (Input.GetKeyDown(KeyCode.LeftShift) && activeDashCoroutine == null) {
            activeDashCoroutine = StartCoroutine(Dash());
        }

        // get input and normalize it, storing the result in the playerInput Vector2 variable
        playerInput = new Vector2(x, y).normalized;

        // cache last player input
        if (playerInput != Vector2.zero) lastPlayerInput = playerInput;
    }

    private void ApplyMovement()
    {
        if (dashing) return;
        Vector2 wishVel = playerInput * _speed;

        rb.velocity = wishVel;
    }

    private IEnumerator Dash()
    {
        dashing = false;
        rb.velocity = lastPlayerInput * _dashSpeed;
        yield return new WaitForSeconds(_dashTime);
        dashing = true;

        yield return new WaitForSeconds(_dashCooldownTime);
        activeDashCoroutine = null;
    }
}
