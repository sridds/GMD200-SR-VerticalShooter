using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReturnObjectToPool : MonoBehaviour
{
    [SerializeField]
    private float _time;

    // Start is called before the first frame update
    void OnEnable()
    {
        Invoke(nameof(ReturnToPool), _time);
    }

    void ReturnToPool() => ObjectPooler.ReturnObjectToPool(gameObject);
}
