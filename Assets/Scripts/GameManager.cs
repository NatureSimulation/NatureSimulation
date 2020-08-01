﻿using System.Collections;
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

    /* Squirrel */
    public GameObject squirrel;
    private int squirrelCount;
    private int squirrelMax;
    public GameObject squirrelProgress;
    private Text squirrelProgressText;
    private Image squirrelProgressImage;


    /* Rabbit */
    public GameObject rabbit;
    private int rabbitCount;
    private int rabbitMax;
    public GameObject rabbitProgress;
    private Text rabbitProgressText;
    private Image rabbitProgressImage;

    /* Eagle */
    public GameObject eagle;
    private int eagleCount;
    private int eagleMax;
    public GameObject eagleProgress;
    private Text eagleProgressText;
    private Image eagleProgressImage;

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

        /* Init rabbit */
        rabbitCount = 0;
        rabbitMax = 10;
        rabbitProgressText = rabbitProgress.transform.GetChild(1).GetComponent<Text>();
        rabbitProgressImage = rabbitProgress.transform.GetChild(0).GetComponent<Image>();
        rabbitProgressText.text = "0";
        rabbitProgressImage.fillAmount = 0f;

        /* Init eagle */
        eagleCount = 0;
        eagleMax = 10;
        eagleProgressText = eagleProgress.transform.GetChild(1).GetComponent<Text>();
        eagleProgressImage = eagleProgress.transform.GetChild(0).GetComponent<Image>();

        /* Init map setting */
        Mesh mesh = plane.GetComponent<MeshFilter>().mesh;
        Bounds bounds = mesh.bounds;
        planeMinX = bounds.min.x;
        planeMinZ = bounds.min.z;
        planeMaxX = bounds.max.x;
        planeMaxZ = bounds.max.z;
        planeMaxY = bounds.max.y;

        for (int i = 0; i < 10; i++) {
            float x = Random.Range(planeMinX, planeMaxX);
            float z = Random.Range(planeMinZ, planeMaxZ);
            float y;
            try {
                y = getHeight(x, z);
            } catch (System.Exception) {
                continue;
            }

            squirrelCount += 1;
            Instantiate (squirrel, new Vector3(x, y, z), Quaternion.identity);
        }

        /* Create rabbit */
        for (int i = 0; i < 10; i++) {
            float x = Random.Range(planeMinX, planeMaxX);
            float z = Random.Range(planeMinZ, planeMaxZ);
            float y;
            try {
                y = getHeight(x, z);
            } catch (System.Exception) {
                continue;
            }
            
            rabbitCount += 1;
            setRabbitProgress(true);
            Instantiate(rabbit, new Vector3(x, y, z), Quaternion.identity);
        }

        /* Create eagle */
        for (int i = 0; i < 10; ++i) {
            float x = Random.Range(planeMinX, planeMaxX);
            float z = Random.Range(planeMinZ, planeMaxZ);
            float y;
            try {
                y = getHeight(x, z) + 10f;
            } catch (System.Exception) {
                continue;
            }

            eagleCount += 1;
            setEagleProgress(true);
            Instantiate (eagle, new Vector3(x, y, z), Quaternion.identity);
        }
    }

    // Update is called once per frame
    void Update()
    {
        /* Create grass */
        grassTimer += Time.deltaTime;
        if (grassTimer > 3) {
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
            setGrassProgress(true);
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

    void setGrassProgress(bool isIncrease) {
        grassProgressText.text = grassCount.ToString();
        if (isIncrease) {
            grassProgressImage.fillAmount += 1.0f / grassMax;
        } else {
            grassProgressImage.fillAmount -= 1.0f / grassMax;
        }
        
    }

    void setRabbitProgress(bool isIncrease) {
        rabbitProgressText.text = rabbitCount.ToString();
        if (isIncrease) {
            rabbitProgressImage.fillAmount += 1.0f / rabbitMax;
        } else {
            rabbitProgressImage.fillAmount -= 1.0f / rabbitMax;
        }
    }

    void setEagleProgress(bool isIncrease) {
        eagleProgressText.text = eagleCount.ToString();
        if (isIncrease) {
            eagleProgressImage.fillAmount += 1.0f / eagleMax;
        } else {
            eagleProgressImage.fillAmount -= 1.0f / eagleMax;
        }
    }

    public void delete(GameObject item, string tag) {
        if (tag == "Grass") {
            grassCount -= 1;
            setGrassProgress(false);
            Destroy(item);
        } else if (tag == "Rabbit") {
            rabbitCount -= 1;
            setRabbitProgress(false);
            Destroy(item);
        } else if (tag == "Eagle") {
            eagleCount -= 1;
            setEagleProgress(false);
            Destroy(item);
        }
    }
}
