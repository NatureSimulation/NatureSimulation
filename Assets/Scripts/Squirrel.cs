using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Squirrel : Animal {
    public override void Start() {
        base.Start();
    }

    public override void Update() {
        base.Update();
    }

    public void Reset() {
        speed = 3f;
        wanderSpeed = 3f;
        escapeSpeed = 10f;
        predators = new string[]{ "Bird", "Iguana" };
        preys = new string[]{ "Grass" };
    }

    public override void RotateOnTargeting(GameObject obj) {
        Vector3 diff = obj.transform.position - transform.position;
        transform.rotation = Quaternion.LookRotation(new Vector3(diff.x, 0, diff.z), Vector3.up);
    }

    public override void UpdatePosition() {
        animator.SetTrigger("move");
        transform.position += (transform.forward * speed * Time.deltaTime);
    }

    public override void UpdateSpeed(float speed) {
        this.speed = speed;
        animator.speed = speed;
    }
}
