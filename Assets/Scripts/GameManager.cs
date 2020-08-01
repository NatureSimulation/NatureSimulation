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

    /* Deer */
    public GameObject deer;
    private int deerCount;
    private int deerMax;
    public GameObject deerProgress;
    private Text deerProgressText;
    private Image deerProgressImage;

    /* Butterfly */
    public GameObject butterfly;
    private int butterflyCount;
    private int butterflyMax;
    public GameObject butterflyProgress;
    private Text butterflyProgressText;
    private Image butterflyProgressImage;

    /* Tiger */
    public GameObject tiger;
    private int tigerCount;
    private int tigerMax;
    public GameObject tigerProgress;
    private Text tigerProgressText;
    private Image tigerProgressImage;

    /* Iguana */
    public GameObject iguana;
    private int iguanaCount;
    private int iguanaMax;
    public GameObject iguanaProgress;
    private Text iguanaProgressText;
    private Image iguanaProgressImage;

    /* Map */
    public GameObject plane;
    private float planeMinX;
    private float planeMinZ;
    private float planeMaxX;
    private float planeMaxZ;
    private float planeMaxY;

    public GameObject panel;


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

        /* Init squirrel */
        squirrelCount = 0;
        squirrelMax = 10;
        squirrelProgressText = squirrelProgress.transform.GetChild(1).GetComponent<Text>();
        squirrelProgressImage = squirrelProgress.transform.GetChild(0).GetComponent<Image>();
        squirrelProgressText.text = "0";
        squirrelProgressImage.fillAmount = 0f;

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
        eagleProgressText.text = "0";
        eagleProgressImage.fillAmount = 0f;

        /* Init deer */
        deerCount = 0;
        deerMax = 10;
        deerProgressText = deerProgress.transform.GetChild(1).GetComponent<Text>();
        deerProgressImage = deerProgress.transform.GetChild(0).GetComponent<Image>();
        deerProgressText.text = "0";
        deerProgressImage.fillAmount = 0f;

        /* Init butterfly */
        butterflyCount = 0;
        butterflyMax = 10;
        butterflyProgressText = butterflyProgress.transform.GetChild(1).GetComponent<Text>();
        butterflyProgressImage = butterflyProgress.transform.GetChild(0).GetComponent<Image>();
        butterflyProgressText.text = "0";
        butterflyProgressImage.fillAmount = 0f;

        /* Init tiger */
        tigerCount = 0;
        tigerMax = 10;
        tigerProgressText = tigerProgress.transform.GetChild(1).GetComponent<Text>();
        tigerProgressImage = tigerProgress.transform.GetChild(0).GetComponent<Image>();
        tigerProgressText.text = "0";
        tigerProgressImage.fillAmount = 0f;

        /* Init iguana */
        iguanaCount = 0;
        iguanaMax = 10;
        iguanaProgressText = iguanaProgress.transform.GetChild(1).GetComponent<Text>();
        iguanaProgressImage = iguanaProgress.transform.GetChild(0).GetComponent<Image>();
        iguanaProgressText.text = "0";
        iguanaProgressImage.fillAmount = 0f;

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
            setSquirrelProgress(true);
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

        /* Create butterfly */
        for (int i = 0; i < 10; i++) {
            float x = Random.Range(planeMinX, planeMaxX);
            float z = Random.Range(planeMinZ, planeMaxZ);
            float y;
            try {
                y = getHeight(x, z);
            } catch (System.Exception) {
                continue;
            }

            butterflyCount += 1;
            setButterflyProgress(true);
            Instantiate(butterfly, new Vector3(x, y, z), Quaternion.identity);
        }

    }

    // Update is called once per frame
    void Update()
    {
        /* Pause */
        if (Input.GetKeyDown(KeyCode.P)) {
            if (panel.activeSelf) {
                Time.timeScale = 1;
                panel.SetActive(false);
            } else {
                Time.timeScale = 0;
                panel.SetActive(true);
            }

        }
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

    void setSquirrelProgress(bool isIncrease) {
        squirrelProgressText.text = squirrelCount.ToString();
        if (isIncrease) {
            squirrelProgressImage.fillAmount += 1.0f / squirrelMax;
        } else {
            squirrelProgressImage.fillAmount -= 1.0f / squirrelMax;
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

    void setButterflyProgress(bool isIncrease) {
        butterflyProgressText.text = butterflyCount.ToString();
        if (isIncrease) {
            butterflyProgressImage.fillAmount += 1.0f / butterflyMax;
        } else {
            butterflyProgressImage.fillAmount -= 1.0f / butterflyMax;
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

    void setDeerProgress(bool isIncrease) {
        deerProgressText.text = deerCount.ToString();
        if (isIncrease) {
            deerProgressImage.fillAmount += 1.0f / deerMax;
        } else {
            deerProgressImage.fillAmount -= 1.0f / deerMax;
        }
    }

    void setTigerProgress(bool isIncrease) {
        tigerProgressText.text = tigerCount.ToString();
        if (isIncrease) {
            tigerProgressImage.fillAmount += 1.0f / tigerMax;
        } else {
            tigerProgressImage.fillAmount -= 1.0f / tigerMax;
        }
    }

    void setIguanaProgress(bool isIncrease) {
        iguanaProgressText.text = iguanaCount.ToString();
        if (isIncrease) {
            iguanaProgressImage.fillAmount += 1.0f / iguanaMax;
        } else {
            iguanaProgressImage.fillAmount -= 1.0f / iguanaMax;
        }
    } 

    public void delete(GameObject item, string tag) {
        if (tag == "Grass") {
            grassCount -= 1;
            setGrassProgress(false);
        } else if (tag == "Rabbit") {
            rabbitCount -= 1;
            setRabbitProgress(false);
        } else if (tag == "Eagle") {
            eagleCount -= 1;
            setEagleProgress(false);
        } else if (tag == "Deer") {
            deerCount -= 1;
            setDeerProgress(false);
        } else if (tag == "Squirrel") {
            squirrelCount -= 1;
            setSquirrelProgress(false);
        } else if (tag == "Tiger") {
            tigerCount -= 1;
            setTigerProgress(false);
        } else if (tag == "Iguana") {
            iguanaCount -= 1;
            setIguanaProgress(false);
        }

        Destroy(item);
    }
}
