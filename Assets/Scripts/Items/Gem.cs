using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gem : MonoBehaviour
{
    [SerializeField] private float spinSpeed;
    [SerializeField] private float disableColliderTime = .5f;

    //public CapsuleCollider collider;
    public CapsuleCollider trigger;

    private void Update()
    {
        transform.Rotate(0f, 10f * spinSpeed * Time.deltaTime, 0f);
    }

    private void Start()
    {
        StartCoroutine(HitboxCycle());
    }

    private IEnumerator HitboxCycle()
    {
        yield return new WaitForSeconds(disableColliderTime);
        //collider.enabled = true;
        trigger.enabled = true;
    }
}
