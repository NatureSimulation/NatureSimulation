using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Butterfly : Animal {
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
        predators = new string[]{ "Frog", "Squirrel" };
        preys = new string[]{ "Grass" };
    }

    public override void RotateOnTargeting(GameObject obj) {
        Vector3 diff = obj.transform.position - transform.position;
        transform.rotation = Quaternion.LookRotation(new Vector3(diff.x, 0, diff.z), Vector3.up);
    }

    public override void AdjustHeight() {
        float planeY;
        try {
            planeY = GameManager.instance.getHeight(this.transform.position.x, this.transform.position.z);
        } catch {
            planeY = 0;
        }

        Quaternion rotation;
        if (planeY + 3 > this.transform.position.y) {
            rotation = Quaternion.Euler(new Vector3(-10, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z));
        } else {
            rotation = Quaternion.Euler(new Vector3(10, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z));
        }
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * 2.0f);
    }

    public override void UpdatePosition() {
        transform.position += (transform.forward * speed * Time.deltaTime);
    }

    public override void UpdateSpeed(float speed) {
        this.speed = speed;
    }
}
