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

    public bool ready = true;

    [ShowIf(nameof(_spawnCall), SpawnCall.ControlledByScript)]
    [SerializeField]
    private bool _ignoreRestTime = false;

    // private fields
    Quaternion angleRot = Quaternion.identity;
    private Coroutine activeFireCoroutine;
    private Transform target;
    private bool firing;
    private ScriptableSpawner activeData;

    #region Powerups
    public void SwapoutData(ScriptableSpawner data) => activeData = data;
    public void ResetData() => activeData = _data;
    #endregion

    private void Start()
    {
        activeData = _data;
        // ensure curve is pingpong
        activeData.Curve.postWrapMode = WrapMode.PingPong;

        target = FindObjectOfType<PlayerMovement>().transform;
    }

    public void ReadyUp() => ready = true;
    public void NotReady() => ready = false;

    void Update()
    {
        // updates the sin angle and spin rate of the curve
        UpdateAngularMotion();
        // fires bullets
        if (_spawnCall == SpawnCall.ControlledByScript || !ready) return;
        Fire();
    }

    private void UpdateAngularMotion()
    {
        // spin rate has no effect if targetting something
        if(!activeData.AimAtTarget) angleRot *= Quaternion.Euler(0, 0, activeData.SpinRate * Time.deltaTime);

        // apply curve rot
        if (activeData.OnlyApplyCurveWhileFiring && !firing) return;

        if (activeData.UseCurve) angleRot *= Quaternion.Euler(0, 0, (activeData.Curve.Evaluate(Time.time * activeData.CurveSpeed) * Time.deltaTime * activeData.CurveAmplitude));
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

        int burstCount = activeData.ProjectilesPerBurst;
        if (activeData.ProjectilesPerBurst != 1)
        {
            burstCount = activeData.ProjectilesPerBurst - 1;
        }

        // an initial update of the fire cone to get the direction of each bullet
        UpdateCone(out startAngle, out currentAngle, out angleStep);

        for (int i = 0; i < activeData.BurstCount; i++)
        {
            yield return null;

            for (int j = 0; j < burstCount; j++)
            {
                yield return null;

                if(activeData.ProjectilesPerBurst == 1) {
                    currentAngle = 90;
                }

                // fire a new bullet
                GameObject newBullet = Instantiate(activeData.BulletPrefab, GetBulletSpawnPos(currentAngle), Quaternion.identity).gameObject;
                newBullet.transform.right = newBullet.transform.position - transform.position;

                // update velocity of bullets
                if(newBullet.TryGetComponent<Bullet>(out Bullet bullet)) {
                    //rb.velocity = newBullet.transform.right * _bulletForce;
                    bullet.Launch(newBullet.transform.right * activeData.BulletForce);
                }

                // increment current angle by the step
                currentAngle += angleStep;
            }

            currentAngle = startAngle;

            // play fire sound
            if(!activeData.PlayFireAfterBurst) AudioHandler.instance.ProcessAudioData(activeData.FireSound);

            yield return new WaitForSeconds(activeData.TimeBetweenBursts);

            // continue updating the cone aim as shots fire
            if (activeData.SpinDuringFire) UpdateCone(out startAngle, out currentAngle, out angleStep);
        }

        firing = false;

        // play fire sound
        if (activeData.PlayFireAfterBurst) AudioHandler.instance.ProcessAudioData(activeData.FireSound);

        //if (_ignoreRestTime)
        //{
            yield return new WaitForSeconds(activeData.RestTime);
        //}

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
        Vector2 targetDir = activeData.AimAtTarget ? dir : targetDir = angleRot * Vector2.up;
        float targetAngle = Mathf.Atan2(targetDir.y, targetDir.x) * Mathf.Rad2Deg;

        startAngle = targetAngle;
        float endAngle;
        currentAngle = 0.0f;
        float halfAngleSpread = 0.0f;
        angleStep = 0.0f;

        if (activeData.AngleSpread != 0)
        {
            if (activeData.ProjectilesPerBurst != 1)
                angleStep = activeData.AngleSpread / (activeData.ProjectilesPerBurst - 1);

            halfAngleSpread = activeData.AngleSpread / 2f;
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
        float x = transform.position.x + activeData.FireRadius * Mathf.Cos(currentAngle * Mathf.Deg2Rad);
        float y = transform.position.y + activeData.FireRadius * Mathf.Sin(currentAngle * Mathf.Deg2Rad);
        Vector2 pos = new Vector2(x, y);

        return pos;
    }
}
