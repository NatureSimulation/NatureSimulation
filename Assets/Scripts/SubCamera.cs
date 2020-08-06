using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubCamera : MonoBehaviour
{
    public static SubCamera instance;

    private GameObject player;

    void Awake() {
        instance = this;
        player = null;
    }

    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (player == null || !player.activeSelf) {
            CameraManager.instance.finishSubCamera();
            return;
        }
        this.transform.position = new Vector3(player.transform.position.x, player.transform.position.y + 5, player.transform.position.z);
        Quaternion direction = Quaternion.Euler(new Vector3(0, player.transform.eulerAngles.y, player.transform.eulerAngles.z));
        this.transform.rotation = direction;
    }

    public void instantiate(GameObject player) {
        this.player = player;
    }

    public void finish() {
        this.player = null;
    }
}
