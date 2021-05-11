using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileRotator : MonoBehaviour
{
    private int rotateAmount;
    private bool hasCounterRotated = false;

    void Start()
    {
        rotateAmount = Random.Range(0, 4);

        if (transform.localEulerAngles == Vector3.zero)//only perform random rotation if tile has not been rotated manually
        {
            transform.Rotate(0, 90 * rotateAmount, 0);//Rotate tile randomly

            //reset the rotation of torches if this is a torch tile
            if (GetComponent<Tile>() && GetComponent<Tile>().tileType == Tile.tileTypes.torch)
            {
                GetComponentInChildren<Torch>().ResetRotation();
            }
        }
    }

    public void UndoRotation()
    {
        if(!hasCounterRotated)
        {
            transform.Rotate(0, 90 * -rotateAmount, 0);//Rotate tile back to the starting rotation
            hasCounterRotated = true;
        }
    }
}
