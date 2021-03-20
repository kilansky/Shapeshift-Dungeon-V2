using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileRotator : MonoBehaviour
{
    void Start()
    {
        transform.Rotate(0, 90 * Random.Range(0, 4), 0);
    }
}
