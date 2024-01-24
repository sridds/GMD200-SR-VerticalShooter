using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    private SpriteRenderer _renderer;

    [SerializeField]
    private SpriteRenderer _graze;

    [SerializeField]
    private SpriteRenderer _thruster;

    [SerializeField]
    private PlayerMovement _movement;

    [Header("Ship Sprites")]
    [SerializeField]
    private Sprite _shipForward;

    [SerializeField]
    private Sprite _shipTiltLeft;

    [SerializeField]
    private Sprite _shipTiltRight;

    [SerializeField]
    private Sprite[] _thrusterSprites;

    [SerializeField]
    private float _thrusterAnimationInterval = 0.05f;

    private float thrusterAnimationTimer;
    private int index;

    void Update()
    {
        if(_movement.Velocity.x > 0.2f) {
            _renderer.sprite = _shipTiltRight;
        } else if(_movement.Velocity.x < -0.2f) {
            _renderer.sprite = _shipTiltLeft;
        }
        else {
            _renderer.sprite = _shipForward;
        }

        if(_movement.Velocity.y > 0.2f) {
            _thruster.enabled = true;

            if(thrusterAnimationTimer <= 0) {
                thrusterAnimationTimer = _thrusterAnimationInterval;

                _thruster.sprite = _thrusterSprites[index];
                index++;

                index = index % _thrusterSprites.Length - 1;
            }
        }
        else {
            _thruster.enabled = false;
        }

        _graze.sprite = _renderer.sprite;

        if (thrusterAnimationTimer > 0) thrusterAnimationTimer -= Time.deltaTime;
    }
}
