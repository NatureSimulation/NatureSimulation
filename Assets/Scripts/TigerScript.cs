using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TigerScript : MonoBehaviour
{
    public CharacterController player;
    private float gravity = 30;
    public float walkspeed = 5f;
    public float minAttackDistance;
    public float sight = 20.0f;
    public float damageSpeed;
    public float recoverSpeed;
    public float minBreedDistance;
    public float coolTimeBreeding;
    private float leftTimeForBreeding;
    public GameObject childPrefab;
    private Animator animator;
    private Health health;
    private float wanderTime;

    private GameObject target;
    public enum TigerState {
        Dead,
        Attacking,
        Targeting,
        Wandering
    };
    public TigerState currentState;

    void Start()
    {
        player = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        health = GetComponent<Health>();
        wanderTime = Random.Range(1.0f, 2.0f);
        currentState = TigerState.Wandering;
        leftTimeForBreeding = coolTimeBreeding;
    }

    void Update()
    {
        if (health.currentHealth <= 0) {
            currentState = TigerState.Dead;
            GameManager.instance.delete(this.gameObject, this.tag);
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
        Collider[] colliders = Physics.OverlapSphere(transform.position, sight)
            .Where(coll => coll.tag == "Rabbit" || coll.tag == "Deer").ToArray();
        if (colliders.Length > 0) {
            currentState = TigerState.Targeting;
            transform.rotation = Quaternion.LookRotation(
                colliders[0].transform.position - transform.position);
            target = colliders[0].gameObject;
        }

        /* Search friend */
        if (currentState == TigerState.Wandering && leftTimeForBreeding < 0) {
            Collider[] friendColliders = Physics.OverlapSphere(transform.position, sight)
                .Where(coll => coll.tag == this.tag && coll.gameObject != this.gameObject).ToArray();
            if (friendColliders.Length > 0) {
                currentState = TigerState.Targeting;
                transform.rotation = Quaternion.LookRotation(
                    friendColliders[0].transform.position - transform.position);
                target = friendColliders[0].gameObject;
            }
        }
        leftTimeForBreeding -= Time.deltaTime;
    }

    void FixedUpdate() {
        if (currentState == TigerState.Wandering) {
            if (wanderTime > 0) {
                animator.SetFloat("moving", walkspeed);
                wanderTime -= Time.deltaTime;
            } else {
                wanderTime = Random.Range(1.0f, 2.0f);
                transform.Rotate(0, Random.Range(-120, 120), 0, Space.World);
            }

            Debug.DrawLine(transform.position, transform.position + transform.forward, Color.white);
        } else if (currentState == TigerState.Targeting) {
            if (target == null) {
                currentState = TigerState.Wandering;
                return;
            }

            animator.SetFloat("moving", walkspeed * 2);
            Debug.DrawLine(transform.position, target.transform.position, Color.white);
            Vector3 diff = target.transform.position - transform.position;
            transform.rotation = Quaternion.LookRotation(new Vector3(diff.x, 0, diff.z), Vector3.up);
            if (target.tag != this.tag) {
                tryDamageTarget();
            } else {
                tryBreeding();
            }
        }

        /* Forward */
        if (!player.isGrounded) {
            Vector3 moveDirection = transform.forward;
            moveDirection.y -= gravity * Time.deltaTime;
            player.Move(moveDirection * walkspeed * Time.deltaTime);
        } else {
            player.Move(transform.forward * walkspeed * Time.deltaTime);
        }

        if (health != null)
            health.TakeDamage(damageSpeed);
    }

    void tryDamageTarget() {
        float targetDistance = (target.transform.position - transform.position).magnitude;
        if (targetDistance < minAttackDistance) {
            // animator.SetFloat("moving", 0);
            transform.LookAt(target.transform);
            currentState = TigerState.Attacking;
            animator.SetTrigger("attack");
            StartCoroutine(stopAttack(0.5f, target));
        }
    }

    IEnumerator stopAttack(float length, GameObject target) {
        yield return new WaitForSeconds(length);
        if (target != null) {
            GameManager.instance.delete(target, target.tag);
            health.currentHealth += Mathf.Min(recoverSpeed, Health.maxHealth - health.currentHealth);
            currentState = TigerState.Wandering;
        }
    }
    void tryBreeding() {
        if (target.tag != this.tag)
            return;
        float distance = (target.transform.position - transform.position).magnitude;
        if (distance < minBreedDistance && leftTimeForBreeding < 0) {
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

    void OnCollisionEnter(Collision other) {
        if (other.gameObject.tag != "Terrain" && other.gameObject.tag != "Wall") {
            Physics.IgnoreCollision(other.gameObject.GetComponent<Collider>(), GetComponent<Collider>());
        }
    }
}
