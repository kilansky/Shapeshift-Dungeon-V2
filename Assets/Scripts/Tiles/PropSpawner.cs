using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropSpawner : MonoBehaviour
{
    /*
     * Script: PropSpawner
     * Programmer: Justin Donato and Joe Warren
     * Description: Figures out if a tile will spawn with a prop and which prop it will be
     * Date Created: 3/28/2021
     * Date Last Edited: 3/28/2021
     */

    [Range(0f, 100f)] public float propChance;
    public GameObject propHolder;

    private void Start()
    {
        foreach (Transform child in propHolder.transform)
        {
            child.gameObject.SetActive(false);
        }
    }

    public void SpawnProp()
    {
        if(HasProp())
        {
            List<GameObject> propList = new List<GameObject>();
            foreach(Transform child in propHolder.transform)
            {
                propList.Add(child.gameObject);
            }
            if (propList.Count == 0)
            { }
                //Debug.LogError("No props in prop list!");
            else
                propList[Random.Range(0, propList.Count)].SetActive(true);
        }
    }

    private bool HasProp()
    {
        if (propChance == 0f)
            return false;
        else if (propChance == 100f)
            return true;
        else
            return (propChance >= Random.Range(0f, 100f));
    }

    [ContextMenu("Test RNG")]
    private void Test()
    {
        for (int i = 0; i < 1000; i++)
        {
            SpawnProp();
        }
    }
}
