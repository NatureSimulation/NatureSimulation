using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class IguanaScript : MonoBehaviour
{
    public float walkspeed;
    public float wanderSpeed;
    public float escapeSpeed;
    public float sight = 20.0f;
    public float damageSpeed;
    public float recoverSpeed;
    public float minBreedDistance;
    public float coolTimeBreeding;
    private float leftTimeForBreeding;
    public GameObject childPrefab;
    public float minAttackDistance;
    private Animator animator;
    private Health health;
    private float wanderTime;
    private GameObject target;
    public enum IguanaState {
        Dead,
        Escaping,
        Targeting,
        Attacking,
        Wandering
    };
    public IguanaState currentState;
    public static string[] predators = {"Eagle"};
    private GameObject predator;
    void Start()
    {
        health = GetComponent<Health>();
        wanderSpeed = 1f;
        escapeSpeed = 5f;
        walkspeed = wanderSpeed;
        wanderTime = Random.Range(1.0f, 2.0f);
        currentState = IguanaState.Wandering;
        animator = GetComponent<Animator>();
        leftTimeForBreeding = coolTimeBreeding;
    }

    void Update()
    {
        if (health.currentHealth <= 0) {
            currentState = IguanaState.Dead;
            animator.SetTrigger("Death");
            StartCoroutine(stopDead(1));
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
            .Where(coll => coll.tag == "Rabbit" || coll.tag == "Squirrel" || coll.tag == "Frog").ToArray();
        if (colliders.Length > 0) {
            currentState = IguanaState.Targeting;
            transform.rotation = Quaternion.LookRotation(
                colliders[0].transform.position - transform.position);
            target = colliders[0].gameObject;
        }

        /* Search friend */
        if (currentState == IguanaState.Wandering && leftTimeForBreeding < 0) {
            Collider[] friendColliders = Physics.OverlapSphere(transform.position, sight)
                .Where(coll => coll.tag == this.tag && coll.gameObject != this.gameObject).ToArray();
            if (friendColliders.Length > 0) {
                currentState = IguanaState.Targeting;
                transform.rotation = Quaternion.LookRotation(
                    friendColliders[0].transform.position - transform.position);
                target = friendColliders[0].gameObject;
            }
        }
        leftTimeForBreeding -= Time.deltaTime;

        Collider[] colls = Physics.OverlapSphere(transform.position, sight).ToArray();
        Collider[] predatorColliders = Physics.OverlapSphere(transform.position, sight).Where(coll => predators.Contains(coll.tag)).ToArray();

        if (predatorColliders.Length > 0) {
            animator.SetFloat("Forward", escapeSpeed);
            animator.speed = escapeSpeed;
            currentState = IguanaState.Escaping;

            Collider closest = predatorColliders.Aggregate(
                (acc, cur) => (acc.transform.position - transform.position).magnitude < (cur.transform.position - transform.position).magnitude ? acc : cur
            );
            transform.rotation = Quaternion.LookRotation(transform.position - closest.transform.position, Vector3.up);
            predator = closest.gameObject;
        } else if (currentState == IguanaState.Escaping) {
            animator.SetFloat("Forward", wanderSpeed);
            animator.speed = wanderSpeed;
            currentState = IguanaState.Wandering;
        }
    }

    void FixedUpdate() {
        if (currentState == IguanaState.Wandering) {
            if (wanderTime > 0) {
                animator.SetFloat("Forward", wanderSpeed);
                wanderTime -= Time.deltaTime;
            } else {
                animator.speed = wanderSpeed;
                wanderTime = Random.Range(1.0f, 2.0f);
                transform.Rotate(0, Random.Range(-120, 120), 0, Space.World);
            }
        } else if (currentState == IguanaState.Targeting) {
            if (target == null) {
                currentState = IguanaState.Wandering;
                animator.speed = wanderSpeed;
                return;
            }

            Debug.DrawLine(transform.position, target.transform.position, Color.white);
            transform.rotation = Quaternion.LookRotation(target.transform.position - transform.position, Vector3.up);
            animator.SetFloat("Forward", walkspeed * 2);
            if (target.tag != this.tag) {
                tryDamageTarget();
            } else {
                tryBreeding();
            }

            animator.SetFloat("Forward", wanderSpeed * 2);
            animator.speed = wanderSpeed * 2;
            tryDamageTarget();
        } else if (currentState == IguanaState.Escaping) {
            if (predator == null) {
                currentState = IguanaState.Wandering;
                animator.SetFloat("Forward", wanderSpeed);
                animator.speed = wanderSpeed;
                return;
            }

            animator.SetFloat("Forward", escapeSpeed);
            animator.speed = escapeSpeed;
            Debug.DrawLine(transform.position, predator.transform.position, Color.white);
            Vector3 diff = transform.position - predator.transform.position;
            transform.rotation = Quaternion.LookRotation(new Vector3(diff.x, 0, diff.z), Vector3.up);
        }
        if (health != null)
            health.TakeDamage(damageSpeed);
    }

    void tryDamageTarget() {
        float targetDistance = (target.transform.position - transform.position).magnitude;
        if (targetDistance < minAttackDistance) {
            currentState = IguanaState.Attacking;
            animator.SetTrigger("Eat");
            transform.LookAt(target.transform);
            StartCoroutine(stopAttack(1));
        }
    }

    IEnumerator stopAttack(float length) {
        yield return new WaitForSeconds(length);

        currentState = IguanaState.Wandering;
        animator.speed = wanderSpeed;
        if (target != null) {
            GameManager.instance.delete(target, target.tag);
            health.currentHealth += Mathf.Min(recoverSpeed, Health.maxHealth - health.currentHealth);
        }
    }

    IEnumerator stopDead(float length) {
        yield return new WaitForSeconds(length);
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
