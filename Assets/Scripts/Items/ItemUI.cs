using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemUI : MonoBehaviour
{
    //Variable Initialization/Declaration
    public Image Image; //Slot to put in the item image
    public TMP_Text Name; //Gets the name of the item
    public TMP_Text Description; //Gets the description of the item

    /// <summary>
    /// When the item is created it will set all the needed UI elemets - AHL (3/8/21)
    /// </summary>
    private void Awake()
    {
        Image.sprite = GetComponentInParent<Item>().item.sprite;
        Name.text = GetComponentInParent<Item>().item.ItemName;
        Description.text = GetComponentInParent<Item>().item.itemDescription;
    }
}
