using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathEvent : MonoBehaviour
{
    [SerializeField]
    private Health _healthListener;

    [SerializeField]
    private ParticleSystem _deathParticle;

    private void Start()
    {
        _healthListener.OnHealthDepleted += CallDeath;
    }

    private void CallDeath()
    {
        Instantiate(_deathParticle, transform.position, Quaternion.identity);

        Destroy(gameObject);
    }
}
