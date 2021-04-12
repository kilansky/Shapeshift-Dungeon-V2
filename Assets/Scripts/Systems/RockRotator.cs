using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockRotator : MonoBehaviour
{
    void Start()
    {
        //Rotates object randomly on the x, y, & z axis
        transform.Rotate(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360));
    }
}
