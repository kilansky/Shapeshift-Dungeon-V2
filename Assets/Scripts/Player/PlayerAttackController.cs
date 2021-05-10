using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackController : SingletonPattern<PlayerAttackController>
{
    [Header("Hitboxes")]
    [SerializeField] private GameObject slashHitbox; //GameObject to hold slash attack hitbox
    [SerializeField] private GameObject thrustHitbox; //GameObject to hold thrust attack hitbox
    [SerializeField] private GameObject dashHitbox; //GameObject to hold dash attack hitbox
    [SerializeField] private GameObject radialHitbox; //GameObject to hold radial attack hitbox
    [SerializeField] private GameObject swordImpactPoint; //GameObject to hold point of impact on third attack
    [SerializeField] private bool showHitboxes = false;

    [Header("VFX")]
    [SerializeField] private Transform sword; //Transform to hold sword position/rotation info
    [SerializeField] private ParticleSystem attack1VFX; //GameObject to hold slash vfx1
    [SerializeField] private ParticleSystem attack2VFX; //GameObject to hold slash vfx2
    [SerializeField] private ParticleSystem[] attack3VFX; //GameObject to hold slash vfx3
    [SerializeField] private ParticleSystem[] thrustVFX; //GameObject to hold thrust vfx

    private Transform player;

    private void Start()
    {
        player = PlayerController.Instance.transform;
    }

    //Activate the slash hitbox - called from attack animation event
    public void ActivateSlashHitbox()
    {
        DeactivateThrustHitbox();

        slashHitbox.GetComponent<MeshCollider>().enabled = true;
        AudioManager.Instance.Play("Swing1");

        if (showHitboxes)
            slashHitbox.GetComponent<MeshRenderer>().enabled = true;

        if (attack1VFX)
            attack1VFX.Play();
    }

    public void ActivateSlashHitbox2()
    {
        DeactivateThrustHitbox();

        slashHitbox.GetComponent<MeshCollider>().enabled = true;
        AudioManager.Instance.Play("Swing2");

        if (showHitboxes)
            slashHitbox.GetComponent<MeshRenderer>().enabled = true;

        if (attack2VFX)
            attack2VFX.Play();
    }

    //Activate the thrust hitbox - called from attack animation event
    public void ActivateThustHitbox()
    {
        DeactivateSlashHitbox();

        thrustHitbox.GetComponent<MeshCollider>().enabled = true;
        AudioManager.Instance.Play("Swing4");

        if (showHitboxes)
            thrustHitbox.GetComponent<MeshRenderer>().enabled = true;

        //Play all the vfx of this attack from the impact point
        foreach (ParticleSystem vfx in thrustVFX)
            vfx.Play();
    }

    //Activate the radial hitbox - called from attack animation event
    public void ActivateRadialHitbox()
    {
        CineShake.Instance.Shake(3f, 0.15f);
        AudioManager.Instance.Play("Swing3");
        StartCoroutine(LerpRadialHitbox());
    }

    //Activate the dash hitbox - called from attack animation event
    public void ActivateDashHitbox()
    {
        DeactivateDashHitbox();

        dashHitbox.GetComponent<CapsuleCollider>().enabled = true;

        if (showHitboxes)
            dashHitbox.GetComponent<MeshRenderer>().enabled = true;
    }

    //Scales a circular wave of damage to hit enemies in a radius
    private IEnumerator LerpRadialHitbox()
    {
        //Set starting position of damage radius to the impact point of the sword
        radialHitbox.transform.position = swordImpactPoint.transform.position;

        //Set up scaling variables
        float attack3HitboxScale;
        float hitboxOriginalScale = radialHitbox.transform.localScale.x;
        float hitboxMinScale = hitboxOriginalScale / 100;
        radialHitbox.transform.localScale = new Vector3(hitboxMinScale, 1, hitboxMinScale); //Shrink scale to min size

        //Enable the hitbox
        radialHitbox.GetComponent<SphereCollider>().enabled = true;
        if (showHitboxes)
            radialHitbox.GetComponent<MeshRenderer>().enabled = true;

        //Play all the vfx of this attack from the impact point
        foreach (ParticleSystem vfx in attack3VFX)
        {
            vfx.gameObject.transform.position = swordImpactPoint.transform.position;
            vfx.Play();
        }

        //Lerp the damage wave to increase in scale over time
        float timeElapsed = 0;
        float duration = .4f;
        while (timeElapsed < duration)
        {
            attack3HitboxScale = Mathf.Lerp(hitboxMinScale, hitboxOriginalScale, timeElapsed / duration);
            radialHitbox.transform.localScale = new Vector3(attack3HitboxScale, 1, attack3HitboxScale);

            timeElapsed += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        //disable the hitbox
        radialHitbox.GetComponent<SphereCollider>().enabled = false;
        if (showHitboxes)
            radialHitbox.GetComponent<MeshRenderer>().enabled = false;

        //reset the scale
        radialHitbox.transform.localScale = new Vector3(hitboxOriginalScale, 1, hitboxOriginalScale); //reset scale
    }

    //Disable the slash hitbox - called from attack animation event
    public void DeactivateSlashHitbox()
    {
        slashHitbox.GetComponent<MeshCollider>().enabled = false;

        if (showHitboxes)
            slashHitbox.GetComponent<MeshRenderer>().enabled = false;
    }

    //Disable the thrust hitbox - called from attack animation event
    public void DeactivateThrustHitbox()
    {
        thrustHitbox.GetComponent<MeshCollider>().enabled = false;

        if (showHitboxes)
            thrustHitbox.GetComponent<MeshRenderer>().enabled = false;
    }

    //Disable the dash hitbox - called from attack animation event
    public void DeactivateDashHitbox()
    {
        dashHitbox.GetComponent<CapsuleCollider>().enabled = false;

        if (showHitboxes)
            dashHitbox.GetComponent<MeshRenderer>().enabled = false;
    }

    public void DeactivateAllHitboxes()
    {
        DeactivateSlashHitbox();
        DeactivateThrustHitbox();
    }
}
