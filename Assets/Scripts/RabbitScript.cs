using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RabbitScript : MonoBehaviour {
    private Animator animator;
    private Rigidbody rb;
    private float wonderTime;

    // Start is called before the first frame update
    void Start() {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        animator.speed = 3;
        wonderTime = Random.Range(1.0f, 2.0f);
    }

    // Update is called once per frame
    void Update() {
        if (!animator.GetBool("dead")) {
            if (wonderTime > 0) {
                animator.SetTrigger("moving");
                wonderTime -= Time.deltaTime;
            } else {
                wonderTime = Random.Range(1.0f, 2.0f);
                transform.Rotate(0, Random.Range(-120, 120), 0, Space.World);
            }

            Collider[] colliders = Physics.OverlapSphere(transform.position, 10.0f);
            foreach (var collider in colliders) {
                Debug.Log("Collision: " + collider.tag);
            }
        }
    }
}