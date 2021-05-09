using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Minimap : MonoBehaviour
{
    [Range(0f, 1f)] public float mapOpacity;

    void Update()
    {
        //GetComponent<RawImage>().material.color = new Color(1, 1, 1, mapOpacity);
    }
}
