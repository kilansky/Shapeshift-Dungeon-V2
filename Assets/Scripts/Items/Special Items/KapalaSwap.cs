using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KapalaSwap : MonoBehaviour
{
    //Sprite Variables to hold all the Kapala sprites to switch through them for when the Kapala charge is a certain value
    public Sprite Kapala0; //Empty Kapala
    public Sprite Kapala1; //Kapala at 40% charge
    public Sprite Kapala2; //Kapala at 60% charge
    public Sprite Kapala3; //Kapala at 80% charge
    public Sprite Kapala4; //Kapala at 100% charge

    /// <summary>
    /// Function that takes in a float value (SpecialCharge / maxValue) and adjusts the sprite on the Kapala Item/Equipment that will be adjusted in the game based on it's charge value - AHL (4/25/21)
    /// </summary>
    /// <param name="percentage"></param>
    public void KapalaSpriteSwap(float percentage)
    {
        //If the Kapala is in the Special Item Slot
        if(PlayerController.Instance.SpecialSlot.ItemName == "Kapala")
        {
            //Empty Kapala Sprite if the percentage is less than 40% (0.4)
            if (percentage < 0.4f)
                PlayerController.Instance.SpecialSlot.sprite = Kapala0;

            //Kapala1 Sprite used if the percentage is less than 60% (0.6)
            else if (percentage < 0.6f)
                PlayerController.Instance.SpecialSlot.sprite = Kapala1;

            //Kapala2 Sprite used if the percentage is less than 80% (0.8)
            else if (percentage < 0.8f)
                PlayerController.Instance.SpecialSlot.sprite = Kapala2;

            //Kapala3 Sprite used if the percentage is less than 100% (1)
            else if (percentage < 1)
                PlayerController.Instance.SpecialSlot.sprite = Kapala3;

            //Kapala4 Sprite used if the percentage is 100% (1)
            else
                PlayerController.Instance.SpecialSlot.sprite = Kapala4;

            HUDController.Instance.SetNewSpecialItemIcon();
        }

        //If the Kapala is in the BOH Slot and it is active
        else if(PlayerController.Instance.hasBagOfHolding && PlayerController.Instance.BagOfHoldingSlot.ItemName == "Kapala")
        {
            //Empty Kapala Sprite if the percentage is less than 40% (0.4)
            if (percentage < 0.4f)
                PlayerController.Instance.BagOfHoldingSlot.sprite = Kapala0;

            //Kapala1 Sprite used if the percentage is less than 60% (0.6)
            else if (percentage < 0.6f)
                PlayerController.Instance.BagOfHoldingSlot.sprite = Kapala1;

            //Kapala2 Sprite used if the percentage is less than 80% (0.8)
            else if (percentage < 0.8f)
                PlayerController.Instance.BagOfHoldingSlot.sprite = Kapala2;

            //Kapala3 Sprite used if the percentage is less than 100% (1)
            else if (percentage < 1)
                PlayerController.Instance.BagOfHoldingSlot.sprite = Kapala3;

            //Kapala4 Sprite used if the percentage is 100% (1)
            else
                PlayerController.Instance.BagOfHoldingSlot.sprite = Kapala4;

            HUDController.Instance.SetNewSpecialItemIcons();
        }
    }
}
