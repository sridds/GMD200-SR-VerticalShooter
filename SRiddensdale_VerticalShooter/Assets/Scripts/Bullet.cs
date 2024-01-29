using UnityEngine;
using NaughtyAttributes;
using System.Collections;

public class Bullet : MonoBehaviour, IDamagable
{
    private Rigidbody2D rb;

    [Header("Bullet Settings")]
    [SerializeField]
    private float _lifeTime;
    [SerializeField]
    private int _damageAmt;

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
    private bool _doSpeedOverCurve;
    [ShowIf(nameof(_doSpeedOverCurve))]
    [SerializeField]
    private AnimationCurve _speedOverTime;
    [HideIf(nameof(_useLifetime))]
    [SerializeField]
    private float _speedCurveEvaluationTime;
    [ShowIf(nameof(_doSpeedOverCurve))]
    [SerializeField]
    private bool _useLifetime;
    [ShowIf(nameof(_doSpeedOverCurve))]
    [SerializeField, Tooltip("This will be the resulting speed after applying the speed curve")]
    private float _targetEndSpeed;

    [Header("Homing")]
    [SerializeField]
    private bool _homingEnabled;
    [ShowIf(nameof(_homingEnabled))]
    [SerializeField]
    private float _homingTime;
    [ShowIf(nameof(_homingEnabled))]
    [SerializeField]
    private float _homingStrength;

    [Header("Damage Settings")]
    [SerializeField]
    private bool _allowDamage;
    [SerializeField]
    private AudioData _bulletDestroySound;
    [SerializeField]
    private ParticleSystem _bulletBreakParticle;
    [SerializeField]
    private int _destroyPointValue = 25;

    private float aliveTime;
    private float speedFactor;
    private Vector2 dir = Vector2.zero;
    private Vector3 pos = Vector3.zero;
    private bool isLaunched;
    private Transform target;

    void Update()
    {
        if (!isLaunched) return;
        // evaluate along curve to get value
        float curveValue = (_movementCurve.Evaluate(_curveEvaluationSpeed * aliveTime) * _curveEvaluationAmplitude);

        // this if statement is rough and dirty but it works :/
        if (_curveAlongVelocityPath && _useMovementCurve) {
            // adjust position to follow curve
            pos += transform.right * Time.deltaTime * speedFactor;
            transform.position = pos + transform.up * curveValue;
        }
        else if(_useMovementCurve && !_homingEnabled){
            // allows the movement to make cool patterns and cave in on itself with the correct settings. you can get flowers and stuff
            rb.velocity = dir * speedFactor * curveValue;
        }
        else if(!_homingEnabled){
            rb.velocity = dir * speedFactor;
        }

        // increment the alive timer
        aliveTime += Time.deltaTime;
    }

    private void FixedUpdate()
    {
        if (_homingEnabled && aliveTime < _homingTime)
        {
            HomeInOnTarget();
        }
    }

    /// <summary>
    /// Homes in the object towards the target
    /// </summary>
    private void HomeInOnTarget()
    {
        Vector2 direction = (Vector2)target.position - rb.position;
        direction.Normalize();

        float rot = Vector3.Cross(direction, transform.right).z;
        rb.angularVelocity = -rot * _homingStrength;

        rb.velocity = transform.right * speedFactor;
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
        dir = vel.normalized;
        speedFactor = vel.magnitude;

        rb.velocity = vel;
        pos = transform.position;
        isLaunched = true;

        if(target == null) target = FindObjectOfType<PlayerMovement>().transform;

        // must be enabled before evaluating speed
        if(_doSpeedOverCurve) StartCoroutine(EvaluateSpeedOverTime());
        // ensure the object gets destroyed. replace this with a pooling system later down the line

        //Destroy(gameObject, _lifeTime);
        Invoke(nameof(ReturnToPool), _lifeTime);
    }

    private void ReturnToPool()
    {
        aliveTime = 0.0f;
        rb.velocity = Vector2.zero;
        isLaunched = false;

        ObjectPooler.ReturnObjectToPool(gameObject);
    }

    /// <summary>
    /// As the bullet continues to live, change the speed factor based on the curve.
    /// </summary>
    /// <returns></returns>
    private IEnumerator EvaluateSpeedOverTime()
    {
        float elapsedTime = 0.0f;
        // determine the duration based on whether the bullet relies on lifeTime for evaluating speed
        float duration = _useLifetime ? _lifeTime : _speedCurveEvaluationTime;
        float curveValue = _speedOverTime.Evaluate(0.0f);

        while(elapsedTime < duration)
        {
            // increment timer and continue evaluating the curve
            elapsedTime += Time.deltaTime;
            curveValue = _speedOverTime.Evaluate(elapsedTime / duration);

            // use an exponential equation rather than just lerping normally bc frame dependency
            speedFactor = Mathf.Lerp(speedFactor, _targetEndSpeed, 1 - Mathf.Pow(1 - curveValue, Time.deltaTime));
            yield return null;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // take damage when hitting damagable
        if(collision.TryGetComponent<IDamagable>(out IDamagable damagable))
        {
            damagable.TakeDamage(_damageAmt);
            GameManager.instance.AddPoints(_destroyPointValue);

            Destroy(gameObject);
        }
    }

    public void TakeDamage(int damageAmount)
    {
        if (!_allowDamage) return;

        Destroy(gameObject);
        AudioHandler.instance.ProcessAudioData(_bulletDestroySound);
        CameraShake.instance.Shake(0.3f, 0.15f);

        if (_bulletBreakParticle != null) ObjectPooler.SpawnObject(_bulletBreakParticle.gameObject, transform.position, Quaternion.identity, ObjectPooler.PoolType.ParticleSystem);
    }
}
