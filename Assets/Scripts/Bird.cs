using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Bird : Animal {
    public override void Start() {
        base.Start();
    }

    public override void Update() {
        base.Update();
    }

    public void Reset() {
        speed = 5f;
        wanderSpeed = 5f;
        predators = new string[]{};
        preys = new string[]{ "Squirrel" };
    }

    public override void AdjustHeight() {
        float planeY;
        try {
            planeY = GameManager.instance.getHeight(this.transform.position.x, this.transform.position.y);
        } catch {
            planeY = 0;
        }

        Quaternion rotation;
        if (planeY + 5 > this.transform.position.y) {
            rotation = Quaternion.Euler(new Vector3(-5, transform.rotation.y, transform.rotation.z));
        } else {
            rotation = Quaternion.Euler(new Vector3(5, transform.rotation.y, transform.rotation.z));
        }
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * 2.0f);
    }

    public override void RotateOnTargeting(GameObject obj) {
        transform.rotation = Quaternion.LookRotation(obj.transform.position - transform.position, Vector3.up);
    }

    public override void UpdatePosition() {
        transform.position += (transform.forward * speed * Time.deltaTime);
    }

    public override void UpdateSpeed(float speed) {
        this.speed = speed;
    }
}
