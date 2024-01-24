using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Beam : MonoBehaviour
{
    [SerializeField]
    private int _damage;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.TryGetComponent<IDamagable>(out IDamagable damagable))
        {
            damagable.TakeDamage(_damage);
        }
    }
}
