using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Deer : Animal {
    public override void Start() {
        base.Start();
    }

    public override void Update() {
        base.Update();
    }

    public void Reset() {
        speed = 5f;
        wanderSpeed = 5f;
        escapeSpeed = 20f;
        predators = new string[]{ "Tiger" };
        preys = new string[]{ "Grass" };
    }

    public override void RotateOnTargeting(GameObject obj) {
        Vector3 diff = obj.transform.position - transform.position;
        transform.rotation = Quaternion.LookRotation(new Vector3(diff.x, 0, diff.z), Vector3.up);
    }

    public override void UpdatePosition() {
        if (!player.isGrounded) {
            Vector3 moveDirection = transform.forward;
            moveDirection.y -= gravity * Time.deltaTime;
            player.Move(moveDirection * speed * Time.deltaTime);
        } else {
            player.Move(transform.forward * speed * Time.deltaTime);
        }
    }

    public override void UpdateSpeed(float speed) {
        this.speed = speed;
    }

    public override void transmitInfection(GameObject target) {
        if (target == null)
            return;
        target.GetComponent<Deer>().UpdateInfection(true);
    }
}
