using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmergencyPit : MonoBehaviour
{
    //If the player ever goes below the map and touches this, teleport them to the center tile
    private void OnTriggerEnter(Collider other)
    {
        Vector3 centerTilePos = new Vector3(0, 5, 0);

        FindSafeTile.Instance.safePos = centerTilePos;

        if (other.GetComponent<PlayerController>())
            FindSafeTile.Instance.TeleportPlayer(centerTilePos);
    }
}
