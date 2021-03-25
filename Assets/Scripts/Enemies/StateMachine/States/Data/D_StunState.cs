using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newStunStateData", menuName = "Data/State Data/Stun State")]

public class D_StunState : ScriptableObject
{
    public float stunTime = 1f;

    //TODO: double check about removing these and adding to knockback manager
    public float stunKnockbackTime = .3f;
    public float stunKnockbackSpeed = 10f;
    public Vector3 stunKnockbackAngle;
}
