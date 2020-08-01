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
        GameManager.instance.delete(target, target.tag);
        currentState = IguanaState.Wandering;
        health.currentHealth += recoverSpeed;
    }

    IEnumerator stopDead(float length) {
        yield return new WaitForSeconds(length);
        GameManager.instance.delete(this.gameObject, this.tag);
    }
}
