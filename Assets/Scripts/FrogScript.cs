using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FrogScript : MonoBehaviour {
    private Animator animator;
    private Rigidbody rb;
    private Health health;
    private float wanderTime;

    public float speed = 10f;

    private GameObject target;

    private enum FrogState {
        Dead,
        Escaping,
        Targeting,
        Wandering
    }

    private FrogState currentState;

    // Start is called before the first frame update
    void Start() {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        health = GetComponent<Health>();
        animator.speed = 3;
        wanderTime = UnityEngine.Random.Range(1.0f, 2.0f);
        currentState = FrogState.Wandering;
    }

    void OnCollisionEnter(Collision other) {
        if (other.gameObject.tag == "Butterfly") {
            health.currentHealth = 1000.0f;
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

        if (health != null)
            health.TakeDamage(1f);
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

            Collider[] preyColliders = Physics.OverlapSphere(transform.position, 10.0f).Where(coll => coll.tag == "Butterfly").ToArray();

            if (preyColliders.Length > 0) {
                currentState = FrogState.Targeting;
                transform.rotation = Quaternion.LookRotation(preyColliders[0].transform.position - transform.position, Vector3.up);
                target = preyColliders[0].gameObject;
                Debug.Log("Targeting");
            }
        } else if (currentState == FrogState.Targeting) {
            if (target == null) {
                currentState = FrogState.Wandering;
                return;
            }

            if ((target.transform.position - transform.position).magnitude < 8) {
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
        }
    }

    IEnumerator Dissolve(float time) {
        yield return new WaitForSeconds(time);

        GameManager.instance.delete(this.gameObject, this.tag);
    }
}
