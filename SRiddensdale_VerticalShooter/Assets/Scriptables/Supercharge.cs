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
        if (Input.GetKey(KeyCode.X) && !onCooldown) {
            _animator.SetBool("Charging", true);
            IsChargingUp = true;
        }
        else {
            _animator.SetBool("Charging", false);
            IsChargingUp = false;
        }

        if(Input.GetKeyUp(KeyCode.X) && chargeReady)
        {
            chargeReady = false;
            IsReleasingCharge = true;
            onCooldown = true;
            StartCoroutine(ReleaseSupercharge());
        }
    }

    /// <summary>
    /// Called externally by an animation event to set charge ready to true
    /// </summary>
    public void ChargeReady() => chargeReady = true;

    private IEnumerator ReleaseSupercharge()
    {
        CameraShake.instance.Shake(0.8f, 0.35f);
        _chargeAnimation.gameObject.SetActive(true);

        float elapsedTime = 0.0f;
        while(elapsedTime < _chargeDuration)
        {
            CameraShake.instance.Shake(0.1f, 0.2f);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        _chargeAnimation.SetTrigger("CloseBeam");
        IsReleasingCharge = false;

        yield return new WaitForSeconds(_chargeCooldown);
        _chargeAnimation.gameObject.SetActive(false);

        onCooldown = false;
    }
}
