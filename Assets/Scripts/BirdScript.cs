using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BirdScript : MonoBehaviour
{
    public float walkspeed = 5;
    public float sight = 10f;
    private Health health;
    private float wanderTime;
    private GameObject target;
    public float damageSpeed;
    public float recoverSpeed;
    private enum BirdState {
        Dead,
        Targeting,
        Wandering
    };
    private BirdState currentState;

    void Start()
    {
        health = GetComponent<Health>();
        wanderTime = Random.Range(1.0f, 2.0f);
        currentState = BirdState.Wandering;
    }

    void OnCollisionEnter(Collision other) {
        if (other.gameObject.tag == "Squirrel") {
            health.currentHealth += Mathf.Min(recoverSpeed, Health.maxHealth - health.currentHealth);
            GameManager.instance.delete(other.gameObject, other.gameObject.tag);
        }
    }
    void Update()
    {
        if (health.currentHealth <= 0) {
            currentState = BirdState.Dead;
            StartCoroutine(Dissolve(1.0f));
        }

        Collider[] colliders = Physics.OverlapSphere(transform.position, sight)
            .Where(coll => coll.tag == "Squirrel").ToArray();

        if (colliders.Length > 0) {
            currentState = BirdState.Targeting;
            transform.rotation = Quaternion.LookRotation(colliders[0].transform.position - transform.position, Vector3.up);
            target = colliders[0].gameObject;
        }
    }

    void FixedUpdate() {
        if (currentState == BirdState.Wandering) {
            if (wanderTime > 0) {
                wanderTime -= Time.deltaTime;
            } else {
                wanderTime = Random.Range(1.0f, 2.0f);
                transform.Rotate(0, Random.Range(-120, 120), 0, Space.World);
                return;
            }

            /* Forward */
            float planeY;
            try {
                planeY = GameManager.instance.getHeight(this.transform.position.x, this.transform.position.y);
            } catch {
                planeY = 0;
            }

            Quaternion rotation;
            if (planeY + 5 > this.transform.position.y) {
                rotation = Quaternion.Euler(new Vector3 (-5, transform.rotation.y, transform.rotation.z));
            } else {
                rotation = Quaternion.Euler(new Vector3 (5, transform.rotation.y, transform.rotation.z));
            }
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * 2.0f);

            transform.Translate(transform.forward * walkspeed * Time.deltaTime);
        } else if (currentState == BirdState.Targeting) {
            if (target == null) {
                currentState = BirdState.Wandering;
                return;
            }

            transform.rotation = Quaternion.LookRotation(target.transform.position - transform.position, Vector3.up);
            transform.Translate(transform.forward * walkspeed * Time.deltaTime);
        }

        if (health != null)
            health.TakeDamage(damageSpeed);
    }

    IEnumerator Dissolve(float time) {
        yield return new WaitForSeconds(time);
        GameManager.instance.delete(this.gameObject, this.tag);
    }
}
