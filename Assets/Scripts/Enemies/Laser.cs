using System.Collections;
using System.Collections.Generic;
using UnityEngine.VFX;
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
    public float length = 7f;
    public float damage = 1f;
    public float tickRate = .4f;
    public bool isStartupLaser = false;

    [Header("Misc")]
    public GameObject laser;
    public LayerMask mask;
    public GameObject hitFX;

    [Header("Special Properties")]
    public bool setOnFire;
    public bool heal;

    private LineRenderer beam;
    private CapsuleCollider capsuleCollider;
    private VisualEffect hitEffect;

    [HideInInspector] public GameObject parentObject; //Variable to hold the parent Object to make sure the Damage Tracker is able to track the damage correctly
    [HideInInspector] public bool laserTriggered = false;

    private void Start()
    {
        beam = laser.gameObject.GetComponent<LineRenderer>();
        capsuleCollider = GetComponent<CapsuleCollider>();

        parentObject = transform.parent.gameObject;

        //If statement to adjust some values if this is used for the player weapon
        if(parentObject.GetComponent<PlayerController>())
        {
            damage = 5f;
            Destroy(gameObject, 0.3f);
        }

        if(!isStartupLaser)
        {
            hitEffect = hitFX.GetComponent<VisualEffect>();
            StartCoroutine(PlayFX());
        }

        if (heal)
            tickRate = 0.6f;
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
            length = hit.distance;
        }
    }

    /// <summary>
    /// Updates the laser's length and hitbox
    /// </summary>
    private void UpdateLaser()
    {
        beam.SetPosition(1, new Vector3(0, 0, length));
        capsuleCollider.center = Vector3.forward * length / 2;
        capsuleCollider.height = length;

        if(!isStartupLaser)
            hitFX.transform.localPosition = new Vector3(0, 0, length);
    }

    /// <summary>
    /// Detects when something enters the laser's collider
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerStay(Collider other)
    {
        if(!laserTriggered && other.transform != parentObject.transform)
        {
            if (!parentObject.GetComponent<PlayerController>() && !PlayerHealth.Instance.isInvincible && damage > 0)
                StartCoroutine(LaserCycle(other.gameObject));

            if (parentObject.GetComponent<PlayerController>() && !other.GetComponent<EnemyBase>().isInvincible && heal)
            {
                print("Hit an enemey for the enemy!");
                StartCoroutine(LaserCycle(other.gameObject));
            }
                
        }
    }

    /// <summary>
    /// Deals damage to certain objects that need to take damage
    /// </summary>
    /// <param name="target"></param>
    /// <returns></returns>
    private IEnumerator LaserCycle(GameObject target)
    {
        laserTriggered = true;

        //if (!parentObject.GetComponent<PlayerController>())
        //{
            if (target.GetComponent<PlayerController>() && !PlayerController.Instance.IsDashing)
            {
                if (!PlayerHealth.Instance.isInvincible)
                    AnalyticsEvents.Instance.PlayerDamaged("Laser"); //Sends analytics event about damage source

                if(!heal)
                    PlayerHealth.Instance.Damage(damage, parentObject);

                if (setOnFire)
                    PlayerHealth.Instance.transform.GetComponent<StatusEffects>().fireStatus(3f);

                yield return new WaitForSeconds(tickRate);
            }
        //}

        //If the other object is Monster (Contains the enemy base script) then go on with the rest of the damage then destroys itself
        if (target.GetComponent<EnemyBase>())
        {
            if (heal)
                target.GetComponent<EnemyBase>().Heal(1);
            else if(!parentObject.GetComponent<EnemyBase>())
            {
                //Starts the knockback coroutine
                StartCoroutine(target.GetComponent<EnemyBase>().EnemyKnockBack());
                target.GetComponent<EnemyBase>().Damage(damage);

                /*if (setOnFire)
                    target.GetComponent<EnemyBase>().transform.GetComponent<StatusEffects>().fireStatus(3f);*/
            }

            yield return new WaitForSeconds(tickRate);
        }

        laserTriggered = false;
    }

    private IEnumerator PlayFX()
    {
        hitEffect.Play();
        yield return new WaitForSeconds(.15f);
        StartCoroutine(PlayFX());
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
