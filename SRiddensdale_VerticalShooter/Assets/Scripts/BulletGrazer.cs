using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Performs similar to Deltarune's bullet grazing. Essentially adds points based on how close you get to a bullet
/// </summary>
public class BulletGrazer : MonoBehaviour
{
    [SerializeField]
    private float _minGrazeDistance = 1.0f;
    [SerializeField]
    private float _grazeInterval = 0.25f;
    [SerializeField]
    private LayerMask _bulletGrazeLayer;

    private float grazeTimer;
    public delegate void Grazed();
    public Grazed OnGrazed;

    private void Update()
    {
        if (CanGraze()) Graze();

        // decrement timer
        if (grazeTimer > 0) grazeTimer -= Time.deltaTime;
    }

    private bool CanGraze()
    {
        // if the graze timer is still active, return false
        if (grazeTimer > 0) return false;
        if (!Physics2D.OverlapCircle(transform.position, _minGrazeDistance, _bulletGrazeLayer)) return false;
        return true;
    }

    /// <summary>
    /// Invokes a graze event to all subscribers and resets the graze timer
    /// </summary>
    private void Graze()
    {
        grazeTimer = _grazeInterval;
        Debug.Log("Graze!");

        OnGrazed?.Invoke();
    }
}
