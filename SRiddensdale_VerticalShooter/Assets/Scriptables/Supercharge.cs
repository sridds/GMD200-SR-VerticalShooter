using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Supercharge : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    private Animator _animator;

    private void Update()
    {
        if (Input.GetKey(KeyCode.X)) {
            _animator.SetBool("Charging", true);
        }
        else {
            _animator.SetBool("Charging", false);
        }
    }

    public void ChargeReady()
    {

    }
}
