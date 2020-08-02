using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BirdScript : MonoBehaviour
{
    public float walkspeed = 5;
    public float sight = 10f;
    private Health health;
    private float wanderTime;
    private GameObject target;
    public float damageSpeed;
    public float recoverSpeed;
    public float minBreedDistance;
    public float coolTimeBreeding;
    private float leftTimeForBreeding;
    public GameObject childPrefab;
    private enum BirdState {
        Dead,
        Targeting,
        Wandering
    };
    public BirdState currentState;

    void Start()
    {
        health = GetComponent<Health>();
        wanderTime = Random.Range(1.0f, 2.0f);
        currentState = BirdState.Wandering;
        leftTimeForBreeding = coolTimeBreeding;
    }

    void OnCollisionEnter(Collision other) {
        if (other.gameObject.tag == "Squirrel") {
            health.currentHealth += Mathf.Min(recoverSpeed, Health.maxHealth - health.currentHealth);
            GameManager.instance.delete(other.gameObject, other.gameObject.tag);
        }
    }
    void Update()
    {
        if (health.currentHealth <= 0) {
            currentState = BirdState.Dead;
            StartCoroutine(Dissolve(1.0f));
        }

        /* Search wall */
        Collider[] wallColliders = Physics.OverlapSphere(transform.position, sight)
            .Where(coll => coll.tag == "Wall").ToArray();
        if (wallColliders.Length > 0) {
            Quaternion rotation;
            if (wallColliders[0].name == "NorthWall") {
                rotation = Quaternion.Euler(new Vector3(transform.rotation.x, 180, transform.rotation.z));
            } else if (wallColliders[0].name == "SouthWall") {
                rotation = Quaternion.Euler(new Vector3(transform.rotation.x, 0, transform.rotation.z));
            } else if (wallColliders[0].name == "EastWall") {
                rotation = Quaternion.Euler(new Vector3(transform.rotation.x, -90, transform.rotation.z));
            } else {
                rotation = Quaternion.Euler(new Vector3(transform.rotation.x, 90, transform.rotation.z));
            }
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * 2.0f);
        }

        /* Search prey */
        Collider[] colliders = Physics.OverlapSphere(transform.position, sight)
            .Where(coll => coll.tag == "Squirrel").ToArray();
        if (colliders.Length > 0) {
            currentState = BirdState.Targeting;
            transform.rotation = Quaternion.LookRotation(colliders[0].transform.position - transform.position, Vector3.up);
            target = colliders[0].gameObject;
        }

        /* Search friend */
        if (currentState == BirdState.Wandering && leftTimeForBreeding < 0) {
            Collider[] friendColliders = Physics.OverlapSphere(transform.position, sight)
                .Where(coll => coll.tag == this.tag && coll.gameObject != this.gameObject).ToArray();
            if (friendColliders.Length > 0) {
                currentState = BirdState.Targeting;
                transform.rotation = Quaternion.LookRotation(
                    friendColliders[0].transform.position - transform.position);
                target = friendColliders[0].gameObject;
            }
        }
        leftTimeForBreeding -= Time.deltaTime;
    }

    void FixedUpdate() {
        if (currentState == BirdState.Wandering) {
            if (wanderTime > 0) {
                wanderTime -= Time.deltaTime;
            } else {
                wanderTime = Random.Range(1.0f, 2.0f);
                transform.Rotate(0, Random.Range(-120, 120), 0, Space.World);
                return;
            }

            /* Forward */
            float planeY;
            try {
                planeY = GameManager.instance.getHeight(this.transform.position.x, this.transform.position.y);
            } catch {
                planeY = 0;
            }

            Quaternion rotation;
            if (planeY + 5 > this.transform.position.y) {
                rotation = Quaternion.Euler(new Vector3 (-5, transform.rotation.y, transform.rotation.z));
            } else {
                rotation = Quaternion.Euler(new Vector3 (5, transform.rotation.y, transform.rotation.z));
            }
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * 2.0f);

            transform.Translate(transform.forward * walkspeed * Time.deltaTime);
        } else if (currentState == BirdState.Targeting) {
            if (target == null) {
                currentState = BirdState.Wandering;
                return;
            }

            transform.rotation = Quaternion.LookRotation(target.transform.position - transform.position, Vector3.up);
            transform.Translate(transform.forward * walkspeed * Time.deltaTime);

            if (target.tag == this.tag)
                tryBreeding();
        }

        if (health != null)
            health.TakeDamage(damageSpeed);
    }

    IEnumerator Dissolve(float time) {
        yield return new WaitForSeconds(time);
        GameManager.instance.delete(this.gameObject, this.tag);
    }

    void tryBreeding() {
        float distance = (target.transform.position - transform.position).magnitude;
        if (target.tag == this.tag && distance < minBreedDistance && leftTimeForBreeding < 0) {
            float x = this.transform.position.x + Random.Range(-20, 20);
            float z = this.transform.position.z + Random.Range(-20, 20);
            float y;
            try {
                y = GameManager.instance.getHeight(x, z);
            } catch (System.Exception) {
                return;
            }

            GameObject child = Instantiate(childPrefab, new Vector3(x, y, z), Quaternion.identity);
            GameManager.instance.breed(child.tag);
            leftTimeForBreeding = coolTimeBreeding;
        }
    }
}
