using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Decoration : MonoBehaviour
{
    public enum decorTypes
    {
        barrel,
        blood,
        chains,
        crate,
        mushroomCluster,
        mushroomSingle,
        mushroomTile,
        rockCluster,
        rockSingle,
        torch,
        webs,
        explodingBarrel
    }
    public decorTypes propType;
    public bool forceTileSwap;
    public LayerMask mask;

    private GameObject tile;

    public void SetTileVariables()
    {
        transform.parent.GetComponent<Tile>().hasDecor = true;
        if (forceTileSwap)
            transform.parent.GetComponent<Tile>().forceSwap = true;

        ResetRotation();
    }

    private void ResetRotation()
    {
        if (transform.parent.GetComponent<TileRotator>())
            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y - transform.parent.rotation.eulerAngles.y, transform.eulerAngles.z);
    }

    private void Awake()
    {
        GetParentTile();
        if (tile)
            transform.SetParent(tile.transform);
        else
            Debug.LogWarning("No tile detected");
    }

    [ContextMenu("Test")]
    public void GetParentTile()
    {
        RaycastHit hit;

        if (Physics.Raycast(transform.position + Vector3.up, transform.TransformDirection(Vector3.down), out hit, Mathf.Infinity, mask)) //Sends a raycast to look for an object below this one
        {
            if (hit.transform.gameObject.GetComponent<Tile>()) //If the raycast finds an object, this finds out if that object is a tile
            {
                //The tile gameobject is stored for later use and is returned back to the original script that called it
                tile = hit.transform.gameObject;
            }
        }
        else
        {
            Debug.Log("No raycast hit");
        }
    }
}
