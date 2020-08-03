using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class Animal : MonoBehaviour {
    protected Animator animator;
    public CharacterController player;
    protected Health health;
    public float speed;
    public float wanderSpeed;
    public float escapeSpeed;
    public float damageSpeed;
    public float recoverSpeed;
    public enum AnimalState {
        Dead,
        Escaping,
        Targeting,
        Wandering,
        Attacking
    }
    public AnimalState currentState;
    public float sight = 20f;
    protected float wanderTime;
    public GameObject childPrefab;
    public float minAttackDistance;
    public float minBreedDistance;
    public float coolTimeBreeding;
    private float leftTimeForBreeding;
    protected float gravity = 30f;

    public string[] preys;
    public string[] predators;
    protected GameObject target;
    protected GameObject predator;

    public abstract void RotateOnTargeting(GameObject obj);
    public abstract void UpdatePosition();
    public abstract void UpdateSpeed(float speed);

    public virtual void Start() {
        try {
            player = GetComponent<CharacterController>();
        } catch (MissingComponentException e) {}
        try {
            animator = GetComponent<Animator>();
        } catch (MissingComponentException e) {}
        health = GetComponent<Health>();
        wanderTime = Random.Range(1.0f, 2.0f);
        currentState = AnimalState.Wandering;
        leftTimeForBreeding = coolTimeBreeding;
    }

    public virtual void Update() {
        CheckHealth();

        if (currentState == AnimalState.Dead)
            return;

        /* Search wall */
        SearchForWall();

        /* Search prey */
        SearchForPrey();

        /* Search friend */
        SearchForFriend();

        /* Search predator */
        SearchForPredator();
    }

    public virtual void FixedUpdate() {
        if (currentState == AnimalState.Wandering) {
            UpdateOnWandering();
        } else if (currentState == AnimalState.Targeting) {
            UpdateOnTargeting();
        } else if (currentState == AnimalState.Escaping) {
            UpdateOnEscaping();
        }

        if (health != null)
            health.TakeDamage(damageSpeed);
    }

    public virtual void CheckHealth() {
        if (health.currentHealth <= 0) {
            currentState = AnimalState.Dead;
            StartCoroutine(Dissolve(0.5f));
        }
    }

    public virtual void SearchForWall() {
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
    }

    public void SearchForPrey() {
        Collider[] preyColliders = Physics.OverlapSphere(transform.position, sight)
                                           .Where(coll => preys.Contains(coll.tag))
                                           .ToArray();

            if (preyColliders.Length > 0) {
                currentState = AnimalState.Targeting;
                RotateOnTargeting(preyColliders[0].gameObject);
                target = preyColliders[0].gameObject;
            }
    }

    public void SearchForFriend() {
        if (currentState == AnimalState.Wandering && leftTimeForBreeding < 0) {
            Collider[] friendColliders = Physics.OverlapSphere(transform.position, sight)
                .Where(coll => coll.tag == this.tag && coll.gameObject != this.gameObject).ToArray();
            if (friendColliders.Length > 0) {
                currentState = AnimalState.Targeting;
                RotateOnTargeting(friendColliders[0].gameObject);
                target = friendColliders[0].gameObject;
            }
        }
        leftTimeForBreeding -= Time.deltaTime;
    }

    public void SearchForPredator() {
        Collider[] predatorColliders = Physics.OverlapSphere(transform.position, sight).Where(coll => predators.Contains(coll.tag)).ToArray();
        if (predatorColliders.Length > 0) {
            currentState = AnimalState.Escaping;
            UpdateSpeed(escapeSpeed);

            Collider closest = predatorColliders.Aggregate(
                (acc, cur) => (acc.transform.position - transform.position).magnitude < (cur.transform.position - transform.position).magnitude ? acc : cur
            );
            Vector3 diff = transform.position - closest.transform.position;
            transform.rotation = Quaternion.LookRotation(new Vector3(diff.x, 0, diff.z), Vector3.up);
            predator = closest.gameObject;
        } else if (currentState == AnimalState.Escaping) {
            currentState = AnimalState.Wandering;
            UpdateSpeed(wanderSpeed);
        }
    }

    public void UpdateOnWandering() {
        if (wanderTime > 0) {
            wanderTime -= Time.deltaTime;
        } else {
            wanderTime = Random.Range(1.0f, 2.0f);
            transform.Rotate(0, Random.Range(-120, 120), 0, Space.World);
        }

        AdjustHeight();
        UpdatePosition();
    }

    public virtual void AdjustHeight() {}

    public virtual void UpdateOnTargeting() {
        if (target == null) {
            currentState = AnimalState.Wandering;
            UpdateSpeed(wanderSpeed);
            return;
        }

        UpdatePosition();
        RotateOnTargeting();
        TryAttacking();
        TryBreeding();
    }

    public void RotateOnTargeting() {
        RotateOnTargeting(target.gameObject);
    }

    public void UpdateOnEscaping() {
        if (predator == null) {
            currentState = AnimalState.Wandering;
            UpdateSpeed(wanderSpeed);
            return;
        }

        Vector3 diff = transform.position - predator.transform.position;
        transform.rotation = Quaternion.LookRotation(new Vector3(diff.x, 0, diff.z), Vector3.up);
        UpdatePosition();
    }

    protected IEnumerator Dissolve(float time) {
        yield return new WaitForSeconds(time);

        GameManager.instance.delete(this.gameObject, this.tag);
    }
    void OnCollisionEnter(Collision other) {
        if (other.gameObject.tag != "Terrain" && other.gameObject.tag != "Wall") {
            Physics.IgnoreCollision(other.gameObject.GetComponent<Collider>(), GetComponent<Collider>());
        }
    }

    protected void TryAttacking() {
        if (!preys.Contains(target.gameObject.tag))
            return;

        float targetDistance = (target.transform.position - transform.position).magnitude;
        if (targetDistance < minAttackDistance) {
            health.currentHealth += Mathf.Min(recoverSpeed, Health.maxHealth - health.currentHealth);
            GameManager.instance.delete(target.gameObject, target.gameObject.tag);
            target = null;
            UpdateSpeed(wanderSpeed);
        }
    }

    protected void TryBreeding() {
        if (target.tag != this.tag)
            return;
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
