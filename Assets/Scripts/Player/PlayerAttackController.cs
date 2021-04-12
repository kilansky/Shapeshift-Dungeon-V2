using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackController : SingletonPattern<PlayerAttackController>
{
    [SerializeField] private GameObject slashHitbox; //GameObject to hold slash attack hitbox
    [SerializeField] private GameObject thrustHitbox; //GameObject to hold thrust attack hitbox
    [SerializeField] private GameObject radialHitbox; //GameObject to hold radial attack hitbox
    [SerializeField] private GameObject swordImpactPoint; //GameObject to hold point of impact on third attack
    [SerializeField] private bool showHitboxes = false;

    //Activate the slash hitbox - called from attack animation event
    public void ActivateSlashHitbox()
    {
        slashHitbox.GetComponent<MeshCollider>().enabled = true;

        if (showHitboxes)
            slashHitbox.GetComponent<MeshRenderer>().enabled = true;
    }

    //Activate the thrust hitbox - called from attack animation event
    public void ActivateThustHitbox()
    {
        thrustHitbox.GetComponent<MeshCollider>().enabled = true;

        if (showHitboxes)
            thrustHitbox.GetComponent<MeshRenderer>().enabled = true;
    }

    //Activate the radial hitbox - called from attack animation event
    public void ActivateRadialHitbox()
    {
        CineShake.Instance.Shake(2.5f, 0.1f);
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
