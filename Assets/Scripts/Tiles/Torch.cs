using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Torch : MonoBehaviour
{
    // Start is called before the first frame update
    public void ResetRotation()
    {
        if (transform.parent.GetComponent<TileRotator>())
            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y - transform.parent.rotation.eulerAngles.y, transform.eulerAngles.z);
    }
}
