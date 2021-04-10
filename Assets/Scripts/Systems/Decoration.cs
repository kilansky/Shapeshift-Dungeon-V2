using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Decoration : MonoBehaviour
{
    public enum decorTypes
    {
        barrel,
        chains,
        crate,
        mushroomCluster,
        mushroomSingle,
        mushroomTile,
        torch,
        webs
    }
    public decorTypes propType;
    public bool forceTileSwap;  

    public void SetTileVariables()
    {
        transform.parent.parent.GetComponent<Tile>().hasDecor = true;
        if (forceTileSwap)
            transform.parent.parent.GetComponent<Tile>().forceSwap = true;

        ResetRotation();
    }

    private void ResetRotation()
    {
        if (transform.parent.parent.GetComponent<TileRotator>())
            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y - transform.parent.parent.rotation.eulerAngles.y, transform.eulerAngles.z);
    }
}
