using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructibleProp : MonoBehaviour
{
    public GameObject destroyEffect;
    public GameObject gemPrefab;
    [Range(0, 100)] public float spawnGemChance;

    public void DestroyObject()
    {
        //Spawn destroy effect
        Instantiate(destroyEffect, transform.position + new Vector3(0, 0.7f, 0), Quaternion.identity);

        //Spawn Gem
        if(Random.Range(0,100) < spawnGemChance)
        {
            GameObject gem = Instantiate(gemPrefab, transform.position + new Vector3(0, 0.5f, 0), Quaternion.identity);
            gem.GetComponent<Rigidbody>().AddForce(Vector3.up * 350f);
        }

        Destroy(gameObject);
    }
}
