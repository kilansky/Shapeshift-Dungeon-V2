using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerUIController : MonoBehaviour
{
    private Vector2 navigationVector;

    //
    public void Navigate(InputAction.CallbackContext context)
    {
        Vector2 axisInput = context.ReadValue<Vector2>();
        navigationVector = new Vector3(axisInput.x, 0, axisInput.y); //Set input y value to z axis
    }

    public void Submit(InputAction.CallbackContext context)
    {
        if (context.performed)
        {

        }
    }

    public void Back(InputAction.CallbackContext context)
    {
        if (context.performed)
        {

        }
    }
}
