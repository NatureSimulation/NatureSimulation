using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Eagle : Animal {
    private float speedOut = 1;
    public float minAttackDistance;

    public override void Start() {
        base.Start();
        animator = GetComponentInChildren<Animator>();
    }

    public override void Update() {
        base.Update();
    }

    public void Reset() {
        speed = 5f;
        wanderSpeed = 5f;
        predators = new string[]{};
        preys = new string[]{ "Frog", "Rabbit", "Iguana" };
    }

    public override void CheckHealth() {
        if (health.currentHealth <= 0) {
            currentState = AnimalState.Dead;
            animator.SetFloat("Speed", 0);
            animator.SetTrigger("isDead");
            StartCoroutine(StopDead(0.5f));
        }
    }
    IEnumerator StopDead(float length) {
        yield return new WaitForSeconds(length);
        GameManager.instance.delete(this.gameObject, this.tag);
    }

    public override void UpdateOnTargeting() {
        if (target == null) {
            currentState = AnimalState.Wandering;
            return;
        }
        animator.SetTrigger("Attack");
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
            animator.SetFloat("Speed", 0);
            transform.LookAt(target.transform);
            currentState = AnimalState.Attacking;
            animator.SetTrigger("Attack");
            StartCoroutine(StopAttack(1));
        }
    }

    IEnumerator StopAttack(float length) {
        yield return new WaitForSeconds(length);
        if (target == null) {
            currentState = AnimalState.Wandering;
        } else {
            GameManager.instance.delete(target, target.tag);
            health.currentHealth += Mathf.Min(recoverSpeed, Health.maxHealth - health.currentHealth);
            currentState = AnimalState.Wandering;
        }
    }

    public override void AdjustHeight() {
        float planeY;
        try {
            planeY = GameManager.instance.getHeight(this.transform.position.x, this.transform.position.z);
        } catch {
            planeY = 0;
        }
        Quaternion rotation;
        if (planeY + 10 > this.transform.position.y) {
            rotation = Quaternion.Euler(new Vector3 (-10, transform.rotation.y, transform.rotation.z));
        } else {
            rotation = Quaternion.Euler(new Vector3 (10, transform.rotation.y, transform.rotation.z));
        }
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * 2.0f);
    }

    public override void RotateOnTargeting(GameObject obj) {
        transform.rotation = Quaternion.LookRotation(obj.transform.position - transform.position, Vector3.up);
    }

    public override void UpdatePosition() {
        transform.position += (transform.forward * speed * Time.deltaTime);
        animator.SetFloat("Speed", speedOut);
    }

    public override void UpdateSpeed(float speed) {
        this.speed = speed;
    }
}
