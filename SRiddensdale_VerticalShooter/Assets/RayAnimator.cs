using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayAnimator : MonoBehaviour
{
    private Spawner _mySpawner;
    private Animator _animator;

    void Start()
    {
        _mySpawner = GetComponent<Spawner>();
        _animator = GetComponent<Animator>();
    }

    private void Update()
    {
        _animator.SetBool("Open", _mySpawner.Firing);
    }
}
