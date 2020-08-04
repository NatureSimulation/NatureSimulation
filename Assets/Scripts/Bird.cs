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
            planeY = GameManager.instance.getHeight(this.transform.position.x, this.transform.position.z);
        } catch {
            planeY = 0;
        }

        Quaternion rotation;
        if (planeY + 5 > this.transform.position.y) {
            rotation = Quaternion.Euler(new Vector3(-30, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z));
        } else {
            rotation = Quaternion.Euler(new Vector3(30, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z));
        }
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * 2.0f);
    }

    public override void RotateOnTargeting(GameObject obj) {
        transform.rotation = Quaternion.LookRotation(obj.transform.position - transform.position, Vector3.up);
    }

    public override void UpdatePosition() {
        transform.Translate(transform.forward * speed * Time.deltaTime, Space.World);
    }

    public override void UpdateSpeed(float speed) {
        this.speed = speed;
    }
}
