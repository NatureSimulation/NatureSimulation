using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EagelScripts : MonoBehaviour
{
    private Animator animator;
    public float walkspeed = 5;
    public float damageSpeed;
    public float recoverSpeed;
    float speedOut = 1;
    private float wonderTime;
	private float rotationDegreePerSecond = 1000;
	private bool isAttacking = false;
    private bool dead;

    public GameObject target;
    public float minAttackDistance;
    private EagleState currentState;
    private Health health;

    enum EagleState {
        Wandering,
        Targeting,
        Dead,
        Attacking
    };

    void Start()
    {
        animator = GetComponentInChildren<Animator>();
        wonderTime = Random.Range(1.0f, 2.0f);
        currentState = EagleState.Wandering;
        health = GetComponent<Health>();
    }

    void Update()
    {
        if (health.currentHealth <= 0) {
            currentState = EagleState.Dead;
            animator.SetFloat("Speed", 0);
            animator.SetTrigger("isDead");
            StartCoroutine(stopDead(1));
        }
    }

    void FixedUpdate()
	{
		if (currentState == EagleState.Wandering)
		{
            /* Rotate */
            if (wonderTime > 0) {
                wonderTime -= Time.deltaTime;
            } else {
                wonderTime = Random.Range(1.0f, 2.0f);
                transform.Rotate(0, Random.Range(-120, 120), 0, Space.World);
            }

            /* Forward */
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

			transform.position += (transform.forward * walkspeed * Time.deltaTime);
			animator.SetFloat("Speed", speedOut);

            /* Look near */
            Collider[] rabbitColliders = Physics.OverlapSphere(transform.position, 20.0f)
                .Where(collider => collider.tag == "Rabbit").ToArray();
            if (rabbitColliders.Length != 0) {
                currentState = EagleState.Targeting;
                transform.rotation = Quaternion.LookRotation(rabbitColliders[0].transform.position - transform.position, Vector3.up);
                target = rabbitColliders[0].gameObject;
            }

        } else if (currentState == EagleState.Targeting ) {
            if (target == null) {
                currentState = EagleState.Wandering;
                return;
            }
            animator.SetTrigger("Attack");
            // Debug.DrawLine(transform.position, target.transform.position, Color.white);
            transform.rotation = Quaternion.LookRotation(target.transform.position - transform.position, Vector3.up);

            transform.position += (transform.forward * walkspeed * Time.deltaTime);
			animator.SetFloat("Speed", speedOut);

            tryDamageTarget();
		}
        if (health != null)
            health.TakeDamage(damageSpeed);
	}

    void tryDamageTarget() {
        float targetDistance = (target.transform.position - transform.position).magnitude;
        if (targetDistance < minAttackDistance) {
            animator.SetFloat("Speed", 0);
            transform.LookAt(target.transform);
            currentState = EagleState.Attacking;
            animator.SetTrigger("Attack");
            StartCoroutine(stopAttack(1));
        }
    }

    IEnumerator stopAttack(float length)
	{
		yield return new WaitForSeconds(length);
        if (target == null) {
            currentState = EagleState.Wandering;
        } else {
            GameManager.instance.delete(target, target.tag);
            health.currentHealth += recoverSpeed;
            currentState = EagleState.Wandering;
        }
	}

    IEnumerator stopDead(float length) {
        yield return new WaitForSeconds(length);
        GameManager.instance.delete(this.gameObject, this.tag);
    }
}
