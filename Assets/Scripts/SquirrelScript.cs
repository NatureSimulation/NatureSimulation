using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SquirrelScript : MonoBehaviour {
    private Animator animator;
    private Rigidbody rb;
    private Health health;
    private float wanderTime;

    public float speed = 3f;

    private GameObject target;

    private enum SquirrelState {
        Dead,
        Escaping,
        Targeting,
        Wandering
    }

    private SquirrelState currentState;

    // Start is called before the first frame update
    void Start() {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        health = GetComponent<Health>();
        animator.speed = 3;
        wanderTime = Random.Range(1.0f, 2.0f);
        currentState = SquirrelState.Wandering;
    }

    void OnCollisionEnter(Collision other) {
        if (other.gameObject.tag == "Grass") {
            health.currentHealth = 1000.0f;
            GameManager.instance.delete(other.gameObject, other.gameObject.tag);
        }
    }

    // Update is called once per frame
    void Update() {
        if (health.currentHealth <= 0) {
            currentState = SquirrelState.Dead;
            StartCoroutine(Dissolve(1.0f));
        }

        if (currentState == SquirrelState.Dead)
            return;

        if (health != null)
            health.TakeDamage(1f);
    }

    void FixedUpdate() {
        if (currentState == SquirrelState.Wandering) {
            if (wanderTime > 0) {
                wanderTime -= Time.deltaTime;
            } else {
                wanderTime = Random.Range(1.0f, 2.0f);
                transform.Rotate(0, Random.Range(-120, 120), 0, Space.World);
            }

            animator.SetTrigger("move");
            transform.position += (transform.forward * speed * Time.deltaTime);

            Collider[] grassColliders = Physics.OverlapSphere(transform.position, 10.0f).Where(coll => coll.tag == "Grass").ToArray();

            if (grassColliders.Length > 0) {
                currentState = SquirrelState.Targeting;
                transform.rotation = Quaternion.LookRotation(grassColliders[0].transform.position - transform.position, Vector3.up);
                target = grassColliders[0].gameObject;
                Debug.Log("Targeting");
            }
        } else if (currentState == SquirrelState.Targeting) {
            if (target == null) {
                currentState = SquirrelState.Wandering;
                return;
            }

            animator.SetTrigger("move");
            transform.position += (transform.forward * speed * Time.deltaTime);
            transform.rotation = Quaternion.LookRotation(target.transform.position - transform.position, Vector3.up);
        }
    }

    IEnumerator Dissolve(float time) {
        yield return new WaitForSeconds(time);

        GameManager.instance.delete(this.gameObject, this.tag);
    }
}
