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
    public float minBreedDistance;
    public float coolTimeBreeding;
    private float leftTimeForBreeding;
    public GameObject childPrefab;
    public float sight = 10f;
    float speedOut = 1;
    private float wonderTime;
	private float rotationDegreePerSecond = 1000;
	private bool isAttacking = false;
    private bool dead;

    public GameObject target;
    public float minAttackDistance;
    public EagleState currentState;
    private Health health;

    public enum EagleState {
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
        leftTimeForBreeding = coolTimeBreeding;
    }

    void Update()
    {
        if (health.currentHealth <= 0) {
            currentState = EagleState.Dead;
            animator.SetFloat("Speed", 0);
            animator.SetTrigger("isDead");
            StartCoroutine(stopDead(1));
        }

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
        Collider[] rabbitColliders = Physics.OverlapSphere(transform.position, sight)
            .Where(collider => collider.tag == "Rabbit" || collider.tag == "Frog" || collider.tag == "Iguana").ToArray();
        if (rabbitColliders.Length != 0) {
            currentState = EagleState.Targeting;
            transform.rotation = Quaternion.LookRotation(rabbitColliders[0].transform.position - transform.position, Vector3.up);
            target = rabbitColliders[0].gameObject;
        }

        /* Search friend */
        if (currentState == EagleState.Wandering && leftTimeForBreeding < 0) {
            Collider[] friendColliders = Physics.OverlapSphere(transform.position, sight)
                .Where(coll => coll.tag == this.tag && coll.gameObject != this.gameObject).ToArray();
            if (friendColliders.Length > 0) {
                currentState = EagleState.Targeting;
                transform.rotation = Quaternion.LookRotation(
                    friendColliders[0].transform.position - transform.position);
                target = friendColliders[0].gameObject;
            }
        }
        leftTimeForBreeding -= Time.deltaTime;
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

        } else if (currentState == EagleState.Targeting) {
            if (target == null) {
                currentState = EagleState.Wandering;
                return;
            }
            animator.SetTrigger("Attack");
            // Debug.DrawLine(transform.position, target.transform.position, Color.white);
            transform.rotation = Quaternion.LookRotation(target.transform.position - transform.position, Vector3.up);
            Debug.Log("Eagle: " + transform.position);

            transform.position += (transform.forward * walkspeed * Time.deltaTime);
			animator.SetFloat("Speed", speedOut);


            if (target.tag != this.tag)
                tryDamageTarget();
            else {
                tryBreeding();
            }
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
            health.currentHealth += Mathf.Min(recoverSpeed, Health.maxHealth - health.currentHealth);
            currentState = EagleState.Wandering;
        }
	}

    IEnumerator stopDead(float length) {
        yield return new WaitForSeconds(length);
        GameManager.instance.delete(this.gameObject, this.tag);
    }

    void tryBreeding() {
        float distance = (target.transform.position - transform.position).magnitude;
        if (target.tag == this.tag && distance < minBreedDistance && leftTimeForBreeding < 0) {
            float x = this.transform.position.x + Random.Range(-20, 20);
            float z = this.transform.position.z + Random.Range(-20, 20);
            float y;
            try {
                y = GameManager.instance.getHeight(x, z);
            } catch (System.Exception) {
                return;
            }

            GameObject child = Instantiate(childPrefab, new Vector3(x, y, z), Quaternion.identity);
            GameManager.instance.breed(child.tag);
            leftTimeForBreeding = coolTimeBreeding;
        }
    }
}
