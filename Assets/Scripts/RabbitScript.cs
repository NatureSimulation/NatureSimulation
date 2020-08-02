﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RabbitScript : MonoBehaviour {
    private Animator animator;
    private Rigidbody rb;
    private Health health;
    public float damageSpeed;
    public float recoverSpeed;
    public float sight = 10f;
    private float wanderTime;

    private GameObject target;

    private enum RabbitState {
        Dead,
        Escaping,
        Targeting,
        Wandering
    }

    private RabbitState currentState;

    // Start is called before the first frame update
    void Start() {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        health = GetComponent<Health>();
        animator.speed = 3;
        wanderTime = Random.Range(1.0f, 2.0f);
        currentState = RabbitState.Wandering;
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
        }
        if (health != null)
            health.TakeDamage(damageSpeed);
    }

    IEnumerator Dissolve(float time) {
        yield return new WaitForSeconds(time);

        GameManager.instance.delete(this.gameObject, this.tag);
    }
}
