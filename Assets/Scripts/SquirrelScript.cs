using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SquirrelScript : MonoBehaviour {
    private Animator animator;
    public CharacterController player;
    public float gravity = 30;
    private Health health;
    private float wanderTime;

    public float speed = 3f;
    public float wanderSpeed = 3f;
    public float escapeSpeed = 10f;
    public float damageSpeed;
    public float recoverSpeed;
    public float minBreedDistance;
    public float coolTimeBreeding;
    private float leftTimeForBreeding;
    public GameObject childPrefab;
    public float sight = 10f;

    private GameObject target;

    public enum SquirrelState {
        Dead,
        Escaping,
        Targeting,
        Wandering
    }

    public SquirrelState currentState;
    public static string[] predators = {"Bird", "Iguana"};
    private GameObject predator;
    // Start is called before the first frame update
    void Start() {
        player = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        health = GetComponent<Health>();
        animator.speed = 3f;
        wanderTime = Random.Range(1.0f, 2.0f);
        currentState = SquirrelState.Wandering;
        leftTimeForBreeding = coolTimeBreeding;
    }

    void OnCollisionEnter(Collision other) {
        if (other.gameObject.tag == "Grass" || other.gameObject.tag == "Butterfly") {
            health.currentHealth += Mathf.Min(recoverSpeed, Health.maxHealth - health.currentHealth);
            GameManager.instance.delete(other.gameObject, other.gameObject.tag);
            animator.speed = wanderSpeed;
            speed = wanderSpeed;
        } else if (other.gameObject.tag != "Terrain" && other.gameObject.tag != "Wall") {
            Physics.IgnoreCollision(other.gameObject.GetComponent<Collider>(), GetComponent<Collider>());
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
            .Where(coll => coll.tag == "Wall" || coll.tag == "Butterfly").ToArray();
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
        Collider[] preyColliders = Physics.OverlapSphere(transform.position, sight).Where(coll => coll.tag == "Butterfly" || coll.tag == "Grass").ToArray();
        if (preyColliders.Length > 0) {
            currentState = SquirrelState.Targeting;
            speed += 1f;
            transform.rotation = Quaternion.LookRotation(preyColliders[0].transform.position - transform.position, Vector3.up);
            target = preyColliders[0].gameObject;
        }

        Collider[] predatorColliders = Physics.OverlapSphere(transform.position, sight).Where(coll => predators.Contains(coll.tag)).ToArray();
        if (predatorColliders.Length > 0) {
            speed = escapeSpeed;
            animator.speed = escapeSpeed;
            currentState = SquirrelState.Escaping;

            Collider closest = predatorColliders.Aggregate(
                (acc, cur) => (acc.transform.position - transform.position).magnitude < (cur.transform.position - transform.position).magnitude ? acc : cur
            );
            Vector3 diff = transform.position - closest.transform.position;
            transform.rotation = Quaternion.LookRotation(new Vector3(diff.x, 0, diff.z), Vector3.up);
            predator = closest.gameObject;
        } else if (currentState == SquirrelState.Escaping) {
            currentState = SquirrelState.Wandering;
            speed = wanderSpeed;
            animator.speed = wanderSpeed;
        }

        /* Search friend */
        if (currentState == SquirrelState.Wandering && leftTimeForBreeding < 0) {
            Collider[] friendColliders = Physics.OverlapSphere(transform.position, sight)
                .Where(coll => coll.tag == this.tag && coll.gameObject != this.gameObject).ToArray();
            if (friendColliders.Length > 0) {
                currentState = SquirrelState.Targeting;
                transform.rotation = Quaternion.LookRotation(
                    friendColliders[0].transform.position - transform.position);
                target = friendColliders[0].gameObject;
            }
        }
        leftTimeForBreeding -= Time.deltaTime;
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

        } else if (currentState == SquirrelState.Targeting) {
            if (target == null) {
                currentState = SquirrelState.Wandering;
                return;
            }

            animator.SetTrigger("move");
            transform.rotation = Quaternion.LookRotation(target.transform.position - transform.position, Vector3.up);
            tryBreeding();
        } else if (currentState == SquirrelState.Escaping) {
            if (predator == null) {
                currentState = SquirrelState.Wandering;
                speed = wanderSpeed;
                animator.speed = wanderSpeed;
                return;
            }

            animator.SetTrigger("move");
            Vector3 diff = transform.position - predator.transform.position;
            transform.rotation = Quaternion.LookRotation(new Vector3(diff.x, 0, diff.z), Vector3.up);
        }

        /* Forward */
        if (!player.isGrounded) {
            Vector3 moveDirection = transform.forward;
            moveDirection.y -= gravity * Time.deltaTime;
            player.Move(moveDirection * speed * Time.deltaTime);
        } else {
            player.Move(transform.forward * speed * Time.deltaTime);
        }

        if (health != null)
            health.TakeDamage(damageSpeed);
    }

    IEnumerator Dissolve(float time) {
        yield return new WaitForSeconds(time);

        GameManager.instance.delete(this.gameObject, this.tag);
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
}
