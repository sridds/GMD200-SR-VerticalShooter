using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReturnObjectToPool : MonoBehaviour
{
    [SerializeField]
    private float _time;


    // Calls the return to pool function when enabled
    void OnEnable() => Invoke(nameof(ReturnToPool), _time);

    void ReturnToPool() => ObjectPooler.ReturnObjectToPool(gameObject);
}
