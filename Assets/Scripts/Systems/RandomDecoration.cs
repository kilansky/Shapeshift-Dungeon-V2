using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomDecoration : MonoBehaviour
{
    void Start()
    {
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(false); //deactivate all children
        }

        int randDecor = Random.Range(0, transform.childCount); //get a random index based on the number of children
        transform.GetChild(randDecor).gameObject.SetActive(true); //activate the random child, aka: random decoration
    }
}
