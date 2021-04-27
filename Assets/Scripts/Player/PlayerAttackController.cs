using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackController : SingletonPattern<PlayerAttackController>
{
    [Header("Hitboxes")]
    [SerializeField] private GameObject slashHitbox; //GameObject to hold slash attack hitbox
    [SerializeField] private GameObject thrustHitbox; //GameObject to hold thrust attack hitbox
    [SerializeField] private GameObject radialHitbox; //GameObject to hold radial attack hitbox
    [SerializeField] private GameObject swordImpactPoint; //GameObject to hold point of impact on third attack
    [SerializeField] private bool showHitboxes = false;

    [Header("VFX")]
    [SerializeField] private Transform sword; //Transform to hold sword position/rotation info
    [SerializeField] private GameObject slashVFX1; //GameObject to hold slash vfx1
    [SerializeField] private GameObject slashVFX2; //GameObject to hold slash vfx2

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

        //Player slash vfx
        Vector3 vfxSpawnPos = new Vector3(player.position.x, slashHitbox.transform.position.y, player.position.z);
        GameObject vfx = Instantiate(slashVFX1, vfxSpawnPos, player.rotation, player);
        vfx.transform.Rotate(90, 0, 42.571f);
        Destroy(vfx, 1);
    }

    public void ActivateSlashHitbox2()
    {
        DeactivateThrustHitbox();

        slashHitbox.GetComponent<MeshCollider>().enabled = true;
        AudioManager.Instance.Play("Swing2");

        if (showHitboxes)
            slashHitbox.GetComponent<MeshRenderer>().enabled = true;

        Vector3 vfxSpawnPos = new Vector3(player.position.x, slashHitbox.transform.position.y, player.position.z);
        GameObject vfx = Instantiate(slashVFX2, vfxSpawnPos, player.rotation, player);
        vfx.transform.Rotate(90, 0, 103.16f); //42.571f 103.16f
        //vfx.transform.localScale = new Vector3(vfx.transform.localScale.x, vfx.transform.localScale.y * -1f, vfx.transform.localScale.z);
        Destroy(vfx, 1);
    }

    //Activate the thrust hitbox - called from attack animation event
    public void ActivateThustHitbox()
    {
        DeactivateSlashHitbox();

        thrustHitbox.GetComponent<MeshCollider>().enabled = true;

        if (showHitboxes)
            thrustHitbox.GetComponent<MeshRenderer>().enabled = true;
    }

    //Activate the radial hitbox - called from attack animation event
    public void ActivateRadialHitbox()
    {
        CineShake.Instance.Shake(3f, 0.15f);
        AudioManager.Instance.Play("Swing3");
        StartCoroutine(LerpRadialHitbox());
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

        //Lerp the damage wave to increase in scale over time
        float timeElapsed = 0;
        float duration = .25f;
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

    public void DeactivateAllHitboxes()
    {
        DeactivateSlashHitbox();
        DeactivateThrustHitbox();
    }
}
