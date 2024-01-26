using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectablePowerup : MonoBehaviour
{
    [SerializeField]
    private Powerup _powerup;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // call collect if player triggered the object
        if (collision.TryGetComponent<Player>(out Player player)) {
            _powerup.Collect();

            Destroy(gameObject);
        }
    }
}