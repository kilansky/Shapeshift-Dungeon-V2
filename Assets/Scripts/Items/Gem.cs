using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gem : MonoBehaviour
{
    [SerializeField] private float spinSpeed;
    [SerializeField] private float disableColliderTime = .5f;

    //public CapsuleCollider collider;
    public CapsuleCollider trigger;

    private void Start()
    {
        StartCoroutine(HitboxCycle());
    }

    private void Update()
    {
        transform.Rotate(0f, 10f * spinSpeed * Time.deltaTime, 0f);

        Physics.Raycast(transform.position, Vector3.down, 0.15f);
    }

    private IEnumerator HitboxCycle()
    {
        yield return new WaitForSeconds(disableColliderTime);
        //collider.enabled = true;
        trigger.enabled = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerController>())
        {
            if (PlayerGems.Instance.GemCount == 0)
                HUDController.Instance.ShowGemCounter();

            PlayerGems.Instance.AddGems(1);
            AudioManager.Instance.Play("CollectGem");
            Destroy(gameObject);
        }
    }
}
