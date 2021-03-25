using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<EnemyBase>())
        {
            other.GetComponent<EnemyBase>().Damage(PlayerController.Instance.CurrAttackDamage);
        }
    }
}
