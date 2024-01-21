using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [Header("Bullet Settings")]
    [SerializeField]
    private float _bulletForce;

    [SerializeField]
    private Bullet _bulletPrefab;

    [Header("Fire Motion")]
    [SerializeField]
    private float _spinRate;

    [SerializeField]
    private bool _useCurve;

    [SerializeField, Tooltip("Determines how much sin is added to the spin angle")]
    [ShowIf(nameof(_useCurve))]
    private float _curveSpeed;

    [SerializeField]
    [ShowIf(nameof(_useCurve))]
    private float _curveAmplitude;

    [SerializeField]
    [ShowIf(nameof(_useCurve))]
    private AnimationCurve _curve = new AnimationCurve(new Keyframe(0, -1), new Keyframe(1, 1));
    Quaternion targetRot = Quaternion.identity;

    private void Start()
    {
        // ensure curve is pingpong
        _curve.postWrapMode = WrapMode.PingPong;
    }

    void Update()
    {
        targetRot *= Quaternion.Euler(0, 0, _spinRate * Time.deltaTime);

        // apply curve rot
        if(_useCurve) targetRot *= Quaternion.Euler(0, 0, (_curve.Evaluate(Time.time * _curveSpeed) * Time.deltaTime * _curveAmplitude));

        Vector2 targetForward = targetRot * Vector2.up;
        Debug.DrawRay(transform.position, targetForward * 15.0f, Color.blue);
        Debug.DrawRay(transform.position, Vector2.up * 15.0f, Color.green);
    }
}
