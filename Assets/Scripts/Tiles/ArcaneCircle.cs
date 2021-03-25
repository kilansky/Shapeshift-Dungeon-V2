using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ArcaneCircle : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Image arcaneCircle = GetComponent<Image>();
        arcaneCircle.color = Color.white;
    }

    private void OnTriggerExit(Collider other)
    {
        Image arcaneCircle = GetComponent<Image>();
        arcaneCircle.color = Color.black;
    }
}
