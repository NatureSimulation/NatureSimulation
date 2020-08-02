using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DeerScript : MonoBehaviour
{
    private Rigidbody rb;
    public float walkspeed = 5;
    public float damageSpeed;
    public float recoverSpeed;
    public float sight = 10f;
    private Health health;
    private float wanderTime;

    private GameObject target;
    private enum DeerState {
        Dead,
        Escaping,
        Targeting,
        Wandering
    }
    private DeerState currentState;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        health = GetComponent<Health>();
        wanderTime = Random.Range(1.0f, 2.0f);
        currentState = DeerState.Wandering;
    }

    void OnCollisionEnter(Collision other) {
        if (other.gameObject.tag == "Grass") {
            health.currentHealth += Mathf.Min(recoverSpeed, Health.maxHealth - health.currentHealth);
            GameManager.instance.delete(other.gameObject, other.gameObject.tag);
        }
    }

    void Update()
    {
        if (health.currentHealth <= 0) {
            currentState = DeerState.Dead;
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
        Collider[] grassColliders = Physics.OverlapSphere(transform.position, sight)
            .Where(coll => coll.tag == "Grass").ToArray();

        if (grassColliders.Length > 0) {
            currentState = DeerState.Targeting;
            transform.rotation = Quaternion.LookRotation(
                grassColliders[0].transform.position - transform.position, Vector3.up);
            target = grassColliders[0].gameObject;
        }
    }

    void FixedUpdate() {
        if (currentState == DeerState.Wandering) {
            if (wanderTime > 0) {
                transform.Translate(transform.forward * walkspeed * Time.deltaTime, Space.World);
                wanderTime -= Time.deltaTime;
            } else {
                wanderTime = Random.Range(1.0f, 2.0f);
                transform.Rotate(0, Random.Range(-120, 120), 0, Space.World);
            }


        } else if (currentState == DeerState.Targeting) {
            if (target == null) {
                currentState = DeerState.Wandering;
                return;
            }

            Debug.DrawLine(transform.position, target.transform.position, Color.white);
            transform.rotation = Quaternion.LookRotation(target.transform.position - transform.position, Vector3.up);
            transform.Translate(transform.forward * walkspeed * Time.deltaTime);
        }

        if (health != null)
            health.TakeDamage(damageSpeed);
    }
}
