using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("Bullet Settings")]
    [SerializeField]
    private float _lifeTime;

    private void Start()
    {
        Destroy(gameObject, _lifeTime);
    }

    void Update()
    {
        
    }
}
