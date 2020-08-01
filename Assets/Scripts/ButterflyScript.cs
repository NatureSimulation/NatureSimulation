using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ButterflyScript : MonoBehaviour
{
    private Animator animator;
    private Rigidbody rb;
    private Health health;
    public float recoverSpeed;
    public float sight = 10f;
    private float wanderTime;

    public float speed = 3f;
    public float damageSpeed;

    private GameObject target;

    private enum ButterflyState {
        Dead,
        Escaping,
        Targeting,
        Wandering
    }

    private ButterflyState currentState;

    // Start is called before the first frame update
    void Start() {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        health = GetComponent<Health>();
        animator.speed = 3;
        wanderTime = Random.Range(1.0f, 2.0f);
        currentState = ButterflyState.Wandering;
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
            currentState = ButterflyState.Dead;
            StartCoroutine(Dissolve(1.0f));
        }

        if (currentState == ButterflyState.Dead)
            return;

    }

    void FixedUpdate() {
        if (currentState == ButterflyState.Wandering) {
            if (wanderTime > 0) {
                wanderTime -= Time.deltaTime;
            } else {
                wanderTime = Random.Range(1.0f, 2.0f);
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
            if (planeY + 1 > this.transform.position.y) {
                rotation = Quaternion.Euler(new Vector3 (-1, transform.rotation.y, transform.rotation.z));
            } else {
                rotation = Quaternion.Euler(new Vector3 (1, transform.rotation.y, transform.rotation.z));
            }
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * 2.0f);

            transform.position += (transform.forward * speed * Time.deltaTime);

            Collider[] grassColliders = Physics.OverlapSphere(transform.position, sight).Where(coll => coll.tag == "Grass").ToArray();

            if (grassColliders.Length > 0) {
                currentState = ButterflyState.Targeting;
                transform.rotation = Quaternion.LookRotation(grassColliders[0].transform.position - transform.position, Vector3.up);
                target = grassColliders[0].gameObject;
            }
        } else if (currentState == ButterflyState.Targeting) {
            if (target == null) {
                currentState = ButterflyState.Wandering;
                return;
            }

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
