using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    /* External variables */
    public GameObject grass;
    public GameObject plane;
    /* For singleton pattern */
    public static GameManager instance;

    /* About Grass */
    private float GrassTimer;
    /* About map */
    private float planeMinX;
    private float planeMinZ;
    private float planeMaxX;
    private float planeMaxZ;
    private float planeMaxY;

    
    void Awake() {
        instance = this;
    }

    void Start()
    {
        /* Init grass timer */
        GrassTimer = 0;

        /* Init map setting */
        Mesh mesh = plane.GetComponent<MeshFilter>().mesh;
        Bounds bounds = mesh.bounds;
        planeMinX = bounds.min.x;
        planeMinZ = bounds.min.z;
        planeMaxX = bounds.max.x;
        planeMaxZ = bounds.max.z;
        planeMaxY = bounds.max.y;
    }

    // Update is called once per frame
    void Update()
    {
        /* Create grass */
        GrassTimer += Time.deltaTime;
        if (GrassTimer > 1) {
            GrassTimer = 0;
            float x = Random.Range(planeMinX, planeMaxX);
            float z = Random.Range(planeMinZ, planeMaxZ);
            float y = getHeight(x, z);
            GameObject debug = Instantiate (grass, new Vector3(x, y, z), Quaternion.identity);
        }
    }

    /* Return height of map */
    float getHeight(float x, float z) {
        RaycastHit hit;
        Ray ray = new Ray(new Vector3(x, planeMaxY, z), Vector3.down);
        if (plane.GetComponent<Collider>().Raycast(ray, out hit, 2.0f * planeMaxY)) {
            Debug.Log("grass generated at: " + hit.point);
            return hit.point.y;
        }
        Debug.Log("grass generated error");
        return -5f;
    } 
}
