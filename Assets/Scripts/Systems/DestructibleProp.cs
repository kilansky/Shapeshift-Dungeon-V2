using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructibleProp : MonoBehaviour
{
    public GameObject destroyEffect;
    public GameObject gemPrefab;
    public Material containsGemMat;
    [Range(0, 100)] public float spawnGemChance;

    private bool spawnGem = false;

    private void Start()
    {
        //Spawn Gem
        if (Random.Range(0, 100) < spawnGemChance)
        {
            spawnGem = true;
            GetComponent<MeshRenderer>().material = containsGemMat;
        }
    }

    public void DestroyObject()
    {
        //Spawn destroy effect
        Instantiate(destroyEffect, transform.position + new Vector3(0, 0.7f, 0), Quaternion.identity);

        if(spawnGem)
        {
            GameObject gem = Instantiate(gemPrefab, transform.position + new Vector3(0, 0.5f, 0), Quaternion.identity);
            gem.GetComponent<Rigidbody>().AddForce(Vector3.up * 350f);
        }

        Destroy(gameObject);
    }
}
