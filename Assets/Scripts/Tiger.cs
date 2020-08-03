using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Tiger : Animal {
    public float minAttackDistance;
    public override void Start() {
        base.Start();
    }

    public override void Update() {
        base.Update();
    }

    public void Reset() {
        speed = 5f;
        wanderSpeed = 5f;
        predators = new string[]{};
        preys = new string[]{ "Rabbit", "Deer" };
    }

    public override void UpdateOnTargeting() {
        if (target == null) {
            currentState = AnimalState.Wandering;
            UpdateSpeed(wanderSpeed);
            return;
        }

        UpdateSpeed(wanderSpeed * 2);
        RotateOnTargeting(target);
        UpdatePosition();

        if (target.tag != this.tag)
            TryDamageTarget();
        else {
            TryBreeding();
        }
    }

    void TryDamageTarget() {
        float targetDistance = (target.transform.position - transform.position).magnitude;
        if (targetDistance < minAttackDistance) {
            // animator.SetFloat("moving", 0);
            transform.LookAt(target.transform);
            currentState = AnimalState.Attacking;
            animator.SetTrigger("attack");
            StartCoroutine(StopAttack(0.5f, target));
        }
    }

    IEnumerator StopAttack(float length, GameObject target) {
        yield return new WaitForSeconds(length);
        if (target != null) {
            GameManager.instance.delete(target, target.tag);
            health.currentHealth += Mathf.Min(recoverSpeed, Health.maxHealth - health.currentHealth);
            currentState = AnimalState.Wandering;
        }
    }

    public override void RotateOnTargeting(GameObject obj) {
        Vector3 diff = obj.transform.position - transform.position;
        transform.rotation = Quaternion.LookRotation(new Vector3(diff.x, 0, diff.z), Vector3.up);
    }

    public override void UpdatePosition() {
        animator.SetFloat("moving", speed);
        transform.position += (transform.forward * speed * Time.deltaTime);
    }

    public override void UpdateSpeed(float speed) {
        animator.SetFloat("moving", speed);
        this.speed = speed;
    }
}
