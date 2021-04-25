﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ArcaneCircle : MonoBehaviour
{
    public GameObject teleportPoint;
    public bool playerOnCircle = false;
    [HideInInspector]public ArcaneGroup groupScript;

    private void Start()
    {
        groupScript = transform.parent.GetComponent<ArcaneGroup>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.GetComponent<PlayerController>())
        {
            playerOnCircle = true;
            groupScript.TeleportPlayer();
            //Debug.Log("Player entered circle!");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<PlayerController>())
        {
            playerOnCircle = false;
            //Debug.Log("Player exited circle!");
        }
    }
}
