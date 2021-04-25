using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class ExplodingBarrel : MonoBehaviour
{
    [Header("Parameters")]
    public float fuseTime = 3f;
    public int warningCycles = 3;
    public bool detonateOnStart = false;

    [Header("Pointers")]
    public GameObject explosion;
    public GameObject explosionRadius;
    public GameObject fireFX;

    private Renderer warningRenderer;
    private Image radiusIndicator;
    private bool isExploding = false;

    private void Start()
    {
        radiusIndicator = explosionRadius.GetComponent<Image>();
        radiusIndicator.color = new Color(1, 0, 0, 0);
        if (detonateOnStart)
            TriggerFuse();
    }

    [ContextMenu("Trigger Fuse")]
    public void TriggerFuse()
    {
        if (!isExploding)
        {
            StartCoroutine(FuseCycle());
            isExploding = true;
        }
        else
            Debug.Log("Already exploding");
    }

    private IEnumerator FuseCycle()
    {
        fireFX.SetActive(true);
        float divTime = fuseTime / (2 * warningCycles);
        float counter = 0f; //Counter to keep track of time elapsed
        for (int i = 0; i < warningCycles; i++)
        {
            radiusIndicator.color = new Color(1, 0f, 0f, 0);
            counter = 0;
            while (counter < divTime) //This while loop moves the object to new position over a set amount of time
            {
                counter += Time.deltaTime;
                float a = Mathf.Lerp(0, 1, counter / divTime);
                radiusIndicator.color = new Color(1, 0f, 0f, 0f + a);
                //Debug.Log(radiusIndicator.color);
                yield return null;
            }
            radiusIndicator.color = new Color(1, 0f, 0f, 1);
            counter = 0;
            while (counter < divTime) //This while loop moves the object to new position over a set amount of time
            {
                counter += Time.deltaTime;
                float a = Mathf.Lerp(0, 1, counter / divTime);
                radiusIndicator.color = new Color(1, 0f, 0f, 1 - a);
                yield return null;
            }
            radiusIndicator.color = new Color(1, 0f, 0f, 0f);
        }

        GetComponent<MeshRenderer>().enabled = false;
        if(GetComponent<CapsuleCollider>())
            GetComponent<CapsuleCollider>().enabled = false;
        fireFX.SetActive(false);
        explosion.SetActive(true);
    }
}
