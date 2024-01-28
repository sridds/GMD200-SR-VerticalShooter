using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Performs similar to Deltarune's bullet grazing. Essentially adds points based on how close you get to a bullet
/// </summary>
public class BulletGrazer : MonoBehaviour
{
    [Header("Modifiers")]
    [SerializeField]
    private float _minGrazeDistance = 1.0f;
    [SerializeField]
    private float _grazeInterval = 0.25f;
    [SerializeField]
    private LayerMask _bulletGrazeLayer;
    [SerializeField]
    private int _grazePointValue = 25;

    [Header("References")]
    [SerializeField]
    private Player _player;
    [SerializeField]
    private SpriteRenderer _grazeVisual;

    [Header("Audio")]
    [SerializeField]
    private AudioData _grazeSound;

    private float grazeTimer;
    public delegate void Grazed();
    public Grazed OnGrazed;

    private void Update()
    {
        if (CanGraze()) Graze();

        // decrement timer
        if (grazeTimer > 0) grazeTimer -= Time.deltaTime;

        if(_grazeVisual.color.a > 0) _grazeVisual.color = new Color(_grazeVisual.color.r, _grazeVisual.color.g, _grazeVisual.color.b, _grazeVisual.color.a - 2 * Time.deltaTime);
    }

    private bool CanGraze()
    {
        // if the graze timer is still active, return false
        if (grazeTimer > 0) return false;
        if (!Physics2D.OverlapCircle(transform.position, _minGrazeDistance, _bulletGrazeLayer)) return false;
        if (_player.PlayerHealth.IFramesActive) return false;

        return true;
    }

    /// <summary>
    /// Invokes a graze event to all subscribers and resets the graze timer
    /// </summary>
    private void Graze()
    {
        grazeTimer = _grazeInterval;
        _grazeVisual.color = new Color(_grazeVisual.color.r, _grazeVisual.color.g, _grazeVisual.color.b, 0.6f);
        AudioHandler.instance.ProcessAudioData(_grazeSound);

        GameManager.instance.AddPoints(_grazePointValue);

        OnGrazed?.Invoke();
    }
}
