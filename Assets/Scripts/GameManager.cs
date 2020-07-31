using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{   
    /* External variables */
    /* For singleton pattern */
    public static GameManager instance;

    /* Grass */
    public GameObject grass;
    private float grassTimer;
    private int grassCount;
    private int grassMax;
    public GameObject grassProgress;
    private Text grassProgressText;
    private Image grassProgressImage;

    /* Eagle */
    public GameObject eagle;
    private int eagleCount;

    /* Rabbit */
    public GameObject rabbit;

    /* Map */
    public GameObject plane;
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
        /* Init grass */
        grassTimer = 0;
        grassCount = 0;
        grassMax = 100;
        grassProgressText = grassProgress.transform.GetChild(1).GetComponent<Text>();
        grassProgressImage = grassProgress.transform.GetChild(0).GetComponent<Image>();
        grassProgressText.text = "0";
        grassProgressImage.fillAmount = 0f;

        /* Init map setting */
        Mesh mesh = plane.GetComponent<MeshFilter>().mesh;
        Bounds bounds = mesh.bounds;
        planeMinX = bounds.min.x;
        planeMinZ = bounds.min.z;
        planeMaxX = bounds.max.x;
        planeMaxZ = bounds.max.z;
        planeMaxY = bounds.max.y;

        /* Create eagle */
        eagleCount = 0;

        for (int i = 0; i < 1; ++i) {
            float x = Random.Range(planeMinX, planeMaxX);
            float z = Random.Range(planeMinZ, planeMaxZ);
            float y = 10f;
            // try {
            //     y = getHeight(x, z) + 10f;
            // } catch (System.Exception) {
            //     continue;
            // }

            eagleCount += 1;
            Instantiate (eagle, new Vector3(x, y, z), Quaternion.identity);
        }

        for (int i = 0; i < 10; i++) {
            float x = Random.Range(planeMinX, planeMaxX);
            float z = Random.Range(planeMinZ, planeMaxZ);
            float y = getHeight(x, z);
            Instantiate(rabbit, new Vector3(x, y, z), Quaternion.identity);
        }
    }

    // Update is called once per frame
    void Update()
    {
        /* Create grass */
        grassTimer += Time.deltaTime;
        if (grassTimer > 1) {
            grassTimer = 0;
            float x = Random.Range(planeMinX, planeMaxX);
            float z = Random.Range(planeMinZ, planeMaxZ);
            float y;
            try {
                y = getHeight(x, z);
            } catch (System.Exception) {
                return;
            }

            grassCount += 1;
            setGrassProgress();
            Instantiate (grass, new Vector3(x, y, z), Quaternion.identity);
        }
    }

    /* Return height of map */
    public float getHeight(float x, float z) {
        RaycastHit hit;
        Ray ray = new Ray(new Vector3(x, planeMaxY, z), Vector3.down);
        if (plane.GetComponent<Collider>().Raycast(ray, out hit, 2.0f * planeMaxY)) {
            Debug.Log("grass generated at: " + hit.point);
            return hit.point.y;
        }
        Debug.Log("grass generated error");
        throw new System.Exception("Invalid coordinate");
    } 

    void setGrassProgress() {
        grassProgressText.text = grassCount.ToString();
        grassProgressImage.fillAmount += 1.0f / grassMax;
    }
}
