using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    /*
     * Script: Laser
     * Programmer: Justin Donato and Joe Warren
     * Description: Handles behaviors of laser objects
     * Date Created: 3/23/2021
     * Date Last Edited: 3/23/2021
     */

    [Header("Laser Parameters")]
    public float legnth = 7f;
    public float damage = 1f;
    public float tickRate = .4f;

    [Header("Misc")]
    public GameObject laser;
    public LayerMask mask;

    private LineRenderer beam;
    private CapsuleCollider capsuleCollider;

    [HideInInspector] public GameObject parentObject; //Variable to hold the parent Object to make sure the Damage Tracker is able to track the damage correctly

    private void Start()
    {
        beam = laser.gameObject.GetComponent<LineRenderer>();
        capsuleCollider = GetComponent<CapsuleCollider>();

        //If statement to adjust some values if this is used for the player weapon
        if(parentObject.name == "Player")
        {
            damage = 4f;
            Destroy(gameObject, 0.3f);
        }
    }

    private void FixedUpdate()
    {
        SetLaserLength();
    }

    private void Update()
    {
        UpdateLaser();
    }

    /// <summary>
    /// Shoots a raycast to determine the length of the laser
    /// </summary>
    private void SetLaserLength()
    {
        RaycastHit hit;

        if(Physics.Raycast(laser.transform.position, laser.transform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity, mask))
        {
            //Debug.Log("Hit " + hit.transform.gameObject.name);
            legnth = hit.distance;
        }
    }

    /// <summary>
    /// Updates the laser's length and hitbox
    /// </summary>
    private void UpdateLaser()
    {
        beam.SetPosition(1, new Vector3(0, 0, legnth));
        capsuleCollider.center = Vector3.forward * legnth / 2;
        capsuleCollider.height = legnth;
    }

    /// <summary>
    /// Detects when something enters the laser's collider
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        if(parentObject.name == "Player")
            StartCoroutine(LaserCycle(other.gameObject));
    }

    /// <summary>
    /// Detects when something enters the laser's collider
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerStay(Collider other)
    {
        if (parentObject.name != "Player")
            StartCoroutine(LaserCycle(other.gameObject));
    }

    /// <summary>
    /// Deals damage to certain objects that need to take damage
    /// </summary>
    /// <param name="target"></param>
    /// <returns></returns>
    private IEnumerator LaserCycle(GameObject target)
    {
        if(parentObject.name != "Player")
        {
            if (target.GetComponent<PlayerController>() && !PlayerController.Instance.IsDashing)
            {
                if (!PlayerHealth.Instance.isInvincible)
                    AnalyticsEvents.Instance.PlayerDamaged("Laser"); //Sends analytics event about damage source

                PlayerHealth.Instance.Damage(damage, parentObject);
                yield return new WaitForSeconds(tickRate);
            }
        }

        else
        {
            //If the other object is Monster (Contains the enemy base script) then go on with the rest of the damage then destroys itself
            if (target.GetComponent<EnemyBase>())
            {
                target.GetComponent<EnemyBase>().Damage(damage);
            }
        }
        
    }

    /// <summary>
    /// Sets the scales of all the vfx effects based on a given percentage
    /// </summary>
    /// <param name="percent"></param>
    public void SetVFXScale(float percent)
    {
        float scaleFactor = percent / 100f;

        foreach(Transform child in transform)
        {
            child.transform.localScale = new Vector3(1, 1, 1) * scaleFactor;
        }
    }
}
