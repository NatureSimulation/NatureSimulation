using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FrogScript : MonoBehaviour {
    private Animator animator;
    private Rigidbody rb;
    private Health health;
    public float damageSpeed;
    public float recoverSpeed;
    public float minBreedDistance;
    public float coolTimeBreeding;
    private float leftTimeForBreeding;
    public GameObject childPrefab;
    public float sight = 10f;
    private float wanderTime;

    public float speed = 10f;
    public float wanderSpeed = 10f;
    public float escapeSpeed = 20f;

    private GameObject target;

    public enum FrogState {
        Dead,
        Escaping,
        Targeting,
        Wandering
    }

    public FrogState currentState;
    public static string[] predators = {"Eagle"};
    private GameObject predator;

    // Start is called before the first frame update
    void Start() {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        health = GetComponent<Health>();
        animator.speed = 3f;
        wanderTime = UnityEngine.Random.Range(1.0f, 2.0f);
        currentState = FrogState.Wandering;
        leftTimeForBreeding = coolTimeBreeding;
    }

    void OnCollisionEnter(Collision other) {
        if (other.gameObject.tag == "Butterfly") {
            health.currentHealth += Mathf.Min(recoverSpeed, Health.maxHealth - health.currentHealth);
            GameManager.instance.delete(other.gameObject, other.gameObject.tag);
        }
    }

    // Update is called once per frame
    void Update() {
        if (health.currentHealth <= 0) {
            currentState = FrogState.Dead;
            StartCoroutine(Dissolve(1.0f));
        }

        if (currentState == FrogState.Dead)
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
        Collider[] preyColliders = Physics.OverlapSphere(transform.position, sight).Where(coll => coll.tag == "Butterfly").ToArray();
        if (preyColliders.Length > 0) {
            currentState = FrogState.Targeting;
            transform.rotation = Quaternion.LookRotation(preyColliders[0].transform.position - transform.position, Vector3.up);
            target = preyColliders[0].gameObject;
        }

        /* Search friend */
        if (currentState == FrogState.Wandering && leftTimeForBreeding < 0) {
            Collider[] friendColliders = Physics.OverlapSphere(transform.position, sight)
                .Where(coll => coll.tag == this.tag && coll.gameObject != this.gameObject).ToArray();
            if (friendColliders.Length > 0) {
                currentState = FrogState.Targeting;
                transform.rotation = Quaternion.LookRotation(
                    friendColliders[0].transform.position - transform.position);
                target = friendColliders[0].gameObject;
            }
        }
        leftTimeForBreeding -= Time.deltaTime;

        Collider[] predatorColliders = Physics.OverlapSphere(transform.position, sight).Where(coll => predators.Contains(coll.tag)).ToArray();
        if (predatorColliders.Length > 0) {
            speed = escapeSpeed;
            currentState = FrogState.Escaping;

            Collider closest = predatorColliders.Aggregate(
                (acc, cur) => (acc.transform.position - transform.position).magnitude < (cur.transform.position - transform.position).magnitude ? acc : cur
            );
            transform.rotation = Quaternion.LookRotation(transform.position - closest.transform.position, Vector3.up);
            predator = closest.gameObject;
        } else if (currentState == FrogState.Escaping) {
            currentState = FrogState.Wandering;
            speed = wanderSpeed;
        }
    }

    void FixedUpdate() {
        if (currentState == FrogState.Wandering) {
            if (wanderTime > 0) {
                wanderTime -= Time.deltaTime;
            } else {
                wanderTime = UnityEngine.Random.Range(1.0f, 2.0f);
                transform.Rotate(0, UnityEngine.Random.Range(-120, 120), 0, Space.World);
            }

            animator.SetTrigger("move");
            transform.position += (transform.forward * speed * Time.deltaTime);

        } else if (currentState == FrogState.Targeting) {
            if (target == null) {
                currentState = FrogState.Wandering;
                return;
            }

            if ((target.transform.position - transform.position).magnitude < 10) {
                animator.SetTrigger("tongue");
                GameManager.instance.delete(target, target.tag);
                return;
            }

            animator.SetTrigger("move");
            transform.position += (transform.forward * speed * Time.deltaTime);
            Vector3 diff = target.transform.position - transform.position;
            Vector3 chaseVec = new Vector3(diff.x, 0, diff.z);
            Debug.DrawLine(transform.position, transform.position + chaseVec, Color.white);
            transform.rotation = Quaternion.LookRotation(chaseVec, Vector3.up);
            tryBreeding();
        } else if (currentState == FrogState.Escaping) {
            if (predator == null) {
                currentState = FrogState.Wandering;
                speed = wanderSpeed;
                return;
            }

            animator.SetTrigger("move");
            transform.position += (transform.forward * speed * Time.deltaTime);
            transform.rotation = Quaternion.LookRotation(transform.position - predator.transform.position, Vector3.up);
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
