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

    private void OnTriggerStay2D(Collider2D collision)
    {
        // Call the damagable to take damage while it remains in the trigger
        if(collision.TryGetComponent<IDamagable>(out IDamagable damagable))
        {
            damagable.TakeDamage(_damage);
        }
    }

    public void EnableBeam()
    {
        _collider.enabled = true;
        _beamActiveSource.Play();
        _beamParticle.Play();

        AudioHandler.instance.ProcessAudioData(_beamReleaseSound);
    }


    public void DisableBeam()
    {
        _collider.enabled = false;
        _beamActiveSource.Stop();
        _beamParticle.Stop();

        AudioHandler.instance.ProcessAudioData(_beamCloseSound);
    }
    
}
