using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Rabbit : Animal {
    public override void Start() {
        base.Start();
    }

    public override void Update() {
        base.Update();
    }

    public void Reset() {
        speed = 3f;
        wanderSpeed = 3f;
        escapeSpeed = 10f;
        animator.speed = wanderSpeed;
        predators = new string[]{ "Tiger", "Eagle", "Iguana" };
        preys = new string[]{ "Grass" };
    }

    public override void CheckHealth() {
        if (health.currentHealth <= 0) {
            currentState = AnimalState.Dead;
            animator.SetBool("dead", true);
            StartCoroutine(Dissolve(1.0f));
        }
    }

    public override void RotateOnTargeting(GameObject obj) {
        Vector3 diff = obj.transform.position - transform.position;
        transform.rotation = Quaternion.LookRotation(new Vector3(diff.x, 0, diff.z), Vector3.up);
    }

    public override void UpdatePosition() {
        animator.SetTrigger("moving");
    }

    public override void UpdateSpeed(float speed) {
        animator.speed = speed;
    }
}
