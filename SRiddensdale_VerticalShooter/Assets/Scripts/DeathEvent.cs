using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathEvent : MonoBehaviour
{
    [SerializeField]
    private Health _healthListener;

    [SerializeField]
    private ParticleSystem _deathParticle;

    [SerializeField]
    private AudioData _deathSound;

    [SerializeField]
    private int _deathPointValue = 100;

    private void Start()
    {
        _healthListener.OnHealthDepleted += CallDeath;
    }

    private void CallDeath()
    {
        // instantiate only if the death particle exists
        if(_deathParticle != null) ObjectPooler.SpawnObject(_deathParticle.gameObject, transform.position, Quaternion.identity, ObjectPooler.PoolType.ParticleSystem);

        AudioHandler.instance.ProcessAudioData(_deathSound);
        GameManager.instance.AddKills();

        // add points
        if(_deathPointValue > 0) GameManager.instance.AddPoints(_deathPointValue);

        Destroy(gameObject);
    }
}
