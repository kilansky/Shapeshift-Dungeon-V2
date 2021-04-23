using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Spins a UI effect around its Z axis
public class UIEffectSpin : MonoBehaviour
{
    private bool isSpinning;

    private void Start()
    {
        //By default, don't show this effect or spin
        //DeactivateEffect();

        ActivateEffect();
    }

    private void Update()
    {
        //Check to spin this gameObject
        if(isSpinning)
            transform.Rotate(new Vector3(0f, 0f, 60f * Time.deltaTime));
    }

    //Show the effect and begin spinning
    public void ActivateEffect()
    {
        GetComponent<Image>().enabled = true;
        isSpinning = true;
    }

    //Hide the effect and stop spinning
    public void DeactivateEffect()
    {
        GetComponent<Image>().enabled = false;
        isSpinning = false;
    }
}
