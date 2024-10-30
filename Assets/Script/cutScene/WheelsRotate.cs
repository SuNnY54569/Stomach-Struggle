using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelsRotate : MonoBehaviour
{
    [SerializeField] private Vector3 rotationSpeed = new Vector3(0, 0, -50);
    void Update()
    {
        transform.Rotate(rotationSpeed * Time.deltaTime);
    }
}
