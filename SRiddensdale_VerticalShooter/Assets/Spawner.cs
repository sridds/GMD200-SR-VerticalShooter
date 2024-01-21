using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

/// <summary>
/// Thanks to this video for helping out with the cone of influence
/// https://www.youtube.com/watch?v=OxDMHn1o0LA&ab_channel=StephenHubbard
/// </summary>

public class Spawner : MonoBehaviour
{
    [Header("Bullet Settings")]
    [SerializeField]
    private float _bulletForce;
    [SerializeField]
    private Bullet _bulletPrefab;

    [Header("Fire Motion")]
    [HideIf(nameof(_aimAtTarget))]
    [SerializeField]
    private float _spinRate;
    [SerializeField]
    private bool _useCurve;

    [Header("Curve Settings")]
    [SerializeField, Tooltip("Determines how much sin is added to the spin angle")]
    [ShowIf(nameof(_useCurve))]
    private float _curveSpeed;
    [SerializeField]
    [ShowIf(nameof(_useCurve))]
    private float _curveAmplitude;
    [SerializeField]
    [ShowIf(nameof(_useCurve))]
    private AnimationCurve _curve = new AnimationCurve(new Keyframe(0, -1), new Keyframe(1, 1));

    [Header("Fire Settings")]
    [SerializeField]
    private bool _aimAtTarget;
    [SerializeField, Min(0)]
    private float _radius;
    [SerializeField, Min(0)]
    private int _burstCount;
    [SerializeField, Min(0)]
    private int _projectilesPerBurst;
    [SerializeField, Range(0, 360)]
    private int _angleSpread;
    [SerializeField, Min(0)]
    private float _timeBetweenBursts;
    [SerializeField, Min(0)]
    private float _restTime;

    // private fields
    Quaternion angleRot = Quaternion.identity;

    private Coroutine activeFireCoroutine;
    private Transform target;

    private void Start()
    {
        // ensure curve is pingpong
        _curve.postWrapMode = WrapMode.PingPong;
        target = FindObjectOfType<PlayerMovement>().transform;
    }

    void Update()
    {
        UpdateAngularMotion();
        Fire();

        Vector2 targetForward = angleRot * Vector2.up;

        // for debug purposes just to show the direction
        Debug.DrawRay(transform.position, targetForward * 15.0f, Color.blue);
        Debug.DrawRay(transform.position, Vector2.up * 15.0f, Color.green);
    }

    private void UpdateAngularMotion()
    {
        // spin rate has no effect if targetting something
        if(!_aimAtTarget) angleRot *= Quaternion.Euler(0, 0, _spinRate * Time.deltaTime);

        // apply curve rot
        if (_useCurve) angleRot *= Quaternion.Euler(0, 0, (_curve.Evaluate(Time.time * _curveSpeed) * Time.deltaTime * _curveAmplitude));
    }

    private void Fire()
    {
        if (activeFireCoroutine != null) return;

        // start fire coroutine
        activeFireCoroutine = StartCoroutine(FireBullets());
    }

    private IEnumerator FireBullets()
    {
        float startAngle, currentAngle, angleStep;
        UpdateCone(out startAngle, out currentAngle, out angleStep);

        for (int i = 0; i < _burstCount; i++)
        {
            for(int j = 0; j < _projectilesPerBurst; j++)
            {
                Vector2 pos = GetBulletSpawnPos(currentAngle);

                GameObject newBullet = Instantiate(_bulletPrefab, pos, Quaternion.identity).gameObject;
                newBullet.transform.right = newBullet.transform.position - transform.position;

                // update velocity of bullets
                if(newBullet.TryGetComponent<Rigidbody2D>(out Rigidbody2D rb)) {
                    rb.velocity = newBullet.transform.right * _bulletForce;
                }

                currentAngle += angleStep;
            }

            currentAngle = startAngle;

            yield return new WaitForSeconds(_timeBetweenBursts);
            UpdateCone(out startAngle, out currentAngle, out angleStep);
        }


        yield return new WaitForSeconds(_restTime);

        activeFireCoroutine = null;
    }

    /// <summary>
    /// Calculates the new angle according the the target
    /// </summary>
    /// <param name="startAngle"></param>
    /// <param name="currentAngle"></param>
    /// <param name="angleStep"></param>
    private void UpdateCone(out float startAngle, out float currentAngle, out float angleStep)
    {
        Vector2 dir = angleRot * (target.transform.position - transform.position);

        // get target direciton based on whether aim at target is selected
        Vector2 targetDir = _aimAtTarget ? dir : targetDir = angleRot * Vector2.up;
        float targetAngle = Mathf.Atan2(targetDir.y, targetDir.x) * Mathf.Rad2Deg;

        startAngle = targetAngle;
        float endAngle;
        currentAngle = 0.0f;
        float halfAngleSpread = 0.0f;
        angleStep = 0.0f;

        if (_angleSpread != 0)
        {
            angleStep = _angleSpread / (_projectilesPerBurst - 1);
            halfAngleSpread = _angleSpread / 2f;
            startAngle = targetAngle - halfAngleSpread;
            endAngle = targetAngle + halfAngleSpread;
            currentAngle = startAngle;
        }
    }

    /// <summary>
    /// Gets the spawn position of the bullet based on the radius around the spawner
    /// </summary>
    /// <param name="currentAngle"></param>
    /// <returns></returns>
    private Vector2 GetBulletSpawnPos(float currentAngle)
    {
        // define x and y values based on sin and cos of current angle 
        float x = transform.position.x + _radius * Mathf.Cos(currentAngle * Mathf.Deg2Rad);
        float y = transform.position.y + _radius * Mathf.Sin(currentAngle * Mathf.Deg2Rad);
        Vector2 pos = new Vector2(x, y);

        return pos;
    }
}
