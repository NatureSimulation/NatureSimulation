using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SquirrelScript : MonoBehaviour {
    private Animator animator;
    private Rigidbody rb;
    private Health health;
    private float wanderTime;

    public float speed = 3f;
    public float damageSpeed;
    public float recoverSpeed;
    public float sight = 10f;

    private GameObject target;

    private enum SquirrelState {
        Dead,
        Escaping,
        Targeting,
        Wandering
    }

    private SquirrelState currentState;

    // Start is called before the first frame update
    void Start() {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        health = GetComponent<Health>();
        animator.speed = 3;
        wanderTime = Random.Range(1.0f, 2.0f);
        currentState = SquirrelState.Wandering;
    }

    void OnCollisionEnter(Collision other) {
        if (other.gameObject.tag == "Grass") {
            health.currentHealth += Mathf.Min(recoverSpeed, Health.maxHealth - health.currentHealth);
            GameManager.instance.delete(other.gameObject, other.gameObject.tag);
        }
    }

    // Update is called once per frame
    void Update() {
        if (health.currentHealth <= 0) {
            currentState = SquirrelState.Dead;
            StartCoroutine(Dissolve(1.0f));
        }

        if (currentState == SquirrelState.Dead)
            return;

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
        Collider[] grassColliders = Physics.OverlapSphere(transform.position, sight).Where(coll => coll.tag == "Grass").ToArray();
        if (grassColliders.Length > 0) {
            currentState = SquirrelState.Targeting;
            transform.rotation = Quaternion.LookRotation(grassColliders[0].transform.position - transform.position, Vector3.up);
            target = grassColliders[0].gameObject;
        }
    }

    void FixedUpdate() {
        if (currentState == SquirrelState.Wandering) {
            if (wanderTime > 0) {
                wanderTime -= Time.deltaTime;
            } else {
                wanderTime = Random.Range(1.0f, 2.0f);
                transform.Rotate(0, Random.Range(-120, 120), 0, Space.World);
            }

            animator.SetTrigger("move");
            transform.position += (transform.forward * speed * Time.deltaTime);

        } else if (currentState == SquirrelState.Targeting) {
            if (target == null) {
                currentState = SquirrelState.Wandering;
                return;
            }

            animator.SetTrigger("move");
            transform.position += (transform.forward * speed * Time.deltaTime);
            transform.rotation = Quaternion.LookRotation(target.transform.position - transform.position, Vector3.up);
        }
        if (health != null)
            health.TakeDamage(damageSpeed);
    }

    IEnumerator Dissolve(float time) {
        yield return new WaitForSeconds(time);

        GameManager.instance.delete(this.gameObject, this.tag);
    }
}
