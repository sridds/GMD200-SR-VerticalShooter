using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleAnimations : MonoBehaviour
{
    [SerializeField]
    private float _sinAmt;
    [SerializeField]
    private float _cosAmt;

    [SerializeField]
    private float _sinSpeed;
    [SerializeField]
    private float _cosSpeed;

    Vector2 initialPos = Vector2.zero;
    Vector3 unitMovement;

    private float _aliveTime;

    private void Start()
    {
        initialPos = transform.localPosition;
    }

    void Update()
    {
        unitMovement.x = initialPos.x + Mathf.Cos(_cosSpeed * _aliveTime) * _cosAmt;
        unitMovement.y = initialPos.y + Mathf.Sin(_sinSpeed * _aliveTime) * _sinAmt;

        transform.localPosition += unitMovement * Time.deltaTime;

        _aliveTime += Time.deltaTime;
    }
}
