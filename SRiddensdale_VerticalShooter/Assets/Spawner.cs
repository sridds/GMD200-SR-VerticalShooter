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
    [SerializeField]
    [ShowIf(nameof(_useCurve))]
    private bool _onlyApplyCurveWhileFiring = true;

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
    [SerializeField, Tooltip("Determines whether the cone of fire will spin / aim while firing shots")]
    private bool _spinDuringFire;

    // private fields
    Quaternion angleRot = Quaternion.identity;

    private Coroutine activeFireCoroutine;
    private Transform target;
    private bool firing;

    private void Start()
    {
        // ensure curve is pingpong
        _curve.postWrapMode = WrapMode.PingPong;
        target = FindObjectOfType<PlayerMovement>().transform;
    }

    void Update()
    {
        // updates the sin angle and spin rate of the curve
        UpdateAngularMotion();
        // fires bullets
        Fire();
    }

    private void UpdateAngularMotion()
    {
        // spin rate has no effect if targetting something
        if(!_aimAtTarget) angleRot *= Quaternion.Euler(0, 0, _spinRate * Time.deltaTime);

        // apply curve rot
        if (_onlyApplyCurveWhileFiring && !firing) return;

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
        firing = true;
        float startAngle, currentAngle, angleStep;

        // an initial update of the fire cone to get the direction of each bullet
        UpdateCone(out startAngle, out currentAngle, out angleStep);

        for (int i = 0; i < _burstCount; i++)
        {
            for(int j = 0; j < _projectilesPerBurst; j++)
            {
                Vector2 pos = GetBulletSpawnPos(currentAngle);

                // fire a new bullet
                GameObject newBullet = Instantiate(_bulletPrefab, pos, Quaternion.identity).gameObject;
                newBullet.transform.right = newBullet.transform.position - transform.position;

                // update velocity of bullets
                if(newBullet.TryGetComponent<Bullet>(out Bullet bullet)) {
                    //rb.velocity = newBullet.transform.right * _bulletForce;
                    bullet.Launch(newBullet.transform.right * _bulletForce);
                }

                // increment current angle by the step
                currentAngle += angleStep;
            }

            currentAngle = startAngle;

            yield return new WaitForSeconds(_timeBetweenBursts);

            // continue updating the cone aim as shots fire
            if (_spinDuringFire) UpdateCone(out startAngle, out currentAngle, out angleStep);
        }

        firing = false;
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
