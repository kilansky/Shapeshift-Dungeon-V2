using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

        if (GetPotionCount() == 0)
            HUDController.Instance.HidePotionsPanel();
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

            AudioManager.Instance.Play("PlayerHit");

            //prevent from taking damage temporarily
            StartCoroutine(InvincibilityFrames());
        }
    }

    /// <summary>
    /// Secondary Damage function that takes into account the damage source to adjust the damage tracker script variables - AHL (3/30/21)
    /// </summary>
    public virtual void Damage(float damage, GameObject damageSource)
    {
        if (!isInvincible && Health > 0)
        {
            //deal damage to player
            Health -= damage * damageModifier.Value;
            Health = Mathf.Clamp(Health, 0, maxHealth);
            StartCoroutine(HUDController.Instance.UpdateHealthBar(Health, maxHealth));
            StartCoroutine(HUDController.Instance.ShowPlayerDamagedOverlay());

            GetComponent<DamageTracker>().updateDamage(damage, damageSource); //Updates the damage variables in damage tracker baseed on the amount of damage that the player took from a specific source

            //Check if the player is dead
            if (Health <= 0)
            {
                Kill();
                return;
            }

            //AudioManager.Instance.Play("PlayerHitMagic");
            //prevent from taking damage temporarily
            StartCoroutine(InvincibilityFrames());
        }
    }

    public virtual void FireDamage(float damage)
    {
        if (Health > 0)
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
                    Heal(maxHealth); //Old healing value: 15f + additionalPotionHealing.Value
                    AudioManager.Instance.Play("Potion");

                    if (GetPotionCount() == 0)
                        HUDController.Instance.HidePotionsPanel();

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

                if (GetPotionCount() > 0)
                    HUDController.Instance.ShowPotionsPanel();

                //Once player has grabbed a potion on the starting floor, clear the floor
                if (LevelManager.Instance.levelsCompleted == 0)
                {
                    MonsterSpawner.Instance.floorCleared = true;
                    CenterTile.Instance.SetTextState();
                }

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

    //Heal the player (Red Herb)
    public virtual void Heal(float heal)
    {
        Health += heal;
        Health = Mathf.Clamp(Health, 0, maxHealth);
        StartCoroutine(HUDController.Instance.UpdateHealthBar(Health, maxHealth));

        if(Health > maxHealth/4)
            StartCoroutine(HUDController.Instance.HidePlayerDamagedOverlay());
    }

    //Game is over, display game over screen and level review
    public virtual void Kill()
    {
        AnalyticsEvents.Instance.PlayerDied(); //Send Player Died Analytics Event
        AnalyticsEvents.Instance.ItemsOnDeath(); //Send Items On Death Analytics Event

        //Play Death Animation, lock player movement, and zoom in the camera
        PlayerController.Instance.PlayerAnimator.SetBool("isDead", true);
        PlayerController.Instance.IsDead = true;
        CameraController.Instance.PlayerDeathZoomIn();

        //Wait to show game over screen until death anim is done
        StartCoroutine(WaitToShowGameOver());
    }

    //Makes the player invincible briefly
    IEnumerator InvincibilityFrames()
    {
        isInvincible = true;
        yield return new WaitForSeconds(dmgInvincibilityTime);
        isInvincible = false;
    }

    //Wait to show the game over screen upon death
    IEnumerator WaitToShowGameOver()
    {
        yield return new WaitForSeconds(4f);

        HUDController.Instance.ShowGameOver();
        Time.timeScale = 0;
    }
}
