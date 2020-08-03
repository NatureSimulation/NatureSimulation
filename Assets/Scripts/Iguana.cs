using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Iguana : Animal {
    public override void Start() {
        base.Start();
    }

    public override void Update() {
        base.Update();
    }

    public void Reset() {
        speed = 1f;
        wanderSpeed = 1f;
        escapeSpeed = 5f;
        predators = new string[]{ "Eagle" };
        preys = new string[]{ "Rabbit", "Squirrel", "Frog" };
    }

    public override void UpdateOnTargeting() {
        if (target == null) {
            currentState = AnimalState.Wandering;
            UpdateSpeed(wanderSpeed);
            return;
        }

        UpdateSpeed(wanderSpeed * 2);
        RotateOnTargeting(target);

        if (target.tag != this.tag)
            TryDamageTarget();
        else {
            TryBreeding();
        }
    }

    void TryDamageTarget() {
        float targetDistance = (target.transform.position - transform.position).magnitude;
        if (targetDistance < minAttackDistance) {
            currentState = AnimalState.Attacking;
            animator.SetTrigger("Eat");
            transform.LookAt(target.transform);
            StartCoroutine(StopAttack(1));
        }
    }

    IEnumerator StopAttack(float length) {
        yield return new WaitForSeconds(length);

        currentState = AnimalState.Wandering;
        UpdateSpeed(wanderSpeed);
        if (target != null) {
            GameManager.instance.delete(target, target.tag);
            health.currentHealth += Mathf.Min(recoverSpeed, Health.maxHealth - health.currentHealth);
        }
    }

    IEnumerator StopDead(float length) {
        yield return new WaitForSeconds(length);
        GameManager.instance.delete(this.gameObject, this.tag);
    }

    public override void CheckHealth() {
        if (health.currentHealth <= 0) {
            currentState = AnimalState.Dead;
            animator.SetTrigger("Death");
            StartCoroutine(StopDead(0.5f));
        }
    }

    public override void RotateOnTargeting(GameObject obj) {
        Vector3 diff = obj.transform.position - transform.position;
        transform.rotation = Quaternion.LookRotation(new Vector3(diff.x, 0, diff.z), Vector3.up);
    }

    public override void UpdatePosition() {}

    public override void UpdateSpeed(float speed) {
        animator.SetFloat("Forward", speed);
        animator.speed = speed;
    }
}
