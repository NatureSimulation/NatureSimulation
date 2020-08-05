using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamerControll : MonoBehaviour
{
    public float mouseSensitive = 50f;

    private float HorizonRot;
    private float VerticalRot;
    public float PlayerXRotValue;
    public float PlayerYRotValue;

    public Transform Axis;
    public Vector3 axis;

    void Start()
    {
        transform.GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        CamRotate();
    }

    void CamRotate() {
        HorizonRot = Input.GetAxis("Mouse X") * mouseSensitive * Time.deltaTime;
        VerticalRot = Input.GetAxis("Mouse Y") * -mouseSensitive * Time.deltaTime;

        PlayerXRotValue += VerticalRot;
        PlayerYRotValue += HorizonRot;
        PlayerXRotValue = Mathf.Clamp(PlayerXRotValue, -80, 80);
        Zvalue(0.0f);

        transform.rotation = Quaternion.Euler(PlayerXRotValue, PlayerYRotValue, 0);
    }

    private void Zvalue(float value) {
        Vector3 ZeulerRot = transform.eulerAngles;
        ZeulerRot.z = value;
        transform.eulerAngles = ZeulerRot;
    }
}
