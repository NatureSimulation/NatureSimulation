using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DeerScript : MonoBehaviour
{
    private Rigidbody rb;
    public float walkspeed = 5;
    public float wanderingSpeed = 5;
    public float escapingSpeed = 20;

    public float damageSpeed;
    public float recoverSpeed;
    public float minBreedDistance;
    public float coolTimeBreeding;
    private float leftTimeForBreeding;
    public GameObject childPrefab;
    public float sight = 10f;
    private Health health;
    private float wanderTime;

    private GameObject target;
    public enum DeerState {
        Dead,
        Escaping,
        Targeting,
        Wandering
    }
    public DeerState currentState;
    public string[] predators = {"Tiger"};
    private GameObject predator;


    void Start()
    {
        rb = GetComponent<Rigidbody>();
        health = GetComponent<Health>();
        wanderTime = Random.Range(1.0f, 2.0f);
        currentState = DeerState.Wandering;
        leftTimeForBreeding = coolTimeBreeding;
    }

    void OnCollisionEnter(Collision other) {
        if (other.gameObject.tag == "Grass") {
            health.currentHealth += Mathf.Min(recoverSpeed, Health.maxHealth - health.currentHealth);
            GameManager.instance.delete(other.gameObject, other.gameObject.tag);
        }
    }

    void Update()
    {
        if (health.currentHealth <= 0) {
            currentState = DeerState.Dead;
            GameManager.instance.delete(this.gameObject, this.tag);
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
        Collider[] grassColliders = Physics.OverlapSphere(transform.position, sight)
            .Where(coll => coll.tag == "Grass").ToArray();

        if (grassColliders.Length > 0) {
            currentState = DeerState.Targeting;
            Vector3 diff = grassColliders[0].transform.position - transform.position;
            transform.rotation = Quaternion.LookRotation(new Vector3(diff.x, 0, diff.z), Vector3.up);
            target = grassColliders[0].gameObject;
        }

        /* Search friend */
        if (currentState == DeerState.Wandering && leftTimeForBreeding < 0) {
            Collider[] friendColliders = Physics.OverlapSphere(transform.position, sight)
                .Where(coll => coll.tag == this.tag && coll.gameObject != this.gameObject).ToArray();
            if (friendColliders.Length > 0) {
                currentState = DeerState.Targeting;
                transform.rotation = Quaternion.LookRotation(
                    friendColliders[0].transform.position - transform.position);
                target = friendColliders[0].gameObject;
            }
        }
        leftTimeForBreeding -= Time.deltaTime;

        Collider[] predatorColliders = Physics.OverlapSphere(transform.position, sight).Where(coll => predators.Contains(coll.tag)).ToArray();
        if (predatorColliders.Length > 0) {
            walkspeed = escapingSpeed;
            currentState = DeerState.Escaping;

            Collider closest = predatorColliders.Aggregate(
                (acc, cur) => (acc.transform.position - transform.position).magnitude < (cur.transform.position - transform.position).magnitude ? acc : cur
            );
            Vector3 diff = transform.position - closest.transform.position;
            transform.rotation = Quaternion.LookRotation(new Vector3(diff.x, 0, diff.z), Vector3.up);
            predator = closest.gameObject;
        } else if (currentState == DeerState.Escaping) {
            currentState = DeerState.Wandering;
            walkspeed = wanderingSpeed;
        }
    }

    void FixedUpdate() {
        if (currentState == DeerState.Wandering) {
            if (wanderTime > 0) {
                transform.Translate(transform.forward * walkspeed * Time.deltaTime, Space.World);
                wanderTime -= Time.deltaTime;
            } else {
                wanderTime = Random.Range(1.0f, 2.0f);
                transform.Rotate(0, Random.Range(-120, 120), 0, Space.World);
            }


        } else if (currentState == DeerState.Targeting) {
            if (target == null) {
                currentState = DeerState.Wandering;
                return;
            }

            Debug.DrawLine(transform.position, target.transform.position, Color.white);
            transform.rotation = Quaternion.LookRotation(target.transform.position - transform.position, Vector3.up);
            transform.Translate(transform.forward * walkspeed * Time.deltaTime);
            if (target.tag == this.tag)
                tryBreeding();
        } else if (currentState == DeerState.Escaping) {
            if (predator == null) {
                currentState = DeerState.Wandering;
                walkspeed = wanderingSpeed;
                return;
            }

            Vector3 diff = transform.position - predator.transform.position;
            transform.rotation = Quaternion.LookRotation(new Vector3(diff.x, 0, diff.z), Vector3.up);
            transform.Translate(transform.forward * walkspeed * Time.deltaTime, Space.World);
        }


        if (health != null)
            health.TakeDamage(damageSpeed);
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
