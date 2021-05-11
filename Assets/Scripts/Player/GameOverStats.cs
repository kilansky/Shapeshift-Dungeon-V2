using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameOverStats : MonoBehaviour
{
    public TextMeshProUGUI runTimeText;
    public TextMeshProUGUI floorText;
    public TextMeshProUGUI healthText;
    public TextMeshProUGUI attackText;
    public TextMeshProUGUI speedText;
    public Image[] items;
    public Sprite smallPotion;

    public void SetGameEndStats()
    {
        PlayerController pc = PlayerController.Instance;

        runTimeText.text = "Time: " + RunTimer.Instance.TimerTextValue;
        floorText.text = "Floor Reached: " + LevelManager.Instance.currFloor.ToString();
        healthText.text = "Health - " + pc.StatMaxHealthCount.ToString();
        attackText.text = "Attack - " + pc.StatAttackCount.ToString();
        speedText.text = "Speed - " + pc.StatSpeedCount.ToString();

        int i = 0;
        if(pc.SpecialSlot)
        {
            items[i].sprite = pc.SpecialSlot.sprite;
            i++;
        }
        if (pc.HeadSlot)
        {
            items[i].sprite = pc.HeadSlot.sprite;
            i++;
        }
        if (pc.TorsoSlot)
        {
            items[i].sprite = pc.TorsoSlot.sprite;
            i++;
        }
        if (pc.FootSlot)
        {
            items[i].sprite = pc.FootSlot.sprite;
            i++;
        }
        if (pc.PocketSlot1)
        {
            items[i].sprite = pc.PocketSlot1.sprite;
            i++;
        }
        if (pc.PocketSlot2)
        {
            items[i].sprite = pc.PocketSlot2.sprite;
            i++;
        }

        int numPotions = PlayerHealth.Instance.GetPotionCount();
        while(numPotions > 0)
        {
            items[i].sprite = smallPotion;
            i++;
            numPotions--;
        }
    }
}
