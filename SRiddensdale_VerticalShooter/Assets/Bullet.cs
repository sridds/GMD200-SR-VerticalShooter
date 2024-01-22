using UnityEngine;
using NaughtyAttributes;
using System.Collections;

public class Bullet : MonoBehaviour
{
    private Rigidbody2D rb;

    [Header("Bullet Settings")]
    [SerializeField]
    private float _lifeTime;

    [Header("Movement Settings")]
    [SerializeField, Tooltip("Allows the velocity / position to be modified directly to follow a custom curve")]
    private bool _useMovementCurve;
    [ShowIf(nameof(_useMovementCurve))]
    [SerializeField, Tooltip("Keep this restricted to -1 to 1 for best results. Ensure looping mode is set to keep the bullets looping on the path")]
    private AnimationCurve _movementCurve;
    [ShowIf(nameof(_useMovementCurve))]
    [SerializeField, Min(0), Tooltip("Determines how fast the custom curve is evaluated")]
    private float _curveEvaluationSpeed;
    [ShowIf(nameof(_useMovementCurve))]
    [SerializeField, Min(0), Tooltip("Determines the strength of the curve")]
    private float _curveEvaluationAmplitude;
    [ShowIf(nameof(_useMovementCurve))]
    [SerializeField, Tooltip("If false, the position will be adjusted, allowing the movement to move along the path. If true, the bullet will oscillate on a different axis")]
    private bool _curveAlongVelocityPath = false;

    [Header("Speed Settings")]
    [SerializeField]
    private AnimationCurve _speedOverTime;
    [HideIf(nameof(_useLifetime))]
    [SerializeField]
    private float _speedCurveEvaluationTime;
    [SerializeField]
    private bool _useLifetime;

    [Header("Homing")]
    [SerializeField]
    private bool _homingEnabled;
    [ShowIf(nameof(_homingEnabled))]
    [SerializeField]
    private float _homingTime;


    float aliveTime;
    float speedFactor;
    Vector2 bulletVel = Vector2.zero;
    Vector3 pos = Vector3.zero;
    bool isLaunched;


    void Update()
    {
        if (!isLaunched) return;
        // evaluate along curve to get value
        float curveValue = (_movementCurve.Evaluate(_curveEvaluationSpeed * aliveTime) * _curveEvaluationAmplitude);

        if (_curveAlongVelocityPath && _useMovementCurve) {
            // adjust position to follow curve
            pos += transform.right * Time.deltaTime * speedFactor;
            transform.position = pos + transform.up * curveValue;
        }
        else if(_useMovementCurve){
            // allows the movement to make cool patterns and cave in on itself with the correct settings. you can get flowers and stuff
            rb.velocity = bulletVel.normalized * speedFactor * curveValue;
        }
        else {
            rb.velocity = bulletVel.normalized * speedFactor;
        }

        // increment the alive timer
        aliveTime += Time.deltaTime;
    }

    /// <summary>
    /// Sets up all variables for the bullet
    /// </summary>
    /// <param name="vel"></param>
    public void Launch(Vector2 vel)
    {
        // cache the rigidbody locally
        rb = GetComponent<Rigidbody2D>();

        // set up the initial variables
        bulletVel = vel;
        speedFactor = bulletVel.magnitude;
        rb.velocity = bulletVel;
        pos = transform.position;
        isLaunched = true;

        StartCoroutine(EvaluateSpeedOverTime());
        // ensure the object gets destroyed. replace this with a pooling system later down the line
        Destroy(gameObject, _lifeTime);
    }

    private IEnumerator EvaluateSpeedOverTime()
    {
        float elapsedTime = 0.0f;
        float duration = _useLifetime ? _lifeTime : _speedCurveEvaluationTime;
        float curveValue = _speedOverTime.Evaluate(0.0f);

        while(elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            curveValue = _speedOverTime.Evaluate(elapsedTime / duration);

            speedFactor = Mathf.Lerp(speedFactor, 0, 1 - Mathf.Pow(1 - curveValue, Time.deltaTime));
            //speedFactor *= curveValue;
            yield return null;
        }

        yield return null;
    }
}
