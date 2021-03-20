using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillMonsterMesh : MonoBehaviour
{
    private EnemyBase enemy;

    private void Start()
    {
        enemy = transform.parent.GetComponent<EnemyBase>();
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("KillBox"))
        {
            Destroy(transform.parent);
            //transform.parent.GetComponent<FloatingSkull>().Damage(100);
            Debug.Log("MONSTER TOUCHED KILLBOX");
            MonsterSpawner.Instance.MonsterKilledPrematurly();
        }
    }

    public void AttackEnded()
    {
        enemy.Anim.SetBool("isAttacking", false);
    }

}
