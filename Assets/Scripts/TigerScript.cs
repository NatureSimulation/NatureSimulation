using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TigerScript : MonoBehaviour
{
    public float walkSpeed = 5f;
    public float minAttackDistance;
    public float sight = 20.0f;
    private Animator animator;
    private Health health;
    private float wanderTime;

    private GameObject target;
    private enum TigerState {
        Dead,
        Attacking,
        Targeting,
        Wandering
    };
    private TigerState currentState;

    void Start()
    {
        animator = GetComponent<Animator>();
        health = GetComponent<Health>();
        wanderTime = Random.Range(1.0f, 2.0f);
        currentState = TigerState.Wandering;
    }

    void Update()
    {
        if (health.currentHealth <= 0) {
            currentState = TigerState.Dead;
            GameManager.instance.delete(this.gameObject, this.tag);
        }
        if (health != null)
            health.TakeDamage(0.2f);
        Collider[] colliders = Physics.OverlapSphere(transform.position, sight)
                .Where(coll => coll.tag == "Rabbit" || coll.tag == "Deer").ToArray();

        if (colliders.Length > 0) {
            currentState = TigerState.Targeting;
            transform.rotation = Quaternion.LookRotation(
                colliders[0].transform.position - transform.position);
            target = colliders[0].gameObject;
        }
    }

    void FixedUpdate() {
        if (currentState == TigerState.Wandering) {
            if (wanderTime > 0) {
                transform.Translate(transform.forward * walkSpeed * Time.deltaTime, Space.World);
                animator.SetFloat("moving", walkSpeed);
                wanderTime -= Time.deltaTime;
            } else {
                wanderTime = Random.Range(1.0f, 2.0f);
                transform.Rotate(0, Random.Range(-120, 120), 0, Space.World);
            }

        } else if (currentState == TigerState.Targeting) {
            if (target == null) {
                currentState = TigerState.Wandering;
                return;
            }

            animator.SetFloat("moving", walkSpeed * 2);
            Debug.DrawLine(transform.position, target.transform.position, Color.white);
            transform.rotation = Quaternion.LookRotation(target.transform.position - transform.position);
            transform.Translate(transform.forward * walkSpeed * 2 * Time.deltaTime);

            tryDamageTarget();
        }
    }

    void tryDamageTarget() {
        float targetDistance = (target.transform.position - transform.position).magnitude;
        if (targetDistance < minAttackDistance) {
            // animator.SetFloat("moving", 0);
            transform.LookAt(target.transform);
            currentState = TigerState.Attacking;
            animator.SetTrigger("attack");
            health.currentHealth = Health.maxHealth;
            StartCoroutine(stopAttack(1));
        }
    }

    IEnumerator stopAttack(float length) {
        yield return new WaitForSeconds(length);
        if (target == null) {
            currentState = TigerState.Wandering;
        } else {
            GameManager.instance.delete(target, target.tag);
            health.currentHealth = Health.maxHealth;
            currentState = TigerState.Wandering;
        }
    }
}
