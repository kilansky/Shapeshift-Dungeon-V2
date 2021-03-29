using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Something Hit");

        if (other.GetComponent<EnemyBase>())
        {
            Debug.Log("Monster Hit");
            other.GetComponent<EnemyBase>().Damage(PlayerController.Instance.CurrAttackDamage);
        }
    }
}
