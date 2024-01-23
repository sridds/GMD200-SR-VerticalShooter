using System.Collections;
using UnityEngine;
using static PlayerMovement;

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

    // accessors
    public float DashTime { get { return _dashTime; } }

    // internal variables
    private Vector2 playerInput;
    private Rigidbody2D rb;
    private bool dashing;

    // events
    public delegate void DashStart();
    public DashStart OnDashStart;

    // corotuines
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

        if (Input.GetKeyDown(KeyCode.LeftShift) && activeDashCoroutine == null && playerInput != Vector2.zero) {
            activeDashCoroutine = StartCoroutine(Dash());
        }

        // get input and normalize it, storing the result in the playerInput Vector2 variable
        playerInput = new Vector2(x, y).normalized;
    }

    private void ApplyMovement()
    {
        // don't overwrite velocity if dashing
        if (dashing) return;
        Vector2 wishVel = playerInput * _speed;

        rb.velocity = wishVel;
    }

    /// <summary>
    /// Handles the dash mechanic along with the dash cooldown in the same coroutine
    /// </summary>
    /// <returns></returns>
    private IEnumerator Dash()
    {
        // call dash event
        OnDashStart?.Invoke();

        dashing = true;
        rb.velocity = playerInput * _dashSpeed;
        yield return new WaitForSeconds(_dashTime);
        dashing = false;

        yield return new WaitForSeconds(_dashCooldownTime);
        activeDashCoroutine = null;
    }
}
