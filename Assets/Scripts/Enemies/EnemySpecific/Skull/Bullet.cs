using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    //script for bullet interactions/stats
    //should be able to create custom projectile
    //prefabs from this one script

    [Range(0f, 1f)]
    public bool canDamage = false;
    public float moveSpeed = 20f;

    //explosion variables
    public float bulletDamage = 5f;

    //bullet charge time
    public float minBulletSize;
    public float maxBulletSize;
    //public string shotBy; //used for damage type analytics - REMOVE LATER

    [Header("VFX Pointers")]
    public GameObject beamLight;
    public GameObject beamDark;
    public GameObject baseVar;
    public GameObject glow;
    public GameObject sparks;
    public GameObject shell;
    public GameObject voidVar;
    public GameObject line;

    [Header("Special Properties")]
    public bool setOnFire;

    private Vector3 original_beamLight;
    private Vector3 original_beamDark;
    private Vector3 original_baseVar;
    private Vector3 original_glow;
    private Vector3 original_sparks;
    private Vector3 original_shell;
    private Vector3 original_voidVar;
    private Vector3 original_line;

    private Vector3 shootDir;

    [HideInInspector] public GameObject parentObject; //Variable to hold the parent Object to make sure the Damage Tracker is able to track the damage correctly

    private void Start()
    {
        original_beamLight = beamLight.transform.localScale;
        original_beamDark = beamDark.transform.localScale;
        original_baseVar = baseVar.transform.localScale;
        original_glow = glow.transform.localScale;
        original_sparks = sparks.transform.localScale;
        original_shell = shell.transform.localScale;
        original_voidVar = voidVar.transform.localScale;
        original_line = line.transform.localScale;

        //Debug.Log("Original scale of beamLight is " + original_beamLight);
        Setup();
        Destroy(gameObject, 10f); //destroy this projectile if in the scene for 10 seconds
    }

    private void Update()
    {
        transform.position += shootDir * moveSpeed * Time.deltaTime;
    }

    //Check if player was hit & deal damage
    private void OnTriggerEnter(Collider collider)
    {
        if (canDamage && collider.GetComponent<PlayerController>())
        {
            //if (!PlayerHealth.Instance.isInvincible)
                //AnalyticsEvents.Instance.PlayerDamaged(shotBy + " Projectile"); //Sends analytics event about damage source

            PlayerHealth.Instance.Damage(bulletDamage, parentObject);

            if (setOnFire)
                PlayerHealth.Instance.transform.GetComponent<StatusEffects>().fireStatus(3f);

            Destroy(gameObject);
        }

        if(collider.GetComponent<ExplodingBarrel>())
        {
            collider.GetComponent<ExplodingBarrel>().TriggerFuse();
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        //destroy the bullet if it hits the environment, walls, the player, or stairs
        if(canDamage && collision.gameObject.layer == 10 || collision.gameObject.layer == 9 || collision.gameObject.layer == 8 || collision.gameObject.layer == 2)
        {
            Destroy(gameObject);
        }

        //if the bullet hit another enemy, damage the enemy & destroy the bullet
        if (canDamage && collision.gameObject.layer == 11)
        {
            collision.gameObject.GetComponent<EnemyBase>().Damage(bulletDamage);

            if (setOnFire)
                collision.gameObject.GetComponent<StatusEffects>().fireStatus(3f);

            Destroy(gameObject);
        }
    }

    private void Setup()
    {
        shootDir = transform.forward;

        //implements bullet facing direction
        transform.eulerAngles = new Vector3(0, 0, GetAngleFromVectorFloat(shootDir));
    }

    //need this to change bullet facing direction
    public static float GetAngleFromVectorFloat(Vector3 dir)
    {
        dir = dir.normalized;
        float n = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        if(n < 0)
        {
            n += 360;
        }
        return n;
    }

    /// <summary>
    /// Sets the scales of all the vfx effects based on a given percentage
    /// </summary>
    /// <param name="percent"></param>
    public void SetVFXScale(float percent)
    {
        float scaleFactor = percent / 100f;

        beamLight.transform.localScale = original_beamLight * scaleFactor;
        beamDark.transform.localScale = original_beamDark * scaleFactor;
        baseVar.transform.localScale = original_baseVar * scaleFactor;
        glow.transform.localScale = original_glow * scaleFactor;
        sparks.transform.localScale = original_sparks * scaleFactor;
        shell.transform.localScale = original_shell * scaleFactor;
        voidVar.transform.localScale = original_voidVar * scaleFactor;
        line.transform.localScale = original_line * scaleFactor;

        //Debug.Log("Set scale of VFX to " + scaleFactor);
    }
}
