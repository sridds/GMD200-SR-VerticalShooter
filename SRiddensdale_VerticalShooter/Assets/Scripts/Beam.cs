using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Beam : MonoBehaviour
{
    [SerializeField]
    private int _damage;

    [SerializeField]
    private AudioData _beamReleaseSound;

    [SerializeField]
    private AudioData _beamCloseSound;

    [SerializeField]
    private AudioSource _beamActiveSource;

    [SerializeField]
    private BoxCollider2D _collider;

    [SerializeField]
    private ParticleSystem _beamParticle;

    private bool beamEnabled;

    private void Start()
    {
        // subscribe to the game state event to handle the beam during pause
        GameManager.instance.OnGameStateChanged += GameStateChanged;
    }

    /// <summary>
    /// Updates the beam to behave properly while the game is paused
    /// </summary>
    /// <param name="state"></param>
    private void GameStateChanged(GameManager.GameState state)
    {
        if (!beamEnabled) return;

        // play or stop sound
        if (state == GameManager.GameState.Paused) _beamActiveSource.Stop();
        else if (state != GameManager.GameState.Paused) _beamActiveSource.Play();
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        // Call the damagable to take damage while it remains in the trigger
        if(collision.TryGetComponent<IDamagable>(out IDamagable damagable))
        {
            damagable.TakeDamage(_damage);
        }
    }

    /// <summary>
    /// Calls the beam to be enabled
    /// </summary>
    public void EnableBeam()
    {
        _collider.enabled = true;
        _beamActiveSource.Play();
        _beamParticle.Play();
        beamEnabled = true;
        
        AudioHandler.instance.ProcessAudioData(_beamReleaseSound);
    }

    /// <summary>
    /// Calls the beam to be disabled
    /// </summary>
    public void DisableBeam()
    {
        _collider.enabled = false;
        _beamActiveSource.Stop();
        _beamParticle.Stop();
        beamEnabled = false;

        AudioHandler.instance.ProcessAudioData(_beamCloseSound);
    }
    
}
