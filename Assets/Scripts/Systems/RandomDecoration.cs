using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomDecoration : MonoBehaviour
{
    [Range(0f, 100f)]public float decorChance;
    public bool mustBePresent;

    public List<GameObject> proplist;

    void Start()
    {
        if(!SpawnRoll())
        {
            gameObject.SetActive(false);
            transform.parent.parent.GetComponent<Tile>().hasDecor = false;
        }
        else
        {
            transform.parent.parent.GetComponent<Tile>().hasDecor = true;
        }
    }

    private bool SpawnRoll()
    {
        return (decorChance >= Random.Range(1, 100) || mustBePresent);
    }
}
