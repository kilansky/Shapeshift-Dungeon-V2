using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BowllingAttack : MonoBehaviour
{
    //Variable Initialization/Declaration
    public float damage; //Damage variable to adjust the damage the enemy takes when it gets hit
    //public float destroyTime; //This is the time that it will take before the bowling ball gets destroyed
    public float speed; //The speed that the bowling ball will go at
    public LayerMask tileLayer; //Public variable to keep track of the environment layer
    public GameObject hitEffect;
    private float gravity; //Gravity!

    private void Start()
    {
        Destroy(gameObject, 8f);
    }

    /// <summary>
    /// OnTrigger so when it hits an enemy it will damage them - AHL (3/4/21)
    /// </summary>
    private void OnTriggerEnter(Collider other)
    {
        //If the other object is Monster (Contains the enemy base script) then go on with the rest of the damage
        if (other.GetComponent<EnemyBase>() && !other.GetComponent<EnemyBase>().isInvincible)
        {
            //Debug.Log(other.gameObject);
            if (other.GetComponent<Skeleton>())
            {
                if (other.GetComponent<Skeleton>().isBlocking)
                    AudioManager.Instance.Play("Block");
                else if (!other.GetComponent<EnemyBase>().isInvincible)
                    AudioManager.Instance.Play("BowlingStrike");
            }
            
            else if (!other.GetComponent<EnemyBase>().isInvincible)
                AudioManager.Instance.Play("BowlingStrike");

            //Spawn hit effect on enemy
            Vector3 enemyPos = other.transform.position;
            Instantiate(hitEffect, new Vector3(enemyPos.x, transform.position.y, enemyPos.z), Quaternion.identity);

            //Apply slight camera shake
            CineShake.Instance.Shake(1f, 0.1f);

            //Apply damage to enemy
            other.GetComponent<EnemyBase>().Damage(damage);

            //Apply Knockback to enemy
            StartCoroutine(other.GetComponent<EnemyBase>().EnemyKnockBack());
        }

        if (other.GetComponent<DestructibleProp>())
        {
            //Apply slight camera shake
            CineShake.Instance.Shake(1f, 0.1f);

            //Destroy Prop
            other.GetComponent<DestructibleProp>().ShatterObject();
        }

        if (other.GetComponent<ExplodingBarrel>())
        {
            other.GetComponent<ExplodingBarrel>().TriggerFuse();
        }

        //If the bowling ball hits a wall layer then it deletes itself
        if (other.gameObject.layer == 9)
        {
            Destroy(gameObject);
        }
    }

    //Update is called once a frame
    private void Update()
    {
        //If the ball is not over a pit then it moves forward
        if (IsAboveEnvironment())
            gravity = 0;

        //If the ball is over a pit then it moves downward into the pit itself
        else
            gravity = 9.81f/2;

        transform.Translate(Vector3.forward * speed * Time.deltaTime); //Every frame the bowling ball will move forward
        transform.Translate(Vector3.down * gravity * Time.deltaTime); //Every frame the bowling ball will move down

        //And rotate forward - Sky (3/29/21)
        //transform.RotateAround(transform.position, Vector3.right, speed * Time.deltaTime);
    }

    //Returns true if the bowling ball above a pit or lower floor - AHL (5/10/21)
    private bool IsAboveEnvironment()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 1f, tileLayer))
        {
            if (hit.transform.GetComponent<Tile>() && hit.transform.GetComponent<Tile>().tileType != Tile.tileTypes.pit)
                return true;
        }

        return false;
    }
}
