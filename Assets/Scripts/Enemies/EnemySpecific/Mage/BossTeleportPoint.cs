using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossTeleportPoint : MonoBehaviour
{
    public void DestroyTeleportPoint()
    {
        Destroy(gameObject, 1);
    }
}
