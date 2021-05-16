using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ArcaneCircle : MonoBehaviour
{
    public GameObject teleportPoint;
    [HideInInspector] public bool playerOnCircle = false;
    [HideInInspector] public ArcaneGroup groupScript;

    private void Start()
    {
        groupScript = transform.parent.GetComponent<ArcaneGroup>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.GetComponent<PlayerController>())
        {
            playerOnCircle = true;
            other.GetComponent<PlayerController>().onArcaneCircle = this;
            HUDController.Instance.ShowQuickHint("Teleport");
            //Debug.Log("Player entered circle!");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<PlayerController>())
        {
            playerOnCircle = false;
            other.GetComponent<PlayerController>().onArcaneCircle = null;
            HUDController.Instance.HideQuickHint();
            //Debug.Log("Player exited circle!");
        }
    }

    public void TeleportBetweenCircles()
    {
        groupScript.TeleportPlayer();
    }
}
