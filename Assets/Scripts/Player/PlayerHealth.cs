using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerHealth : SingletonPattern<PlayerHealth>, IDamageable
{
    [Header("Health Stats")]
    public float startingMaxHealth = 30f;
    public int startingPotionCount = 0;
    public PlayerStats damageModifier; //Damage Modifier from ItemsEquipment for Damage modifier
    public float dmgInvincibilityTime = 1f;
    public PlayerStats additionalPotionHealing; //Potion Healing Modifier to be used from ItemsEquipment
    
    [Header("UI References")]
    //public Slider healthBar;
    //public TextMeshProUGUI healthText;
    public Image[] potionSlots = new Image[3];
    public Sprite healthPotionIcon;
    public Sprite transparentSquare;

    public float Health { get; set; }
    [HideInInspector] public float maxHealth; //**AHL - [PlayerStats] Remember to adjust the script for this and get the values right
    [HideInInspector] public bool isInvincible = false;

    private void Start()
    {
        //Set health & health bar values
        maxHealth = startingMaxHealth;
        Health = maxHealth;
        StartCoroutine(HUDController.Instance.UpdateHealthBar(Health, maxHealth));

        //Set starting potions - REPLACE W/ BETTER SYSTEM LATER!
        int potionsAdded = 0;
        foreach (Image potionIcon in potionSlots)
        {
            if (potionsAdded < startingPotionCount)
            {
                potionIcon.sprite = healthPotionIcon;
                potionsAdded++;
            }
            else
                potionIcon.sprite = transparentSquare;
        }
    }

    public virtual void Damage(float damage)
    {
        if (!isInvincible && Health > 0)
        {
            //deal damage to player
            Health -= damage * damageModifier.Value;
            Health = Mathf.Clamp(Health, 0, maxHealth);
            StartCoroutine(HUDController.Instance.UpdateHealthBar(Health, maxHealth));
            StartCoroutine(HUDController.Instance.ShowPlayerDamagedOverlay());

            //Check if the player is dead
            if (Health <= 0)
            {
                Kill();
                return;
            }

            //prevent from taking damage temporarily
            StartCoroutine(InvincibilityFrames());
        }
    }

    //Uses a potion - REPLACE W/ BETTER SYSTEM LATER!
    public void UsePotion()
    {
        //Only use a potion if NOT at max health already
        if(Health < maxHealth)
        {
            //Check each potion slot back to front
            //If the potion is available, use it
            for (int i = potionSlots.Length - 1; i >= 0; i--)
            {
                if (potionSlots[i].sprite != transparentSquare)
                {
                    potionSlots[i].sprite = transparentSquare;
                    Heal(15f + additionalPotionHealing.Value);
                    StartCoroutine(HUDController.Instance.HidePlayerDamagedOverlay());
                    return;
                }
            }
            Debug.Log("Out of Health Potions!");
        }
    }

    //Adds a potion - REPLACE W/ BETTER SYSTEM LATER!
    public void AddPotion()
    {
        //Check each potion slot front to back
        //If the slot is available, add the potion to it
        for (int i = 0; i < potionSlots.Length; i++)
        {
            if (potionSlots[i].sprite == transparentSquare)
            {
                potionSlots[i].sprite = healthPotionIcon;
                return;
            }
        }
    }

    //Checks the number of potions the player has
    public int GetPotionCount()
    {
        int potionCount = 0;
        //Check each potion slot front to back
        //If the slot is available, add the potion to it
        for (int i = 0; i < potionSlots.Length; i++)
        {
            if (potionSlots[i].sprite == healthPotionIcon)
                potionCount++;
        }
        return potionCount;
    }

    //Increase the maximum health value of the player
    public void IncreaseMaxHealth(int increaseValue)
    {
        maxHealth += increaseValue;
        Health += increaseValue;
        StartCoroutine(HUDController.Instance.UpdateHealthBar(Health, maxHealth));
    }

    //Heal the player (potions)
    public virtual void Heal(float heal)
    {
        Health += heal;
        Health = Mathf.Clamp(Health, 0, maxHealth);
        StartCoroutine(HUDController.Instance.UpdateHealthBar(Health, maxHealth));
    }

    //Game is over, display game over screen and level review
    public virtual void Kill()
    {
        AnalyticsEvents.Instance.PlayerDied(); //Send Player Died Analytics Event
        AnalyticsEvents.Instance.ItemsOnDeath(); //Send Items On Death Analytics Event
        HUDController.Instance.ShowGameOver();
    }

    //Makes the player invincible briefly
    IEnumerator InvincibilityFrames()
    {
        isInvincible = true;
        yield return new WaitForSeconds(dmgInvincibilityTime);
        isInvincible = false;
    }
}
