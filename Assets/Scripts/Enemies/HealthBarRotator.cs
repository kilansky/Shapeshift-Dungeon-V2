using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBarRotator : MonoBehaviour
{
    void Update()
    {
        transform.LookAt(transform.position + new Vector3(0, -4, 10), Vector3.up);
    }
}
