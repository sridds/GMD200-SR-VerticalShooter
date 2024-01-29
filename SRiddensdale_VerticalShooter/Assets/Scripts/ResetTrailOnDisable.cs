using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetTrailOnDisable : MonoBehaviour
{
    [SerializeField]
    private TrailRenderer trail;

    private void OnDisable()
    {
        trail.Clear();
        trail.emitting = false;
        trail.Clear();
    }

    private void OnEnable()
    {
        trail.Clear();
        trail.emitting = true;
    }
}
