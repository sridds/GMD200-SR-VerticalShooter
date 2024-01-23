using UnityEngine;
using NaughtyAttributes;

[CreateAssetMenu(fileName = "SpawnerData", menuName = "CustomObjects/Spawner", order = 1)]
public class ScriptableSpawner : ScriptableObject
{
    [Header("Bullet Settings")]
    public float BulletForce;
    public Bullet BulletPrefab;

    [Header("Fire Motion")]
    public bool UseCurve;
    [HideIf(nameof(AimAtTarget))]
    public float SpinRate;

    [Header("Curve Settings")]
    [Tooltip("Determines how much sin is added to the spin angle")]
    [ShowIf(nameof(UseCurve))]
    public float CurveSpeed;
    [ShowIf(nameof(UseCurve))]
    public float CurveAmplitude;
    [ShowIf(nameof(UseCurve))]
    public AnimationCurve Curve = new AnimationCurve(new Keyframe(0, -1), new Keyframe(1, 1));
    [ShowIf(nameof(UseCurve))]
    public bool OnlyApplyCurveWhileFiring = true;

    [Header("Fire Settings")]
    public bool AimAtTarget;
    [Min(0)]
    public float FireRadius;
    [Min(0)]
    public int BurstCount;
    [Min(0)]
    public int ProjectilesPerBurst;
    [Range(0, 360)]
    public int AngleSpread;
    [Min(0)]
    public float TimeBetweenBursts;
    [Min(0)]
    public float RestTime;
    [Tooltip("Determines whether the cone of fire will spin / aim while firing shotsw ")]
    public bool SpinDuringFire;
}
