using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RabbitScript : MonoBehaviour {
    private Animator animator;
    private Rigidbody rb;
    private Health health;
    public float speed = 3;
    public float wanderSpeed = 3;
    public float escapeSpeed = 10;
    public float damageSpeed;
    public float recoverSpeed;
    public float minBreedDistance;
    public float coolTimeBreeding;
    private float leftTimeForBreeding;
    public GameObject childPrefab;
    public float sight = 10f;
    private float wanderTime;

    private GameObject target;

    public enum RabbitState {
        Dead,
        Escaping,
        Targeting,
        Wandering
    }

    public RabbitState currentState;

    public static string[] predators = {"Tiger", "Eagle", "Iguana"};
    private GameObject predator;

    // Start is called before the first frame update
    void Start() {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        health = GetComponent<Health>();
        animator.speed = speed;
        wanderTime = Random.Range(1.0f, 2.0f);
        currentState = RabbitState.Wandering;
        leftTimeForBreeding = coolTimeBreeding;
    }

    void OnCollisionEnter(Collision other) {
        if (other.gameObject.tag == "Grass") {
            health.currentHealth += Mathf.Min(recoverSpeed, Health.maxHealth - health.currentHealth);
            GameManager.instance.delete(other.gameObject, other.gameObject.tag);
        }
    }

    // Update is called once per frame
    void Update() {
        if (health.currentHealth <= 0) {
            currentState = RabbitState.Dead;
            animator.SetBool("dead", true);
            StartCoroutine(Dissolve(5.0f));
        }

        if (currentState == RabbitState.Dead)
            return;

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
        Collider[] grassColliders = Physics.OverlapSphere(transform.position, sight).Where(coll => coll.tag == "Grass").ToArray();
        if (grassColliders.Length > 0) {
            currentState = RabbitState.Targeting;
            transform.rotation = Quaternion.LookRotation(grassColliders[0].transform.position - transform.position, Vector3.up);
            target = grassColliders[0].gameObject;
        }

        /* Search friend */
        if (currentState == RabbitState.Wandering && leftTimeForBreeding < 0) {
            Collider[] friendColliders = Physics.OverlapSphere(transform.position, sight)
                .Where(coll => coll.tag == this.tag && coll.gameObject != this.gameObject).ToArray();
            if (friendColliders.Length > 0) {
                currentState = RabbitState.Targeting;
                transform.rotation = Quaternion.LookRotation(
                    friendColliders[0].transform.position - transform.position);
                target = friendColliders[0].gameObject;
            }
        }
        leftTimeForBreeding -= Time.deltaTime;

        Collider[] predatorColliders = Physics.OverlapSphere(transform.position, sight).Where(coll => predators.Contains(coll.tag)).ToArray();
        if (predatorColliders.Length > 0) {
            animator.speed = escapeSpeed;
            currentState = RabbitState.Escaping;

            Collider closest = predatorColliders.Aggregate(
                (acc, cur) => (acc.transform.position - transform.position).magnitude < (cur.transform.position - transform.position).magnitude ? acc : cur
            );

            Vector3 diff = transform.position - closest.transform.position;
            transform.rotation = Quaternion.LookRotation(new Vector3(diff.x, 0, diff.z), Vector3.up);
            predator = closest.gameObject;
        } else if (currentState == RabbitState.Escaping) {
            currentState = RabbitState.Wandering;
            animator.speed = wanderSpeed;
        }
    }

    void FixedUpdate() {
        if (currentState == RabbitState.Wandering) {
            if (wanderTime > 0) {
                animator.SetTrigger("moving");
                wanderTime -= Time.deltaTime;
            } else {
                wanderTime = Random.Range(1.0f, 2.0f);
                transform.Rotate(0, Random.Range(-120, 120), 0, Space.World);
            }

        } else if (currentState == RabbitState.Targeting) {
            if (target == null) {
                currentState = RabbitState.Wandering;
                return;
            }

            transform.rotation = Quaternion.LookRotation(target.transform.position - transform.position, Vector3.up);
            animator.SetTrigger("moving");
            tryBreeding();
        } else if (currentState == RabbitState.Escaping) {
            if (predator == null) {
                currentState = RabbitState.Wandering;
                animator.speed = wanderSpeed;
                return;
            }

            Vector3 diff = transform.position - predator.transform.position;
            transform.rotation = Quaternion.LookRotation(new Vector3(diff.x, 0, diff.z), Vector3.up);
            animator.SetTrigger("moving");
        }
        Debug.DrawLine(transform.position, transform.position + transform.forward * 10);
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
