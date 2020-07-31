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
                // transform.Translate(Vector3.forward * 0.01f);
                animator.SetTrigger("moving");
                wonderTime -= Time.deltaTime;
            } else {
                // animator.SetBool("dead", true);
                wonderTime = Random.Range(1.0f, 2.0f);
                // transform.eulerAngles = new Vector3(0, Random.Range(0, 360), 0);
                transform.Rotate(0, Random.Range(0, 120), 0, Space.World);
            }
        }
    }
}
