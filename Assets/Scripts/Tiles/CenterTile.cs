using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CenterTile : SingletonPattern<CenterTile>
{
    /**
     * Script: CenterTile
     * Programmer: Justin Donato
     * Description: Handles the mechanics of the level such as transitioning from one level to the next
     * Date Created: 2/18/2021
     * Date Last Edited: 3/1/2021
     **/

    [SerializeField] private TextMeshProUGUI levelDisplay;
    [SerializeField] private TMP_FontAsset glowFont;
    [SerializeField] private TMP_FontAsset dimFont;
    [SerializeField] private GameObject invisibleWall;
    [SerializeField] private Material glowMat;
    [SerializeField] private Material dimMat;
    [HideInInspector] public bool onTile = false; //Used to determine if the player is on the tile. Will be used for input to change scenes
    [HideInInspector] public bool canTransition = true; //Lets the center tile be set to not allow transitions

    /// <summary>
    /// Sets a bool to true if the player is in the tile's collider
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" && MonsterSpawner.Instance.floorCleared && canTransition)
        {
            onTile = true;

            if (MonsterSpawner.Instance.floorCleared)
                HUDController.Instance.ShowQuickHint("Interact");
        }
    }

    /// <summary>
    /// Sets a bool to false if the player leaves the collider
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            onTile = false;

            if (MonsterSpawner.Instance.floorCleared)
                HUDController.Instance.HideQuickHint();
        }
    }

    /// <summary>
    /// Sets the text of the floor tile
    /// </summary>
    /// <param name="text"></param>
    public void SetFloorText(int text)
    {
        levelDisplay.text = text.ToString();
    }

    /// <summary>
    /// Sets the text state (ie: color, alpha, & glow) of the floor tile
    /// </summary>
    /// <param name="text"></param>
    public void SetTextState()
    {
        if (MonsterSpawner.Instance.floorCleared)
        {
            gameObject.GetComponent<Renderer>().material = glowMat;

            levelDisplay.color = new Color32(255, 255, 255, 255);
            levelDisplay.font = glowFont;
        }
        else
        {
            gameObject.GetComponent<Renderer>().material = dimMat;

            levelDisplay.color = new Color32(200, 200, 200, 80);
            levelDisplay.font = dimFont;
        }
    }

    /// <summary>
    /// Sets the state of the invisible walls
    /// </summary>
    /// <param name="input"></param>
    public void SetInvisibleWall(bool input)
    {
        invisibleWall.SetActive(input);
    }
}