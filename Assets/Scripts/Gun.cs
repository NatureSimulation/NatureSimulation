﻿
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour {

    public int BulletMax;
    public int BulletNow;

    public float BetweenShots;
    public float ReloadTime;
    public float muzzleVelocity;
    public float nextShotTime = 0f;
    public float nextReloadTime  = 0f;

    public bool ReloadValue = false;

    public GameObject Bullet;
    public Transform muzzle;

    public AudioClip fireSfx;
    public AudioClip reloadSfx;
    private AudioSource audiosource = null;

    public void Start() {
        audiosource = GetComponent<AudioSource>();
    }
    public void Shoot() {
        if (BulletNow > 0) {
            if (ReloadValue == false && Time.time > nextShotTime) {
                nextShotTime = Time.time + BetweenShots / 1000;
                GameObject newBullet = Instantiate (Bullet, Camera.main.transform.position, transform.rotation) as GameObject;
                Vector3 goalPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                goalPos += Camera.main.transform.forward * 10f;
                newBullet.transform.rotation = Quaternion.LookRotation(goalPos - Camera.main.transform.position, Vector3.up);
                Debug.DrawLine(Camera.main.transform.position, goalPos);

                newBullet.GetComponent<Bullet> ().SetSpeed (muzzleVelocity);
                Object.Destroy(newBullet, 2.0f);
                BulletNow -= 1;
                audiosource.PlayOneShot(fireSfx, 0.4f);
            }

        } else {
            if (ReloadValue == false) {
                Reload();
            }
        }
    }

    public void Reload() {
        nextReloadTime = Time.time + 2;
        ReloadValue = true;
        audiosource.PlayOneShot(reloadSfx, 0.8f);
    }
}
