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
    public float preyThreshold = 100f;
    public float breedThreshold = 300f;
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
    public float coolTimeEscaping = 1f;
    private float leftTimeForBreeding;
    private float leftTimeForEscaping;
    protected float gravity = 30f;

    public string[] preys;
    public string[] predators;
    protected GameObject target;
    protected GameObject predator;
    protected bool isInfection = false;

    protected float lightningDeathProbability = 0.5f;
    private bool attemptSuicide = false;

    public abstract void RotateOnTargeting(GameObject obj);
    public abstract void UpdatePosition();
    public abstract void UpdateSpeed(float speed);

    public virtual void Start() {
        try {
            player = GetComponent<CharacterController>();
            player.enabled = false;
            transform.position = transform.position;
            transform.rotation = Quaternion.identity;
            player.enabled = true;
        } catch (MissingComponentException e) {}
        try {
            animator = GetComponent<Animator>();
        } catch (MissingComponentException e) {}
        health = GetComponent<Health>();
        wanderTime = Random.Range(1.0f, 2.0f);
        currentState = AnimalState.Wandering;
        leftTimeForBreeding = coolTimeBreeding;
        leftTimeForEscaping = coolTimeEscaping;
        UpdateInfection(isInfection);
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

        /* ClickListener */
        CheckClick();

        /*  high level debugging ㅎㅎ */
        UpdateInfection(isInfection);
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
            if (isInfection)
                health.TakeDamage(damageSpeed * 2);
            else {
                health.TakeDamage(damageSpeed);
            }
    }

    public virtual void CheckHealth() {
        if (health.currentHealth <= 0) {
            currentState = AnimalState.Dead;
            StartCoroutine(Dissolve(0.5f));
        }
        LightningTrial();
    }

    public virtual void LightningTrial() {
        if (!attemptSuicide && GameManager.instance.lightningOn) {
            if (Random.Range(0.0f, 1.0f) < lightningDeathProbability) {
                GameManager.instance.delete(this.gameObject, this.tag);
            }
            attemptSuicide = true;
        } else if (attemptSuicide && !GameManager.instance.lightningOn) {
            attemptSuicide = false;
        }
    }

    public virtual void SearchForWall() {
        // For optimization
        if (transform.position.magnitude < 80)
            return;
        IEnumerable wallColliders = Physics.OverlapSphere(transform.position, sight)
            .Where(coll => coll.tag == "Wall");
        IEnumerator it = wallColliders.GetEnumerator();

        if (it.MoveNext()) {
            Quaternion rotation;
            Collider wall = (Collider)it.Current;
            if (wall.name == "NorthWall") {
                rotation = Quaternion.Euler(new Vector3(transform.rotation.x, 180, transform.rotation.z));
            } else if (wall.name == "SouthWall") {
                rotation = Quaternion.Euler(new Vector3(transform.rotation.x, 0, transform.rotation.z));
            } else if (wall.name == "EastWall") {
                rotation = Quaternion.Euler(new Vector3(transform.rotation.x, -90, transform.rotation.z));
            } else {
                rotation = Quaternion.Euler(new Vector3(transform.rotation.x, 90, transform.rotation.z));
            }
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime);
        }
    }

    public void SearchForPrey() {
        if(health.currentHealth > preyThreshold)
            return;
        Collider[] preyColliders = Physics.OverlapSphere(transform.position, sight)
                                           .Where(coll => preys.Contains(coll.tag))
                                           .ToArray();

            if (preyColliders.Length > 0) {
                currentState = AnimalState.Targeting;
                Collider closest = preyColliders.Aggregate(
                    (acc, cur) => (acc.transform.position - transform.position).magnitude < (cur.transform.position - transform.position).magnitude ? acc : cur
                    );
                RotateOnTargeting(closest.gameObject);
                target = closest.gameObject;
            }
    }

    public void SearchForFriend() {
        if (currentState == AnimalState.Wandering && leftTimeForBreeding < 0
            && health.currentHealth > breedThreshold) {
            Collider[] friendColliders = Physics.OverlapSphere(transform.position, sight)
                .Where(coll => coll.tag == this.tag && coll.gameObject != this.gameObject).ToArray();
            if (friendColliders.Length > 0) {
                currentState = AnimalState.Targeting;
                Collider closest = friendColliders.Aggregate(
                    (acc, cur) => (acc.transform.position - transform.position).magnitude < (cur.transform.position - transform.position).magnitude ? acc : cur
                    );
                RotateOnTargeting(closest.gameObject);
                target = closest.gameObject;
            }
        }
        leftTimeForBreeding -= Time.deltaTime;
    }

    public void SearchForPredator() {
        leftTimeForEscaping -= Time.deltaTime;
        /* Check existing escaping */
        if (currentState == AnimalState.Escaping && predator != null) {
            float distance = (transform.position - predator.transform.position).magnitude;
            if (distance > sight * 2) {
                currentState = AnimalState.Wandering;
                UpdateSpeed(wanderSpeed);
            }
            return;
        }
        if (currentState != AnimalState.Wandering || leftTimeForEscaping > 0)
            return;

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
            leftTimeForEscaping = coolTimeEscaping;
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
        if (other.collider.tag == "Bullet") {
            GameManager.instance.delete(transform.gameObject, transform.tag);
            GameManager.instance.HuntCount += 1;
            GameManager.instance.HuntCountText.text = "Kills:\t" + GameManager.instance.HuntCount;
            return;
        }
        if (other.gameObject.tag != "Terrain" && other.gameObject.tag != "Wall") {
            Physics.IgnoreCollision(other.gameObject.GetComponent<Collider>(), GetComponent<Collider>());
        }
    }

    protected void TryAttacking() {
        if (target == null)
            return;
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
        if (target == null)
            return;
        if (target.tag != this.tag)
            return;
        if (GameManager.instance.isHuntMode) {
            if (Random.Range(0f, 1f) < 0.9f)
                return;
        }
        float distance = (target.transform.position - transform.position).magnitude;
        if (target.tag == this.tag && distance < minBreedDistance && leftTimeForBreeding < 0) {
            /* Transmit infection to partner */
            if (isInfection)
                transmitInfection(target);

            float x = this.transform.position.x + Random.Range(-20, 20);
            float z = this.transform.position.z + Random.Range(-20, 20);
            float y;
            try {
                y = GameManager.instance.getHeight(x, z);
            } catch (System.Exception) {
                return;
            }

            leftTimeForBreeding = coolTimeBreeding;
            currentState = AnimalState.Wandering;
            GameObject child = ObjectPool.GetObject(this.tag);
            child.transform.position = new Vector3(x, y, z);
            child.transform.rotation = Quaternion.identity;
            child.GetComponent<Health>().currentHealth = Health.maxHealth * 0.5f;

            /* Inherit infection */
            if (isInfection)
                transmitInfection(child);
            GameManager.instance.breed(child.tag);
        }
    }

    public virtual void transmitInfection(GameObject target) {}

    protected void CheckClick() {
        if (GameManager.instance.isHuntMode)
            return;

        if (Input.GetMouseButtonDown(0)) {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            Physics.Raycast(ray, out hit);
            if ((transform.position - hit.point).magnitude < 5) {
                if (GameManager.instance.currentButtonState == GameManager.ButtonState.Infection) {
                    UpdateInfection(true);
                } else {
                    CameraManager.instance.startSubCamera(this.gameObject);
                }
            }
        }
    }

    protected void UpdateInfection(bool turnOn) {
        if (turnOn) {
            isInfection = true;
            GetComponent<ParticleSystem>().Play();
        } else {
            isInfection = false;
            GetComponent<ParticleSystem>().Stop();
        }
    }
}
