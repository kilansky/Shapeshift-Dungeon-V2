using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DestructibleProp : MonoBehaviour
{
    public GameObject destroyEffect;
    public GameObject gemPrefab;
    public Material containsGemMat;
    [Range(0, 100)] public float spawnGemChance;
    public Collider unshatteredCollider;
    public GameObject shatteredProp;

    private bool spawnGem = false;
    public bool HasGem { get { return spawnGem; } }

    private void Start()
    {
        //Spawn Gem
        if (Random.Range(0, 100) < spawnGemChance && LevelManager.Instance.currFloor > 0)
        {
            spawnGem = true;
            GetComponent<MeshRenderer>().material = containsGemMat;
        }
    }

    public void ShatterObject()
    {
        //Spawn destroy effect
        Instantiate(destroyEffect, transform.position + new Vector3(0, 0.7f, 0), Quaternion.identity);

        if(spawnGem)//Spawn a gem
        {
            GameObject gem = Instantiate(gemPrefab, transform.position + new Vector3(0, 0.5f, 0), Quaternion.identity);
            gem.GetComponent<Rigidbody>().AddForce(Vector3.up * 350f);
        }

        //Disable the un-shattered prop
        gameObject.GetComponent<MeshRenderer>().enabled = false;
        gameObject.GetComponent<Collider>().enabled = false;
        gameObject.GetComponent<Rigidbody>().useGravity = false;
        gameObject.GetComponent<NavMeshObstacle>().enabled = false;
        unshatteredCollider.enabled = false;

        //Enable the shattered prop
        shatteredProp.SetActive(true);

        Destroy(gameObject, 12f);
    }
}
