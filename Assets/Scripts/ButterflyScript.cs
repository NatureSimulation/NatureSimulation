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
    public float minBreedDistance;
    public float coolTimeBreeding;
    private float leftTimeForBreeding;
    public GameObject childPrefab;
    public float sight = 10f;
    private float wanderTime;

    public float speed = 3f;
    public float wanderSpeed = 3;
    public float escapeSpeed = 10;
    public float damageSpeed;

    private GameObject target;

    public enum ButterflyState {
        Dead,
        Escaping,
        Targeting,
        Wandering
    }

    public ButterflyState currentState;
    public string[] predators = {"Frog"};
    private GameObject predator;

    // Start is called before the first frame update
    void Start() {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        health = GetComponent<Health>();
        animator.speed = 3f;
        wanderTime = Random.Range(1.0f, 2.0f);
        currentState = ButterflyState.Wandering;
        leftTimeForBreeding = coolTimeBreeding;
    }

    void OnCollisionEnter(Collision other) {
        if (other.gameObject.tag == "Grass") {
            health.currentHealth += Mathf.Min(recoverSpeed, Health.maxHealth - health.currentHealth);
            GameManager.instance.delete(other.gameObject, other.gameObject.tag);
        } else if (other.gameObject.tag != "Terrain" && other.gameObject.tag != "Wall") {
            Physics.IgnoreCollision(other.gameObject.GetComponent<Collider>(), GetComponent<Collider>());
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
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime);
        }

        /* Search prey */
        Collider[] grassColliders = Physics.OverlapSphere(transform.position, sight).Where(coll => coll.tag == "Grass").ToArray();
            if (grassColliders.Length > 0) {
                currentState = ButterflyState.Targeting;
                transform.rotation = Quaternion.LookRotation(grassColliders[0].transform.position - transform.position, Vector3.up);
                target = grassColliders[0].gameObject;
            }

        /* Search friend */
        if (currentState == ButterflyState.Wandering && leftTimeForBreeding < 0) {
            Collider[] friendColliders = Physics.OverlapSphere(transform.position, sight)
                .Where(coll => coll.tag == this.tag && coll.gameObject != this.gameObject).ToArray();
            if (friendColliders.Length > 0) {
                currentState = ButterflyState.Targeting;
                transform.rotation = Quaternion.LookRotation(
                    friendColliders[0].transform.position - transform.position);
                target = friendColliders[0].gameObject;
            }
        }
        leftTimeForBreeding -= Time.deltaTime;

        Collider[] predatorColliders = Physics.OverlapSphere(transform.position, sight).Where(coll => predators.Contains(coll.tag)).ToArray();
        if (predatorColliders.Length > 0) {
            speed = escapeSpeed;
            currentState = ButterflyState.Escaping;

            Collider closest = predatorColliders.Aggregate(
                (acc, cur) => (acc.transform.position - transform.position).magnitude < (cur.transform.position - transform.position).magnitude ? acc : cur
            );
            Vector3 diff = transform.position - closest.transform.position;
            transform.rotation = Quaternion.LookRotation(new Vector3(diff.x, 0, diff.z), Vector3.up);
            predator = closest.gameObject;
        } else if (currentState == ButterflyState.Escaping) {
            currentState = ButterflyState.Wandering;
            speed = wanderSpeed;
        }
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

        } else if (currentState == ButterflyState.Targeting) {
            if (target == null) {
                currentState = ButterflyState.Wandering;
                return;
            }

            transform.position += (transform.forward * speed * Time.deltaTime);
            transform.rotation = Quaternion.LookRotation(target.transform.position - transform.position, Vector3.up);
            if (target.tag == this.tag)
                tryBreeding();
        } else if (currentState == ButterflyState.Escaping) {
            if (predator == null) {
                currentState = ButterflyState.Wandering;
                speed = wanderSpeed;
                return;
            }

            Vector3 diff = transform.position - predator.transform.position;
            transform.rotation = Quaternion.LookRotation(new Vector3(diff.x, 0, diff.z), Vector3.up);
            transform.position += (transform.forward * speed * Time.deltaTime);
        }

        if (health != null)
            health.TakeDamage(damageSpeed);
    }

    IEnumerator Dissolve(float time) {
        yield return new WaitForSeconds(time);

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
