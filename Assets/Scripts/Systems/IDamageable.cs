using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
    /**
     * Script: IDamageable
     * Programmer: Justin Donato
     * Description: Interface for damageable entities
     * Date Created: 2/8/2021
     * Date Last Edited: 2/8/2021
     **/

    ///Health describes how much hit points an entity has
    float Health { get; set; }

    /// <summary>
    /// Entity will take a varying amount of damage
    /// </summary>
    /// <param name="dmg">Amount of damage taken</param>
    void Damage(float dmg);

    /// <summary>
    /// Entity recovers a variable amount of health
    /// </summary>
    /// <param name="heal">Amount of hitpoints recovered</param>
    void Heal(float heal);

    /// <summary>
    /// Entity is killed
    /// </summary>
    void Kill();
}
