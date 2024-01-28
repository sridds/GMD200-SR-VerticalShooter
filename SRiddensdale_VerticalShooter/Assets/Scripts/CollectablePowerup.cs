using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectablePowerup : MonoBehaviour
{
    [SerializeField]
    private Powerup _powerup;

    [SerializeField]
    private AudioData _audioCollect;

    [SerializeField]
    private int _powerupPointValue = 50;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // call collect if player triggered the object
        if (collision.TryGetComponent<Player>(out Player player)) {
            // cannot collect two at once
            if (player.ActivePowerup != null) return;

            _powerup.Collect();
            AudioHandler.instance.ProcessAudioData(_audioCollect);
            GameManager.instance.AddPoints(_powerupPointValue);
            GameManager.instance.CreateSpecialText(_powerup.displayName);
            Destroy(gameObject);
        }
    }

    public void Despawn()
    {
        Destroy(gameObject);
    }
}
