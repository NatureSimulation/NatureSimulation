using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GunController : MonoBehaviour {

    public Gun newGun;

    void Start () {
        newGun.ReloadValue = false;
    }

    void Update() {
        if (newGun.ReloadValue == true && Time.time >= newGun.nextReloadTime) {
            newGun.ReloadValue = false;
            newGun.BulletNow = newGun.BulletMax;
        }
    }

    public void  Shoot() {
        newGun.Shoot ();
    }

    public void Reload() {
        newGun.Reload ();
    }
}
