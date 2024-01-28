using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Supercharge : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    private Animator _animator;
    [SerializeField]
    private Animator _chargeAnimation;
    [SerializeField]
    private Beam _beam;
    [SerializeField]
    private AudioSource _chargeUpSource;
    [SerializeField]
    private AudioData _chargeReadySound;

    [Header("Properties")]
    [SerializeField]
    private float _chargeDuration;
    [SerializeField]
    private float _chargeCooldown;

    private bool chargeReady;
    private bool onCooldown;

    public bool IsChargingUp { get; private set; }
    public bool IsReleasingCharge { get; private set; }

    private void Update()
    {
        if (Input.GetKey(KeyCode.X) && !onCooldown && !GameManager.instance.IsGameOver) {
            _animator.SetBool("Charging", true);
            IsChargingUp = true;

            if (!_chargeUpSource.isPlaying) _chargeUpSource.Play();
        }
        else {
            _animator.SetBool("Charging", false);
            IsChargingUp = false;

            _chargeUpSource.Stop();
        }

        if(Input.GetKeyUp(KeyCode.X) && chargeReady)
        {
            chargeReady = false;
            IsReleasingCharge = true;
            onCooldown = true;
            StartCoroutine(ReleaseSupercharge());
        }

        // quick disable beam while game is over
        if (GameManager.instance.IsGameOver && IsReleasingCharge)
        {
            StopAllCoroutines();

            _chargeAnimation.SetTrigger("CloseBeam");
            _beam.DisableBeam();
            IsReleasingCharge = false;
        }
    }

    /// <summary>
    /// Called externally by an animation event to set charge ready to true
    /// </summary>
    public void ChargeReady() => chargeReady = true;

    private IEnumerator ReleaseSupercharge()
    {
        CameraShake.instance.Shake(0.8f, 0.35f);
        _beam.EnableBeam();
        _chargeAnimation.SetTrigger("OpenBeam");

        float elapsedTime = 0.0f;
        while(elapsedTime < _chargeDuration)
        {
            CameraShake.instance.Shake(0.1f, 0.2f);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        _chargeAnimation.SetTrigger("CloseBeam");
        _beam.DisableBeam();
        IsReleasingCharge = false;

        yield return new WaitForSeconds(_chargeCooldown);
        GameManager.instance.CreateSpecialText("CHARGE READY");
        AudioHandler.instance.ProcessAudioData(_chargeReadySound);

        onCooldown = false;
    }
}
