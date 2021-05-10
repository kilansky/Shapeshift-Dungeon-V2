using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WormSpawn : SingletonPattern<WormSpawn>
{
    public float moveUpAmt = 2f;
    public float moveUpTime = 1f;

    private float startYPos;
    private float endYPos;

    public Animator wormAnimator;

    private bool isSpawned = false;

    // Start is called before the first frame update
    void Start()
    {
        startYPos = transform.position.y;
        endYPos = startYPos + moveUpAmt;
    }

    public void SpawnWorm()
    {
        if(!isSpawned)
            StartCoroutine(WormMoveUp());
        else
            StartCoroutine(WormMoveDown());
    }

    private IEnumerator WormMoveUp()
    {
        wormAnimator.SetBool("isSpawning", true);
        float timeElapsed = 0f;
        float wormYPos = startYPos;

        while (timeElapsed < moveUpTime)
        {
            wormYPos = Mathf.Lerp(startYPos, endYPos, timeElapsed / moveUpTime);
            transform.position = new Vector3(transform.position.x, wormYPos, transform.position.z);

            timeElapsed += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        transform.position = new Vector3(transform.position.x, endYPos, transform.position.z);
        isSpawned = true;
        wormAnimator.SetBool("isSpawning", false);
    }

    private IEnumerator WormMoveDown()
    {
        wormAnimator.SetBool("isDespawning", true);
        float timeElapsed = 0f;
        float wormYPos = endYPos;

        while (timeElapsed < moveUpTime)
        {
            wormYPos = Mathf.Lerp(endYPos, startYPos, timeElapsed / moveUpTime);
            transform.position = new Vector3(transform.position.x, wormYPos, transform.position.z);

            timeElapsed += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        transform.position = new Vector3(transform.position.x, startYPos, transform.position.z);
        isSpawned = false;
        wormAnimator.SetBool("isDespawning", false);
    }
}
