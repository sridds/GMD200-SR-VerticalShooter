using System.Collections;
using System.Collections.Generic;
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
    private bool releasingCharge;

    private void Update()
    {
        if (Input.GetKey(KeyCode.X) && !releasingCharge) {
            _animator.SetBool("Charging", true);
        }
        else {
            _animator.SetBool("Charging", false);
        }

        if(Input.GetKeyUp(KeyCode.X) && chargeReady)
        {
            chargeReady = false;
            releasingCharge = true;
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
        yield return new WaitForSeconds(_chargeCooldown);
        _chargeAnimation.gameObject.SetActive(false);

        releasingCharge = false;
    }
}
