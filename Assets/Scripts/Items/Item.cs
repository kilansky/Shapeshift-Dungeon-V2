﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public int price = 0;
    private int originalPrice = 0;
    public ItemsEquipment item;
    public GameObject itemBase;

    public GameObject priceCanvas;

    private bool isSecondItem = false;
    private bool isNewBOHItem = false;

    /// <summary>
    /// AHL - 4/8/21
    /// Equip function to have the item be attached to the player and adjusts their stats by using the StatModifier script
    /// </summary>
    public void Equip(PlayerController c, PlayerHealth h)
    {
        //Variables to set the item bools to false to cause no further mixups on getting an item
        c.touchingItem = false;
        c.pickupItem = false;

        //Set the item icon in the equipment panel - Sky (3/28/21)
        HUDController.Instance.SetEquipmentPanelItem(item.ItemSlot, item.sprite);

        //Long chain of if statements XD - Sometimes I wonder if switch cases are the best

        //Special Item Slot
        if (item.ItemSlot == 0)
        {
            if (c.SpecialSlot == null) //If the player doesn't have a special item then equip one
            {
                c.SpecialSlot = this.item;

                //If the item is the Kapala then we set the CanUseSpecial to false as you can't use the Kapala out of the gate and resets the sprite - AHL (4/25/21)
                if (item.ItemName == "Kapala")
                {
                    c.canUseSpecial = false;
                    item.prefab.GetComponent<KapalaSwap>().KapalaSpriteSwap(0);
                }
            }
                
                
            //If the BOH Slot is being set for the first time then we have some special aspecst to do with setting values
            else if(c.hasBagOfHolding && !c.BagOfHoldingSlot) //If the player has a special item and the bag of holding but nothing in the bag of holding then we place the item in the bag
            {
                c.BagOfHoldingSlot = this.item;
                isNewBOHItem = true;

                //If the item is the Kapala then we need to reset it's sprite  - AHL (4/25/21)
                if (item.ItemName == "Kapala")
                {
                    item.prefab.GetComponent<KapalaSwap>().KapalaSpriteSwap(0);
                    c.specialCharge2MaxValue = 10;
                }

                //If the item is anything but the Kapala then we need to set its special charge 2 max value to whatever it's value is
                else
                {
                    c.specialCharge2 = item.statMods[0].adjustableValue;
                    c.specialCharge2MaxValue = c.specialCharge2;
                }
            }

            else //If the player does have a special item and No Bag of Holding OR if the player has the bag of holding and it is filled as well then unequip their current one and equip the new one
            {
                if(c.isItemSwapping == false) //Only instantiate the prefab if there is no item swapping happening from the Bag Of Holding button
                    Instantiate(c.SpecialSlot.prefab, transform.position, transform.rotation, transform.parent);
                
                c.SpecialSlot.prefab.GetComponent<Item>().Unequip(c, h);
                c.SpecialSlot = this.item;
            }
        }

        //Head Item Slot
        else if (item.ItemSlot == 1)
        {
            if (c.HeadSlot == null) //If the player doesn't have a head item then equip one
                c.HeadSlot = this.item;

            else //If the player does have a head item then unequip their current one and equip the new one
            {
                Instantiate(c.HeadSlot.prefab, transform.position, transform.rotation, transform.parent);
                c.HeadSlot.prefab.GetComponent<Item>().Unequip(c, h);
                c.HeadSlot = this.item;
            }

            //Checks if the item being equipped is the Monster Mask so we can adjust the bool variable in the player controller
            if (item.ItemName == "Monster Mask")
                c.hasMonsterMask = true;

        }

        //Torso Item Slot
        else if (item.ItemSlot == 2)
        {
            if (c.TorsoSlot == null) //If the player doesn't have a torso item then equip one
                c.TorsoSlot = this.item;

            else //If the player does have a torso item then unequip their current one and equip the new one
            {
                Instantiate(c.TorsoSlot.prefab, transform.position, transform.rotation, transform.parent);
                c.TorsoSlot.prefab.GetComponent<Item>().Unequip(c, h);
                c.TorsoSlot = this.item;
            }
        }

        //Foot Item Slot
        else if (item.ItemSlot == 3)
        {
            if (c.FootSlot == null) //If the player doesn't have a foot item then equip one
                c.FootSlot = this.item;

            else //If the player does have a foot item then unequip their current one and equip the new one
            {
                Instantiate(c.FootSlot.prefab, transform.position, transform.rotation, transform.parent);
                c.FootSlot.prefab.GetComponent<Item>().Unequip(c, h);
                c.FootSlot = this.item;
            }
        }

        //Pocket Item Slots
        else if (item.ItemSlot == 4)
        {
            if (c.PocketSlot1 == null) //If the player doesn't have a pocket1 item then equip one
                c.PocketSlot1 = this.item;

            else if (c.PocketSlot2 == null) //If the player doesn't have a pocket2 item then equip one
                c.PocketSlot2 = this.item;

            else //If the player does have a pocket1 and pocket2 items then unequip the first one, slide 2 to 1, then equip the new one
            {
                if (c.PocketSlot1.ItemName == "Red Herb") //Checks if the pocket slot 1 item is the Red Herb so we can adjust the bool variable in the player controller since we are dropping it
                {
                    c.hasRedHerb = false;
                }

                if (c.PocketSlot1.ItemName == "Bag Of Holding") //Checks if the pocket slot 1 item is the Bag Of Holding item so we can adjust the bool variable in the player controller since we are dropping it
                {
                    c.hasBagOfHolding = false;

                    HUDController.Instance.HideSpecialSwapPanel();
                    HUDController.Instance.HideBagOfHoldingSlot();
                }

                Instantiate(c.PocketSlot1.prefab, transform.position, transform.rotation, transform.parent);
                c.PocketSlot1.prefab.GetComponent<Item>().Unequip(c, h);
                c.PocketSlot1 = c.PocketSlot2;
                c.PocketSlot2 = this.item;
            }

            if (item.ItemName == "Red Herb") //Checks if the item being equipped is a Red Herb so we can adjust the bool variable in the player controller
            {
                c.hasRedHerb = true;
            }

            if (item.ItemName == "Bag Of Holding") //Checks if the item being equipped is the Bag Of Holding itme so we can adjust the bool variable in the player controller
            {
                c.hasBagOfHolding = true;
                HUDController.Instance.ShowBagOfHoldingSlot();

                if(c.BagOfHoldingSlot)
                    HUDController.Instance.ShowSpecialSwapPanel();
            }
        }

        //Pick Up Items
        else if (item.ItemSlot == 5)
        {
            //Checks the name of the pick up and applies the following code as needed
            
            //Health Potion
            if(item.ItemName == "Health Potion")
            {
                //print("The amount of potions before we add 1 is " + PlayerHealth.Instance.potionSlots.Length);
                PlayerHealth.Instance.AddPotion(); //Adds 1 potion to the total pool
            }

            //Stat Upgrade Potion
            else if(item.ItemName == "Stat Upgrade Potion")
            {
                //print("Alright time to open up the stat potion upgrade panel!");
                HUDController.Instance.ShowStatPotionPanel(); //Opens up the Upgrade Stat Potion Panel
            }

            //Gem Bag
            else if (item.ItemName == "Gem Bag")
            {
                PlayerController.Instance.GetComponent<PlayerGems>().AddGems(5);
            }
        }


        for (int i = 0; i < item.statMods.Length; i++) //For-Loop to go through all the elements in the array to make sure they equip properly
        {
            //Long chain of if statements XD - Sometimes I wonder if switch cases are the best

            //HP Adjustment - Currently not set up but is a placeholder for a stat for later if needed
            /*if ((int)item.statMods[i].statType == 1)
            {
                //**AHL**
                //DON'T HAVE ANYTHING YET (Check PlayerHealth Script)
                //Using a Debug to show it works
                Debug.Log("This item " + item.ItemName + " has been equipped so HP has been adjusted.");
            }*/

            //Move Speed Adjustment
            if ((int)item.statMods[i].statType == 2)
            {
                //Flat Value
                if ((int)item.statMods[i].statModifier == 100)
                    c.baseMoveSpeed.AddModifiers(new StatModifier(item.statMods[i].adjustableValue, StatModType.Flat, item.prefab));

                //Percent Add Value
                if ((int)item.statMods[i].statModifier == 200)
                    c.baseMoveSpeed.AddModifiers(new StatModifier(item.statMods[i].adjustableValue, StatModType.PercentAdd, item.prefab));

                //Percent Mult Value
                if ((int)item.statMods[i].statModifier == 300)
                    c.baseMoveSpeed.AddModifiers(new StatModifier(item.statMods[i].adjustableValue, StatModType.PercentMult, item.prefab));

                //Debug.Log("This item " + item.ItemName + " has been equipped so Move Speed has been adjusted.");
                //Debug.Log("The new move speed is " + c.baseMoveSpeed.Value);
            }

            //Attack Damage Adjustment
            else if ((int)item.statMods[i].statType == 3)
            {
                //Flat Value
                if ((int)item.statMods[i].statModifier == 100)
                    c.baseAttackDamage.AddModifiers(new StatModifier(item.statMods[i].adjustableValue, StatModType.Flat, item.prefab));

                //Percent Add Value
                if ((int)item.statMods[i].statModifier == 200)
                    c.baseAttackDamage.AddModifiers(new StatModifier(item.statMods[i].adjustableValue, StatModType.PercentAdd, item.prefab));

                //Percent Mult Value
                if ((int)item.statMods[i].statModifier == 300)
                    c.baseAttackDamage.AddModifiers(new StatModifier(item.statMods[i].adjustableValue, StatModType.PercentMult, item.prefab));

                //Debug.Log("This item " + item.ItemName + " has been equipped so Attack Damage has been adjusted.");
                //Debug.Log("The new Attack Damage is " + c.baseAttackDamage.Value);
            }

            //Attack Speed Adjustment
            else if ((int)item.statMods[i].statType == 4)
            {
                //Flat Value
                if ((int)item.statMods[i].statModifier == 100)
                    c.baseAttackSpeed.AddModifiers(new StatModifier(item.statMods[i].adjustableValue, StatModType.Flat, item.prefab));

                //Percent Add Value
                if ((int)item.statMods[i].statModifier == 200)
                    c.baseAttackSpeed.AddModifiers(new StatModifier(item.statMods[i].adjustableValue, StatModType.PercentAdd, item.prefab));

                //Percent Mult Value
                if ((int)item.statMods[i].statModifier == 300)
                    c.baseAttackSpeed.AddModifiers(new StatModifier(item.statMods[i].adjustableValue, StatModType.PercentMult, item.prefab));

                //Debug.Log("This item " + item.ItemName + " has been equipped so Attack has been adjusted.");
                //Debug.Log("The new Attack Speed is " + c.attackTime.Value);
            }

            //Charge Attack Time Adjustment
            else if ((int)item.statMods[i].statType == 5)
            {
                //Flat Value
                if ((int)item.statMods[i].statModifier == 100)
                    c.timeToFullCharge.AddModifiers(new StatModifier(item.statMods[i].adjustableValue, StatModType.Flat, item.prefab));

                //Percent Add Value
                if ((int)item.statMods[i].statModifier == 200)
                    c.timeToFullCharge.AddModifiers(new StatModifier(item.statMods[i].adjustableValue, StatModType.PercentAdd, item.prefab));

                //Percent Mult Value
                if ((int)item.statMods[i].statModifier == 300)
                    c.timeToFullCharge.AddModifiers(new StatModifier(item.statMods[i].adjustableValue, StatModType.PercentMult, item.prefab));

                //Debug.Log("This item " + item.ItemName + " has been equipped so Charge Attack Time has been adjusted.");
                //Debug.Log("The new Charge Attack Time is " + c.timeToFullCharge.Value);
            }

            //Attack Knockback Adjustment
            else if ((int)item.statMods[i].statType == 6)
            {
                //**AHL**
                //DON'T HAVE ANYTHING YET (Check PlayerController Script?)
                //Using a Debug to show it works
                //Debug.Log("This item " + item.ItemName + " has been equipped so Attack Knockback has been adjusted.");
            }

            //Dash Invlurnerability Time Adjustment
            else if ((int)item.statMods[i].statType == 7)
            {
                //**AHL**
                //DON'T HAVE ANYTHING YET (Check PlayerController Script?)
                //Using a Debug to show it works
                //Debug.Log("This item " + item.ItemName + " has been equipped so Dash Invulnerability Time has been adjusted.");
            }

            //Dash Speed Adjustment
            else if ((int)item.statMods[i].statType == 8)
            {
                //Flat Value
                if ((int)item.statMods[i].statModifier == 100)
                    c.dashSpeed.AddModifiers(new StatModifier(item.statMods[i].adjustableValue, StatModType.Flat, item.prefab));

                //Percent Add Value
                if ((int)item.statMods[i].statModifier == 200)
                    c.dashSpeed.AddModifiers(new StatModifier(item.statMods[i].adjustableValue, StatModType.PercentAdd, item.prefab));

                //Percent Mult Value
                if ((int)item.statMods[i].statModifier == 300)
                    c.dashSpeed.AddModifiers(new StatModifier(item.statMods[i].adjustableValue, StatModType.PercentMult, item.prefab));

                //Debug.Log("This item " + item.ItemName + " has been equipped so Dash Speed has been adjusted.");
                //Debug.Log("The new Dash Speed is " + c.dashSpeed.Value);
            }

            //Dash Refresh Speed Adjustment
            else if ((int)item.statMods[i].statType == 9)
            {
                //Flat Value
                if ((int)item.statMods[i].statModifier == 100)
                    c.dashCooldownTime.AddModifiers(new StatModifier(item.statMods[i].adjustableValue, StatModType.Flat, item.prefab));

                //Percent Add Value
                if ((int)item.statMods[i].statModifier == 200)
                    c.dashCooldownTime.AddModifiers(new StatModifier(item.statMods[i].adjustableValue, StatModType.PercentAdd, item.prefab));

                //Percent Mult Value
                if ((int)item.statMods[i].statModifier == 300)
                    c.dashCooldownTime.AddModifiers(new StatModifier(item.statMods[i].adjustableValue, StatModType.PercentMult, item.prefab));

                //Debug.Log("This item " + item.ItemName + " has been equipped so Dash Cooldown has been adjusted.");
                //Debug.Log("The new Dash Cooldown is " + c.dashCooldownTime.Value);
            }

            //Damage From Enemies Adjustment
            else if ((int)item.statMods[i].statType == 10)
            {
                //Flat Value
                if ((int)item.statMods[i].statModifier == 100)
                    h.damageModifier.AddModifiers(new StatModifier(item.statMods[i].adjustableValue, StatModType.Flat, item.prefab));

                //Percent Add Value
                if ((int)item.statMods[i].statModifier == 200)
                    h.damageModifier.AddModifiers(new StatModifier(item.statMods[i].adjustableValue, StatModType.PercentAdd, item.prefab));

                //Percent Mult Value
                if ((int)item.statMods[i].statModifier == 300)
                    h.damageModifier.AddModifiers(new StatModifier(item.statMods[i].adjustableValue, StatModType.PercentMult, item.prefab));

                //Debug.Log("This item " + item.ItemName + " has been equipped so Damage From Enemies has been adjusted.");
                //Debug.Log("The new Damage From Enemies is " + h.damageModifier.Value);
            }

            //Potion Healing Adjustment
            else if ((int)item.statMods[i].statType == 11)
            {
                //Flat Value
                if ((int)item.statMods[i].statModifier == 100)
                    h.additionalPotionHealing.AddModifiers(new StatModifier(item.statMods[i].adjustableValue, StatModType.Flat, item.prefab));

                //Percent Add Value
                if ((int)item.statMods[i].statModifier == 200)
                    h.additionalPotionHealing.AddModifiers(new StatModifier(item.statMods[i].adjustableValue, StatModType.PercentAdd, item.prefab));

                //Percent Mult Value
                if ((int)item.statMods[i].statModifier == 300)
                    h.additionalPotionHealing.AddModifiers(new StatModifier(item.statMods[i].adjustableValue, StatModType.PercentMult, item.prefab));

                //Debug.Log("This item " + item.ItemName + " has been equipped so they will gain " + item.statMods[i].adjustableValue + " more health from healing items.");
            }

            //Special Item Recharge Time Adjustment
            else if ((int)item.statMods[i].statType == 12)
            {
                //This item equips only if this is the main item and not for the BOH original pick up
                if (!isNewBOHItem)
                {
                    //Flat Value
                    if ((int)item.statMods[i].statModifier == 100)
                        c.specialCooldownTime.AddModifiers(new StatModifier(item.statMods[i].adjustableValue, StatModType.Flat, item.prefab));

                    //Percent Add Value
                    if ((int)item.statMods[i].statModifier == 200)
                        c.specialCooldownTime.AddModifiers(new StatModifier(item.statMods[i].adjustableValue, StatModType.PercentAdd, item.prefab));

                    //Percent Mult Value
                    if ((int)item.statMods[i].statModifier == 300)
                        c.specialCooldownTime.AddModifiers(new StatModifier(item.statMods[i].adjustableValue, StatModType.PercentMult, item.prefab));
                }

                //Shows the special swap panel
                if(item.ItemName != "AA Battery")
                {
                    HUDController.Instance.ShowSpecialItemPanel();

                    if(c.hasBagOfHolding && c.SpecialSlot != null)//if player already has the bag of holding and a special item, show the swap panel
                        HUDController.Instance.ShowSpecialSwapPanel();
                }

                //Debug.Log("This item " + item.ItemName + " has been equipped so Special Item Recharge Time has been adjusted.");
                //Debug.Log("The new Special Item Recharge Time is " + c.specialCooldownTime.Value);
            }
        }

        HUDController.Instance.HideQuickHint();
        if(c.isItemSwapping == false) //Only Destroy a game object if there is no item swapping happening from the Bag of Holding button
            Destroy(gameObject);

        if (isNewBOHItem)
            isNewBOHItem = false;

        //If the item being equipped is the Kapala then we adjust the hasKapala variable in the playerController Script
        if (item.ItemName == "Kapala")
            c.hasKapala = true;

        StartCoroutine(c.RechargeSpecial());
    }

    /// <summary>
    /// AHL - 2/22/21
    /// Unequip function to have the item be removed from the player and adjusts their stats by using the StatModifier script
    /// </summary>
    public void Unequip(PlayerController c, PlayerHealth h)
    {
        for(int i = 0; i < item.statMods.Length; i++) //For-Loop to go through all the elements in the array to make sure they unequip properly
        {
            //Long chain of if statements XD - Sometimes I wonder if switch cases are the best

            //HP Adjustment - Currently not set up but is a placeholder for a stat for later if needed
            /*if ((int)item.statMods[i].statType == 1)
            {
                //**AHL**
                //DON'T HAVE ANYTHING YET (Check PlayerHealth Script)
                //Using a Debug to show it works
                Debug.Log("This item " + item.ItemName + " has been removed so HP has been adjusted.");
            }*/

            //Move Speed Adjustment
            if ((int)item.statMods[i].statType == 2)
            {
                c.baseMoveSpeed.RemoveAllModifiersFromSource(item.prefab);
                //Debug.Log("This item " + item.ItemName + " has been removed so Move Speed has been adjusted.");
                //Debug.Log("The new move speed is " + c.baseMoveSpeed.Value);
            }

            //Attack Damage Adjustment
            else if ((int)item.statMods[i].statType == 3)
            {
                c.baseAttackDamage.RemoveAllModifiersFromSource(item.prefab);
                //Debug.Log("This item " + item.ItemName + " has been removed so Attack Damage has been adjusted.");
                //Debug.Log("The new Attack Damage is " + c.baseAttackDamage.Value);
            }
            
            //Attack Speed Adjustment
            else if ((int)item.statMods[i].statType == 4)
            {
                c.attackTime.RemoveAllModifiersFromSource(item.prefab);
                //Debug.Log("This item " + item.ItemName + " has been removed so Attack Speed has been adjusted.");
                //Debug.Log("The new Attack Speed is " + c.attackTime.Value);
            }

            //Charge Attack Time Adjustment
            else if ((int)item.statMods[i].statType == 5)
            {
                c.timeToFullCharge.RemoveAllModifiersFromSource(item.prefab);
                //Debug.Log("This item " + item.ItemName + " has been removed so Charge Attack Time has been adjusted.");
                //Debug.Log("The new Charge Attack Time is " + c.chargeRate.Value);
            }

            //Attack Knockback Adjustment
            else if ((int)item.statMods[i].statType == 6)
            {
                //**AHL**
                //DON'T HAVE ANYTHING YET (Check PlayerController Script?)
                //Using a Debug to show it works
                //Debug.Log("This item " + item.ItemName + " has been removed so Attack Knockback has been adjusted.");
            }

            //Dash Invlurnerability Time Adjustment
            else if ((int)item.statMods[i].statType == 7)
            {
                //**AHL**
                //DON'T HAVE ANYTHING YET (Check PlayerController Script?)
                //Using a Debug to show it works
                //Debug.Log("This item " + item.ItemName + " has been removed so Dash Invulnerability Time has been adjusted.");
            }

            //Dash Speed Adjustment
            else if ((int)item.statMods[i].statType == 8)
            { 
                c.dashSpeed.RemoveAllModifiersFromSource(item.prefab);
                //Debug.Log("This item " + item.ItemName + " has been removed so Dash Speed has been adjusted.");
                //Debug.Log("The new Dash Speed is " + c.dashSpeed.Value);
            }

            //Dash Refresh Speed Adjustment
            else if ((int)item.statMods[i].statType == 9)
            {
                c.dashCooldownTime.RemoveAllModifiersFromSource(item.prefab);
                //Debug.Log("This item " + item.ItemName + " has been removed so Dash Cooldown has been adjusted.");
                //Debug.Log("The new Dash Cooldown is " + c.dashCooldownTime.Value);
            }

            //Damage From Enemies Adjustment
            else if ((int)item.statMods[i].statType == 10)
            {
                h.damageModifier.RemoveAllModifiersFromSource(item.prefab);
                //Debug.Log("This item " + item.ItemName + " has been removed so Damage From Enemies has been adjusted.");
                //Debug.Log("The new Damage From Enemies is " + h.damageModifier.Value);
            }

            //Potion Healing Adjustment
            else if ((int)item.statMods[i].statType == 11)
            {
                h.additionalPotionHealing.RemoveAllModifiersFromSource(item.prefab);
                //Debug.Log("This item " + item.ItemName + " has been removed so they will gain " + item.statMods[i].adjustableValue + " less health from healing items.");
            }

            //Special Item Recharge Time Adjustment
            else if ((int)item.statMods[i].statType == 12)
            {
                c.specialCooldownTime.RemoveAllModifiersFromSource(item.prefab);
                //Debug.Log("This item " + item.ItemName + " has been removed so Special Item Recharge has been adjusted.");
                //Debug.Log("The new Special Item Recharge is " + c.specialCooldownTime.Value);
            }
        }

        //If the monster Mask is being unequipped then we set the bool to false
        if (item.ItemName == "Monster Mask")
            c.hasMonsterMask = false;

        //If the Kapala is being unequipped then we set the bool to false
        if (item.ItemName == "Kapala")
            c.hasKapala = false;
    }

    private void Start()
    {
        originalPrice = price;
    }

    public void SetPrice(int value)
    {
        price = value;
        itemBase.GetComponent<ItemUI>().SetPriceCanvas();
        isSecondItem = true;
    }

    public bool IsSecondItem()
    {
        return isSecondItem;
    }

    public void SetWorldGUILayer(bool active)
    {
        if (active)
        {
            foreach (Transform canvas in itemBase.transform)
            {
                canvas.gameObject.layer = 16;//Set world GUI layer on each child
            }
        }
        else
        {
            foreach (Transform canvas in itemBase.transform)
            {
                canvas.gameObject.layer = 5;//Set UI layer on each child
            }
        }
    }


    ///<summary>
    /// Variables that item stores
    /// int ItemSlot - What player slot does this item go to?
    /// string ItemName - Items Name
    /// float ModValue - The adjustable % or value to be added to the modifier
    /// int StatModifier - Flalt (100), %add (200), %mult (300)
    /// int PlayerStatAdjusted - Which stat is being adjusted for the player?
    /// </summary>
}
