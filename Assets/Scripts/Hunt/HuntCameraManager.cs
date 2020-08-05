using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HuntCameraManager : MonoBehaviour
{
    public static HuntCameraManager instance;
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

    public void StartSubCamera() {
        if (!subCamera.activeSelf) {
            mainCamera.SetActive(false);
            subCamera.SetActive(true);
        }
    }

    public void FinishSubCamera() {
        if (!mainCamera.activeSelf) {
            mainCamera.SetActive(true);
            subCamera.SetActive(false);
        }

    }
}
