using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnockbackArea : MonoBehaviour
{
    //knockback will be called for heavy 3rd attack, bombs

    public float knockBackStrength;

    public float knockBackRadius;

    private void OnCollisionEnter(Collision collision)
    {
        //AOE knockback
        Collider[] colliders = Physics.OverlapSphere(transform.position, knockBackRadius);
        Debug.Log("Collider has this many members " + colliders.Length);
        for (int i = 0; i < colliders.Length; i++)
        {
            Rigidbody rb = colliders[i].GetComponent<Rigidbody>();

            if (rb != null)
            {
                rb.AddExplosionForce(knockBackStrength, transform.position, knockBackRadius, 0f, ForceMode.Impulse);
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, knockBackRadius);
    }
}
