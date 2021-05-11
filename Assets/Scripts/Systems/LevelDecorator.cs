using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AdjustableDecor
{
    public Decoration.decorTypes type;
    [Range(0f, 100f)] public float spawnChance;
}

public class LevelDecorator : MonoBehaviour
{
    /*
     * Script: LevelDecorator
     * Programmer: Justin Donato
     * Description: Handles the behaviors of individual tiles
     * Date Created: 4/3/2021
     * Date Last Edited: 4/4/2021
     */

    public List<AdjustableDecor> propList;

    private List<Transform> props = new List<Transform>();
    private List<Transform> propsToRemove = new List<Transform>();

    // Start is called before the first frame update
    void Start()
    {
        GetAllProps();

        SpawnProps();
    }

    private void GetAllProps()
    {
        //propArray = GetComponentsInChildren<Transform>();
        foreach (Transform child in GetComponentsInChildren<Transform>())
        {
            if (child.GetComponent<Decoration>())
                props.Add(child);
        }
    }

    private void SpawnProps()
    {
        foreach(AdjustableDecor decorType in propList)
        {
            //Debug.Log("There are currently " + props.Count + " props in the list to scan");
            foreach(Transform prop in props)
            {
                if(prop.GetComponent<Decoration>().propType == decorType.type)
                {
                    if (!Roll(decorType.spawnChance))
                    {
                        //props.Remove(prop);
                        //Destroy(prop.gameObject);
                        prop.gameObject.SetActive(false);                        
                    }
                    else
                    {
                        prop.GetComponent<Decoration>().SetTileVariables();
                    }
                    propsToRemove.Add(prop);
                }
            }

            foreach(Transform prop in propsToRemove)
            {
                props.Remove(prop);
            }
        }

        if(props.Count > 0)
        {
            //Debug.LogError("Unassigned prop types found! Disabling all unasigned props");
            foreach(Transform prop in props)
            {
                Debug.LogWarning(prop.GetComponent<Decoration>().propType + " has been disabled at " + prop.position);
                prop.gameObject.SetActive(false);
            }
        }
    }

    private bool Roll(float chance)
    {
        float random = Random.Range(1f, 100f);
        if (chance >= random)
        {
            //Debug.Log("Successful roll of " + random + " out of " + chance);
            return true;
        }
        else
        {
            //Debug.Log("Unsuccessful roll of " + random + " out of " + chance);
            return false;
        }
    }
}
