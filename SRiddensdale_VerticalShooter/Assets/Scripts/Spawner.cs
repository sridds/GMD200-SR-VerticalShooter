using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Thanks to this video for helping out with the cone of influence
/// https://www.youtube.com/watch?v=OxDMHn1o0LA&ab_channel=StephenHubbard
/// </summary>

public class Spawner : MonoBehaviour
{
    public enum SpawnCall
    {
        Automatic,
        ControlledByScript,
    }

    [SerializeField]
    private ScriptableSpawner _data;
    [SerializeField, Tooltip("Determines whether the spawn is handled by this script or by another script. This allows flexibility for enemies and player")]
    private SpawnCall _spawnCall;

    // private fields
    Quaternion angleRot = Quaternion.identity;

    private Coroutine activeFireCoroutine;
    private Transform target;
    private bool firing;

    private void Start()
    {
        // ensure curve is pingpong
        _data.Curve.postWrapMode = WrapMode.PingPong;
        target = FindObjectOfType<PlayerMovement>().transform;
    }

    void Update()
    {
        // updates the sin angle and spin rate of the curve
        UpdateAngularMotion();
        // fires bullets
        if (_spawnCall == SpawnCall.ControlledByScript) return;
        Fire();
    }

    private void UpdateAngularMotion()
    {
        // spin rate has no effect if targetting something
        if(!_data.AimAtTarget) angleRot *= Quaternion.Euler(0, 0, _data.SpinRate * Time.deltaTime);

        // apply curve rot
        if (_data.OnlyApplyCurveWhileFiring && !firing) return;

        if (_data.UseCurve) angleRot *= Quaternion.Euler(0, 0, (_data.Curve.Evaluate(Time.time * _data.CurveSpeed) * Time.deltaTime * _data.CurveAmplitude));
    }

    public void Fire()
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

        for (int i = 0; i < _data.BurstCount; i++)
        {
            for(int j = 0; j < _data.ProjectilesPerBurst; j++)
            {
                if(_data.ProjectilesPerBurst == 1) {
                    currentAngle = 90;
                }
                Vector2 pos = GetBulletSpawnPos(currentAngle);

                // fire a new bullet
                GameObject newBullet = Instantiate(_data.BulletPrefab, pos, Quaternion.identity).gameObject;
                newBullet.transform.right = newBullet.transform.position - transform.position;

                // update velocity of bullets
                if(newBullet.TryGetComponent<Bullet>(out Bullet bullet)) {
                    //rb.velocity = newBullet.transform.right * _bulletForce;
                    bullet.Launch(newBullet.transform.right * _data.BulletForce);
                }

                // increment current angle by the step
                currentAngle += angleStep;
            }

            currentAngle = startAngle;

            yield return new WaitForSeconds(_data.TimeBetweenBursts);

            // continue updating the cone aim as shots fire
            if (_data.SpinDuringFire) UpdateCone(out startAngle, out currentAngle, out angleStep);
        }

        firing = false;
        yield return new WaitForSeconds(_data.RestTime);

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
        Vector2 targetDir = _data.AimAtTarget ? dir : targetDir = angleRot * Vector2.up;
        float targetAngle = Mathf.Atan2(targetDir.y, targetDir.x) * Mathf.Rad2Deg;

        startAngle = targetAngle;
        float endAngle;
        currentAngle = 0.0f;
        float halfAngleSpread = 0.0f;
        angleStep = 0.0f;

        if (_data.AngleSpread != 0)
        {
            if (_data.ProjectilesPerBurst != 1)
                angleStep = _data.AngleSpread / (_data.ProjectilesPerBurst - 1);

            halfAngleSpread = _data.AngleSpread / 2f;
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
        float x = transform.position.x + _data.FireRadius * Mathf.Cos(currentAngle * Mathf.Deg2Rad);
        float y = transform.position.y + _data.FireRadius * Mathf.Sin(currentAngle * Mathf.Deg2Rad);
        Vector2 pos = new Vector2(x, y);

        return pos;
    }
}
