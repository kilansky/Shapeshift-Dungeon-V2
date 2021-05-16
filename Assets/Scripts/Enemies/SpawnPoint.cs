using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    public float spawnTime = 2.5f;
    public GameObject spawnCircle;
    public GameObject spawnVFX;
    public Vector3 offset;
    [HideInInspector] public bool onAtStart = false;

    //Spawns a monster on a tile (called from MonsterSpawner)
    public void SpawnMonster(GameObject monsterToSpawn, bool isGemMonster)
    {
        Instantiate(spawnCircle, transform.position + offset, Quaternion.identity);
        StartCoroutine(WaitToSpawn(monsterToSpawn, isGemMonster));
    }

    private IEnumerator WaitToSpawn(GameObject monsterToSpawn, bool isGemMonster)
    {
        yield return new WaitForSeconds(spawnTime);

        GameObject monster = Instantiate(monsterToSpawn, transform.position + offset, Quaternion.identity);
        Instantiate(spawnVFX, monster.transform.position + new Vector3(0, 1.5f, 0), Quaternion.identity);
        GetComponent<AudioSource>().Play();

        //Set monster to gem monster
        if (isGemMonster && monster && monster.GetComponent<GemMonster>())
            monster.GetComponent<GemMonster>().SetGemMonster();
    }
}
