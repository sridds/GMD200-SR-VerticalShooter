using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Beam : MonoBehaviour
{
    [SerializeField]
    private int _damage;

    [SerializeField]
    private BoxCollider2D _collider;

    private void OnTriggerStay2D(Collider2D collision)
    {
        // Call the damagable to take damage while it remains in the trigger
        if(collision.TryGetComponent<IDamagable>(out IDamagable damagable))
        {
            damagable.TakeDamage(_damage);
        }
    }

    public void EnableBeam() => _collider.enabled = true;

    public void DisableBeam() => _collider.enabled = false;
}
