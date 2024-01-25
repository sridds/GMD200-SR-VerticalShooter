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

    [Header("Graze Sprites")]
    [SerializeField]
    private Sprite _grazeForward;

    [SerializeField]
    private Sprite _grazeTiltLeft;

    [SerializeField]
    private Sprite _grazeTiltRight;

    [Header("Dash Trail")]
    [SerializeField]
    private SpriteRenderer _trailPrefab;

    [SerializeField]
    private float _trailInterval = 0.15f;

    [Header("Misc Sprites")]
    [SerializeField]
    private Sprite[] _thrusterSprites;

    [SerializeField]
    private float _thrusterAnimationInterval = 0.05f;

    private float thrusterAnimationTimer;
    private int index;

    private void Start()
    {
        _movement.OnDashStart += EnableDashTrail;
        _movement.OnDashEnd += DisableDashTrail;
    }

    void Update()
    {
        // tilting right
        if(_movement.Velocity.x > 0.2f) {
            _renderer.sprite = _shipTiltRight;
            _graze.sprite = _grazeTiltRight;
        // tilting left
        } else if(_movement.Velocity.x < -0.2f) {
            _renderer.sprite = _shipTiltLeft;
            _graze.sprite = _grazeTiltLeft;
        }
        // forward
        else {
            _renderer.sprite = _shipForward;
            _graze.sprite = _grazeForward;
        }

        // enable thrusters
        if(_movement.Velocity.y > 0.2f) {
            _thruster.enabled = true;

            if(thrusterAnimationTimer <= 0) {
                thrusterAnimationTimer = _thrusterAnimationInterval;

                _thruster.sprite = _thrusterSprites[index];
                index++;

                index = index % (_thrusterSprites.Length);
            }
        }
        else {
            _thruster.enabled = false;
        }

        if (thrusterAnimationTimer > 0) thrusterAnimationTimer -= Time.deltaTime;
    }


    private void EnableDashTrail()
    {
        dashTrailActive = true;
        StartCoroutine(DashTrail());
    }

    private void DisableDashTrail()
    {
        dashTrailActive = false;
    }

    bool dashTrailActive;
    private IEnumerator DashTrail()
    {
        while (dashTrailActive)
        {
            SpriteRenderer r = Instantiate(_trailPrefab, transform.position, Quaternion.identity);
            r.sprite = _renderer.sprite;

            yield return new WaitForSeconds(_trailInterval);
        }
    }
}
