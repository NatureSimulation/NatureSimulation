using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RabbitScript : MonoBehaviour {
    private Animator animator;
    private Rigidbody rb;
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
        animator.speed = 3;
        wanderTime = Random.Range(1.0f, 2.0f);
        currentState = RabbitState.Wandering;
    }

    void OnCollisionEnter(Collision other) {
        if (other.gameObject.tag == "Grass") {
            Destroy(other.gameObject);
        }
    }

    // Update is called once per frame
    void Update() {
        if (currentState == RabbitState.Wandering) {
            if (wanderTime > 0) {
                animator.SetTrigger("moving");
                wanderTime -= Time.deltaTime;
            } else {
                wanderTime = Random.Range(1.0f, 2.0f);
                transform.Rotate(0, Random.Range(-120, 120), 0, Space.World);
            }

            Collider[] grassColliders = Physics.OverlapSphere(transform.position, 10.0f).Where(coll => coll.tag == "Grass").ToArray();

            if (grassColliders.Length > 0) {
                currentState = RabbitState.Targeting;
                transform.rotation = Quaternion.LookRotation(grassColliders[0].transform.position - transform.position, Vector3.up);
                target = grassColliders[0].gameObject;
                Debug.Log("Targeting");
            }
        } else if (currentState == RabbitState.Targeting) {
            if (target == null) {
                currentState = RabbitState.Wandering;
                return;
            }

            Debug.DrawLine(transform.position, target.transform.position, Color.white);
            Debug.DrawLine(transform.position, transform.position + transform.forward, Color.white);
            transform.rotation = Quaternion.LookRotation(target.transform.position - transform.position, Vector3.up);
            animator.SetTrigger("moving");
        }
    }
}