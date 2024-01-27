using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spin : MonoBehaviour
{
    [SerializeField]
    private float _speed;
    void Update()
    {
        transform.Rotate(new Vector3(0, 0, 20) * Time.deltaTime * _speed);
    }
}
