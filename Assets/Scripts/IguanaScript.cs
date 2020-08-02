using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class IguanaScript : MonoBehaviour
{
    public float walkspeed = 5;
    public float sight = 20.0f;
    public float damageSpeed;
    public float recoverSpeed;
    public float minAttackDistance;
    private Animator animator;
    private Health health;
    private float wanderTime;
    private GameObject target;
    private enum IguanaState {
        Dead,
        Escaping,
        Targeting,
        Attacking,
        Wandering
    };
    private IguanaState currentState;

    void Start()
    {
        health = GetComponent<Health>();
        wanderTime = Random.Range(1.0f, 2.0f);
        currentState = IguanaState.Wandering;
        animator = GetComponent<Animator>();
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
    }

    void FixedUpdate() {
        if (currentState == IguanaState.Wandering) {
            if (wanderTime > 0) {
                animator.SetFloat("Forward", walkspeed);
                wanderTime -= Time.deltaTime;
            } else {
                wanderTime = Random.Range(1.0f, 2.0f);
                transform.Rotate(0, Random.Range(-120, 120), 0, Space.World);
            }
        } else if (currentState == IguanaState.Targeting) {
            if (target == null) {
                currentState = IguanaState.Wandering;
                return;
            }

            Debug.DrawLine(transform.position, target.transform.position, Color.white);
            transform.rotation = Quaternion.LookRotation(target.transform.position - transform.position, Vector3.up);
            animator.SetFloat("Forward", walkspeed * 2);
            tryDamageTarget();
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
        if (target != null) {
            GameManager.instance.delete(target, target.tag);
            health.currentHealth += Mathf.Min(recoverSpeed, Health.maxHealth - health.currentHealth);
        }
    }

    IEnumerator stopDead(float length) {
        yield return new WaitForSeconds(length);
        GameManager.instance.delete(this.gameObject, this.tag);
    }
}
