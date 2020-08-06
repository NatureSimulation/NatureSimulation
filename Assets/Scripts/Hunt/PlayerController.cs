using UnityEngine;
using System.Collections;

[RequireComponent (typeof(CharacterController))]

public class PlayerController : MonoBehaviour {

    public float movementSpeed = 5f;
    public float mouseSensitivity = 10f;
    public float upDownRange= 90;
    public float jumpSpeed = 5;
    public float downSpeed = -5;

    private Vector3 speed;
    private float forwardSpeed;
    private float sideSpeed;

    private float rotLeftRight = 0;
    private float rotUpDown;
    private float verticalRotation = 0f;
    private float verticalVelocity = 0f;
    private GunController PlayerGunCtrl;

    private CharacterController cc;

    // Use this for initialization
    void Start () {
        cc = GetComponent<CharacterController> ();
        Cursor.lockState = CursorLockMode.Locked;
        PlayerGunCtrl = GetComponent<GunController> ();
    }

    // Update is called once per frame
    void Update () {
        if (Cursor.lockState == CursorLockMode.None) {
            if (Input.GetMouseButton (0)) {
                Cursor.lockState = CursorLockMode.Locked;
            }
            return;
        }
        FPMove ();
        FPRotate ();
        Shoot();
    }


    void FPMove()
    {
        forwardSpeed = Input.GetAxisRaw ("Vertical") * movementSpeed;
        sideSpeed = Input.GetAxisRaw ("Horizontal") * movementSpeed;

        //if (cc.isGrounded && Input.GetButtonDown ("Jump"))
            //verticalVelocity = jumpSpeed;
        if (!cc.isGrounded)
            verticalVelocity = downSpeed;

        verticalVelocity += Physics.gravity.y * Time.deltaTime;

        speed = new Vector3 (sideSpeed, verticalVelocity, forwardSpeed);
        speed = transform.rotation * speed;

        cc.Move (speed * Time.deltaTime);
    }

    void FPRotate()
    {
        rotLeftRight = Input.GetAxis ("Mouse X") * mouseSensitivity;
        transform.rotation *= Quaternion.Euler(0f, rotLeftRight, 0f);

        verticalRotation -= Input.GetAxis ("Mouse Y") * mouseSensitivity;
        verticalRotation = Mathf.Clamp (verticalRotation, -upDownRange, upDownRange);
        Camera.main.transform.localRotation = Quaternion.Euler (verticalRotation, 0f, 0f);
    }

    void Shoot() {
        if (Input.GetMouseButton (0)) {
            PlayerGunCtrl.Shoot ();
        }
        if (Input.GetKey (KeyCode.R)
            && PlayerGunCtrl.newGun.BulletNow != PlayerGunCtrl.newGun.BulletMax) {
            PlayerGunCtrl.Reload ();
        }
    }
}
