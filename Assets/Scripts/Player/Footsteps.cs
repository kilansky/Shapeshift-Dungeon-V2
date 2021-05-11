using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Footsteps : SingletonPattern<Footsteps>
{
    /*
     * Script: Footsteps
     * Programmer: Justin Donato
     * Description: Plays footstep sounds based on the tile the player is currently above
     * Date Created: 5/10/2021
     * Date Last Edited: 5/10/2021
     */

    public float rateOfStep = .25f;
    public LayerMask mask;

    private AudioManager audioPlayer;
    private CharacterController controller;
    private bool isPlayingSound = false;

    private void Start()
    {
        audioPlayer = AudioManager.Instance;
        controller = GetComponent<CharacterController>();
    }

    private void Update()
    {
        if((controller.velocity.x > .5f || controller.velocity.z > .5f || controller.velocity.x < -.5f || controller.velocity.z < -.5f) && !PlayerController.Instance.IsDashing && !PlayerController.Instance.IsAttacking)
        {
            if(!isPlayingSound)
            {
                StopAllCoroutines();
                isPlayingSound = true;
                StartCoroutine(FootstepCycle());
            }
        }
        else if(PlayerController.Instance.IsDashing || PlayerController.Instance.IsAttacking)
        {
            StopAllCoroutines();
            StopFootsteps();
            isPlayingSound = false;
        }
        else
        {
            StopAllCoroutines();
            StartCoroutine(WaitToResumeSound());
            //StopFootsteps();
        }
    }

    private IEnumerator FootstepCycle()
    {
        
        audioPlayer.Play(GetFootstepType(), Random.Range(0.9f, 1.1f));
        yield return new WaitForSeconds(rateOfStep);
        isPlayingSound = false;
    }

    private string GetFootstepType()
    {
        Tile.tileTypes currentTile = GetTileType();

        if (currentTile == Tile.tileTypes.dirtGrass || currentTile == Tile.tileTypes.stoneGrass)
            return "Grass";
        else if (currentTile == Tile.tileTypes.sand)
            return "Sand";
        else if (currentTile == Tile.tileTypes.dirt)
            return "Dirt";
        else if (currentTile == Tile.tileTypes.stairs || currentTile == Tile.tileTypes.stone || currentTile == Tile.tileTypes.item || currentTile == Tile.tileTypes.arcane || currentTile == Tile.tileTypes.dispenser || currentTile == Tile.tileTypes.torch)
            return "Stone";
        else if (currentTile == Tile.tileTypes.spike || currentTile == Tile.tileTypes.bridge)
            return "Metal";
        else
            return "Pit";
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    private Tile.tileTypes GetTileType()
    {
        RaycastHit hit;

        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), out hit, Mathf.Infinity, mask)) //Sends a raycast to look for an object below this one
        {
            if (hit.transform.gameObject.GetComponent<Tile>()) //If the raycast finds an object, this finds out if that object is a tile
            {
                return hit.transform.gameObject.GetComponent<Tile>().tileType;
            }
            else if(hit.transform.parent.GetComponent<CenterTile>())
                return Tile.tileTypes.stone;
            else //If object is not a tile, return null
            {
                return Tile.tileTypes.pit;
            }
        }
        else //If raycast finds no object, return null
        {
            return Tile.tileTypes.pit;
        }
    }

    public void StopFootsteps()
    {
        audioPlayer.Stop("Grass");
        audioPlayer.Stop("Sand");
        audioPlayer.Stop("Dirt");
        audioPlayer.Stop("Stone");
        audioPlayer.Stop("Metal");
    }

    private IEnumerator WaitToResumeSound()
    {
        yield return new WaitForSeconds(.2f);
        isPlayingSound = false;
    }
}