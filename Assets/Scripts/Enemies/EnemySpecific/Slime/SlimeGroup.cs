using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeGroup : MonoBehaviour
{
    public GameObject bigSlime;
    public List<GameObject> mediumSlimes = new List<GameObject>();
    public List<GameObject> smallSlimes = new List<GameObject>();
    [HideInInspector] public int activeSlimes;

    // Start is called before the first frame update
    void Start()
    {
        activeSlimes = 1;

        //Spawn a small slime from floors 1-5
        if (LevelManager.Instance.currFloor < 5)
            smallSlimes[0].SetActive(true);

        //Spawn a small or medium slime from floors 5-10
        else if (LevelManager.Instance.currFloor < 10)
        {
            float spawnMediumSlime = Random.value;

            if(spawnMediumSlime < 0.5f)
                mediumSlimes[0].SetActive(true);
            else
                smallSlimes[0].SetActive(true);
        }

        //Spawn a medium or big slime from floors 10-15
        else if (LevelManager.Instance.currFloor < 15)
        {
            float spawnBigSlime = Random.value;

            if (spawnBigSlime < 0.4f)
                bigSlime.SetActive(true);
            else
                mediumSlimes[0].SetActive(true);
        }

        else //Always spawn Big slime after floor 15
        {
            bigSlime.SetActive(true);
        }
    }

    public void BigSlimeKilled(Slime slime)
    {
        activeSlimes++;

        mediumSlimes[0].transform.position = slime.transform.position;
        mediumSlimes[1].transform.position = slime.transform.position;
        mediumSlimes[0].SetActive(true);
        mediumSlimes[1].SetActive(true);

        if (slime.GetComponent<GemMonster>().isGemMonster)
            mediumSlimes[0].GetComponent<GemMonster>().SetGemMonster();

            slime.gameObject.SetActive(false);
    }

    public void MediumSlimeKilled(Slime slime)
    {
        activeSlimes++;

        if (slime.gameObject == mediumSlimes[0])
        {
            smallSlimes[0].transform.position = slime.transform.position;
            smallSlimes[1].transform.position = slime.transform.position;
            smallSlimes[0].SetActive(true);
            smallSlimes[1].SetActive(true);

            if (slime.GetComponent<GemMonster>().isGemMonster)
                smallSlimes[0].GetComponent<GemMonster>().SetGemMonster();
        }
        else
        {
            smallSlimes[2].transform.position = slime.transform.position;
            smallSlimes[3].transform.position = slime.transform.position;
            smallSlimes[2].SetActive(true);
            smallSlimes[3].SetActive(true);

            if (slime.GetComponent<GemMonster>().isGemMonster)
                smallSlimes[2].GetComponent<GemMonster>().SetGemMonster();
        }

        slime.gameObject.SetActive(false);
    }

    public void SmallSlimeKilled(Slime slime)
    {
        activeSlimes--;

        if (slime.GetComponent<GemMonster>().isGemMonster)
            slime.DropGem();

        slime.gameObject.SetActive(false);

        //Update the monster count of the room IF this is the last small slime of the group
        if (activeSlimes == 0)
        {
            //Debug.Log("Last slime of a group killed");
            MonsterSpawner.Instance.MonsterKilled();

            //Destroy self from root object
            Destroy(transform.root.gameObject);
        }
    }
}
