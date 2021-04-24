using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcaneGroup : MonoBehaviour
{
    public float cooldown = 5f;
    public GameObject circleOne;
    public GameObject circleTwo;

    private GameObject player;
    private FindSafeTile teleportScript;
    private GameObject target;
    private bool onCooldown = false;

    private void Start()
    {
        player = PlayerController.Instance.gameObject;
        teleportScript = player.GetComponent<FindSafeTile>();
    }

    [ContextMenu("Test Teleport")]
    public void TeleportPlayer()
    {  
        if(onCooldown)
        {
            Debug.Log("Teleporters on cooldown");
            return;
        }

        if(circleOne.GetComponent<ArcaneCircle>().playerOnCircle)
        {
            target = circleTwo;
        }
        else if(circleTwo.GetComponent<ArcaneCircle>().playerOnCircle)
        {
            target = circleOne;
        }
        else
        {
            Debug.LogWarning("Player not on arcane circles!");
        }

        Debug.Log("Target: " + target);
        if(target != null)
        {
            teleportScript.TeleportPlayer(target.GetComponent<ArcaneCircle>().teleportPoint.transform.position);
            StartCoroutine(DoCooldown());
            if(target == circleOne)
            {
                circleTwo.GetComponent<ArcaneCircle>().playerOnCircle = false;
            }
            if (target == circleTwo)
            {
                circleOne.GetComponent<ArcaneCircle>().playerOnCircle = false;
            }
        }           
        target = null;
    }

    private IEnumerator DoCooldown()
    {
        onCooldown = true;
        yield return new WaitForSeconds(cooldown);
        onCooldown = false;
    }

    public void DestroyGroup()
    {
        Destroy(gameObject);
    }
}
