using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gem : MonoBehaviour
{
    public float spinSpeed;

    private void Update()
    {
        transform.Rotate(0f, 10f * spinSpeed * Time.deltaTime, 0f);
    }
}
