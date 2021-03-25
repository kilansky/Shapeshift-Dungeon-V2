using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newAttackStateData", menuName = "Data/State Data/Attack State")]

public class D_AttackState : ScriptableObject
{
    //what is the individual enemies attack range
    public float attackRange;

    public float chargeTime = 1f;
}
