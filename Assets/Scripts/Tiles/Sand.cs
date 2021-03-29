using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sand : MonoBehaviour
{
    public float slowPercent = 20f;
    public BoxCollider triggerBox;

    private float actualSlowValue;

    private void Start()
    {
        actualSlowValue = slowPercent / 100f;
    }

    private void OnTriggerStay(Collider other)
    {
        if(other.gameObject == PlayerController.Instance.gameObject)
        {
            //Reduce player speed
            Debug.Log("Player is on sand");
            PlayerController.Instance.SandSpeedMod = 1 - (slowPercent / 100);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == PlayerController.Instance.gameObject)
        {
            //Set player speed back
            Debug.Log("Player has exited sand");
            PlayerController.Instance.SandSpeedMod = 1;
        }
    }
}
