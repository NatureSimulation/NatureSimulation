﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager instance;
    public GameObject mainCamera;
    public GameObject subCamera;
    void Awake() {
        instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void startSubCamera(GameObject player) {
        if (!subCamera.activeSelf) {
            mainCamera.SetActive(false);
            subCamera.SetActive(true);
        }
        SubCamera.instance.instantiate(player);
    }

    public void finishSubCamera() {
        if (!mainCamera.activeSelf) {
            SubCamera.instance.finish();
            mainCamera.SetActive(true);
            subCamera.SetActive(false);
        }

    }
}
