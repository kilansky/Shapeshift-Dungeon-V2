using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirePoints1 : MonoBehaviour
{
    public GameObject firePointFront;
    public GameObject firePointRight;
    public GameObject firePointLeft;

    private GameObject player;

    private void Start()
    {
        player = PlayerController.Instance.gameObject;
    }

    void Update()
    {
        //Rotate fire points toward the player
        Vector3 targetPoint = (player.transform.position + new Vector3(0, 1.5f, 0)) - firePointFront.transform.position;
        Quaternion targetRotation = Quaternion.LookRotation(targetPoint);
        firePointFront.transform.rotation = targetRotation;
        firePointRight.transform.rotation = targetRotation;
        firePointLeft.transform.rotation = targetRotation;

        //Rotate left and right fire points at an angle from the player
        firePointRight.transform.Rotate(0, 15f, 0);
        firePointLeft.transform.Rotate(0, -15f, 0);
    }
}
