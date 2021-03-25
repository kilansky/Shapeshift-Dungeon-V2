using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageTestScript : MonoBehaviour, IDamageable
{
    /**
     * Script: DamageTest
     * Programmer: Justin Donato
     * Description: This is a script to test the damage interface
     * Date Created: 2/8/2021
     * Date Last Edited: 2/8/2021
     **/

    public float Health { get; set; }

    public void Damage(float dmg)
    {
        Health -= dmg;
        PrintHealth();

        if(Health<=0)
        {
            Kill();
        }
    }

    public void Heal(float heal)
    {
        Health += heal;
        PrintHealth();
    }

    public void Kill()
    {
        Debug.Log("This object has been killed!");
        Destroy(gameObject);
    }

    private void Start()
    {
        Health = 10f;
        Damage(9f);
        Heal(4f);
        Damage(17f);
    }

    private void PrintHealth()
    {
        Debug.Log("This test enemy has " + Health + " health");
    }
}
