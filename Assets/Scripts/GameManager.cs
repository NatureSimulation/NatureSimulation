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
    public int grassMax;
    public GameObject grassProgress;
    private Text grassProgressText;
    private Image grassProgressImage;

    /* Squirrel */
    public GameObject squirrel;
    public int squirrelInitNum;
    private int squirrelCount;
    public int squirrelMax;
    public GameObject squirrelProgress;
    private Text squirrelProgressText;
    private Image squirrelProgressImage;


    /* Rabbit */
    public GameObject rabbit;
    public int rabbitInitNum;
    private int rabbitCount;
    public int rabbitMax;
    public GameObject rabbitProgress;
    private Text rabbitProgressText;
    private Image rabbitProgressImage;

    /* Eagle */
    public GameObject eagle;
    public int eagleInitNum;
    private int eagleCount;
    public int eagleMax;
    public GameObject eagleProgress;
    private Text eagleProgressText;
    private Image eagleProgressImage;

    /* Deer */
    public GameObject deer;
    public int deerInitNum;
    private int deerCount;
    public int deerMax;
    public GameObject deerProgress;
    private Text deerProgressText;
    private Image deerProgressImage;

    /* Butterfly */
    public GameObject butterfly;
    public int butterflyInitNum;
    private int butterflyCount;
    public int butterflyMax;
    public GameObject butterflyProgress;
    private Text butterflyProgressText;
    private Image butterflyProgressImage;

    /* Tiger */
    public GameObject tiger;
    public int tigerInitNum;
    private int tigerCount;
    public int tigerMax;
    public GameObject tigerProgress;
    private Text tigerProgressText;
    private Image tigerProgressImage;

    /* Iguana */
    public GameObject iguana;
    public int iguanaInitNum;
    private int iguanaCount;
    public int iguanaMax;
    public GameObject iguanaProgress;
    private Text iguanaProgressText;
    private Image iguanaProgressImage;

    /* Bird */
    public GameObject bird;
    public int birdInitNum;
    private int birdCount;
    public int birdMax;
    public GameObject birdProgress;
    private Text birdProgressText;
    private Image birdProgressImage;

    /* Frog */
    public GameObject frog;
    public int frogInitNum;
    private int frogCount;
    public int frogMax;
    public GameObject frogProgress;
    private Text frogProgressText;
    private Image frogProgressImage;

    /* Map */
    public GameObject plane;
    private float planeMinX;
    private float planeMinZ;
    private float planeMaxX;
    private float planeMaxZ;
    private float planeMaxY;
    private float planeOffset = 5;

    public GameObject panel;


    void Awake() {
        instance = this;
    }

    void Start()
    {
        /* Init grass */
        grassTimer = 0;
        grassCount = 0;
        grassProgressText = grassProgress.transform.GetChild(1).GetComponent<Text>();
        grassProgressImage = grassProgress.transform.GetChild(0).GetComponent<Image>();
        grassProgressText.text = "Grass: " + "0";
        grassProgressImage.fillAmount = 0f;

        /* Init squirrel */
        squirrelCount = 0;
        squirrelProgressText = squirrelProgress.transform.GetChild(1).GetComponent<Text>();
        squirrelProgressImage = squirrelProgress.transform.GetChild(0).GetComponent<Image>();
        squirrelProgressText.text = "Squirrel: " + "0";
        squirrelProgressImage.fillAmount = 0f;

        /* Init rabbit */
        rabbitCount = 0;
        rabbitProgressText = rabbitProgress.transform.GetChild(1).GetComponent<Text>();
        rabbitProgressImage = rabbitProgress.transform.GetChild(0).GetComponent<Image>();
        rabbitProgressText.text = "Rabbit: " + "0";
        rabbitProgressImage.fillAmount = 0f;

        /* Init eagle */
        eagleCount = 0;
        eagleProgressText = eagleProgress.transform.GetChild(1).GetComponent<Text>();
        eagleProgressImage = eagleProgress.transform.GetChild(0).GetComponent<Image>();
        eagleProgressText.text = "Eagle: " + "0";
        eagleProgressImage.fillAmount = 0f;

        /* Init deer */
        deerCount = 0;
        deerProgressText = deerProgress.transform.GetChild(1).GetComponent<Text>();
        deerProgressImage = deerProgress.transform.GetChild(0).GetComponent<Image>();
        deerProgressText.text = "Deer: " + "0";
        deerProgressImage.fillAmount = 0f;

        /* Init butterfly */
        butterflyCount = 0;
        butterflyProgressText = butterflyProgress.transform.GetChild(1).GetComponent<Text>();
        butterflyProgressImage = butterflyProgress.transform.GetChild(0).GetComponent<Image>();
        butterflyProgressText.text = "Butterfly: " + "0";
        butterflyProgressImage.fillAmount = 0f;

        /* Init tiger */
        tigerCount = 0;
        tigerProgressText = tigerProgress.transform.GetChild(1).GetComponent<Text>();
        tigerProgressImage = tigerProgress.transform.GetChild(0).GetComponent<Image>();
        tigerProgressText.text = "Tiger: " + "0";
        tigerProgressImage.fillAmount = 0f;

        /* Init iguana */
        iguanaCount = 0;
        iguanaProgressText = iguanaProgress.transform.GetChild(1).GetComponent<Text>();
        iguanaProgressImage = iguanaProgress.transform.GetChild(0).GetComponent<Image>();
        iguanaProgressText.text = "Iguana: " + "0";
        iguanaProgressImage.fillAmount = 0f;

        /* Init bird */
        birdCount = 0;
        birdProgressText = birdProgress.transform.GetChild(1).GetComponent<Text>();
        birdProgressImage = birdProgress.transform.GetChild(0).GetComponent<Image>();
        birdProgressText.text = "Bird: " + "0";
        birdProgressImage.fillAmount = 0f;

        /* Init frog */
        frogCount = 0;
        frogProgressText = frogProgress.transform.GetChild(1).GetComponent<Text>();
        frogProgressImage = frogProgress.transform.GetChild(0).GetComponent<Image>();
        frogProgressText.text = "Frog: " + "0";
        frogProgressImage.fillAmount = 0f;

        /* Init map setting */
        Mesh mesh = plane.GetComponent<MeshFilter>().mesh;
        Bounds bounds = mesh.bounds;
        planeMinX = bounds.min.x + planeOffset;
        planeMinZ = bounds.min.z + planeOffset;
        planeMaxX = bounds.max.x - planeOffset;
        planeMaxZ = bounds.max.z - planeOffset;
        planeMaxY = bounds.max.y;

        /* Create squirrel */
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
        for (int i = 0; i < rabbitInitNum; i++) {
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
        for (int i = 0; i < eagleInitNum; ++i) {
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

        /* Create deer */
        for (int i = 0; i < deerInitNum; i++) {
            float x = Random.Range(planeMinX, planeMaxX);
            float z = Random.Range(planeMinZ, planeMaxZ);
            float y;
            try {
                y = getHeight(x, z);
            } catch (System.Exception) {
                continue;
            }

            deerCount += 1;
            setDeerProgress(true);
            Instantiate(deer, new Vector3(x, y, z), Quaternion.identity);
        }

        /* Create butterfly */
        for (int i = 0; i < butterflyInitNum; i++) {
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

        /* Create tiger */
        for (int i = 0; i < tigerInitNum; i++) {
            float x = Random.Range(planeMinX, planeMaxX);
            float z = Random.Range(planeMinZ, planeMaxZ);
            float y;
            try {
                y = getHeight(x, z);
            } catch (System.Exception) {
                continue;
            }

            tigerCount += 1;
            setTigerProgress(true);
            Instantiate(tiger, new Vector3(x, y, z), Quaternion.identity);
        }

        /* Create iguana */
        for (int i = 0; i < iguanaInitNum; i++) {
            float x = Random.Range(planeMinX, planeMaxX);
            float z = Random.Range(planeMinZ, planeMaxZ);
            float y;
            try {
                y = getHeight(x, z);
            } catch (System.Exception) {
                continue;
            }

            iguanaCount += 1;
            setIguanaProgress(true);
            Instantiate(iguana, new Vector3(x, y, z), Quaternion.identity);
        }

        /* Create bird */
        for (int i = 0; i < birdInitNum; i++) {
            float x = Random.Range(planeMinX, planeMaxX);
            float z = Random.Range(planeMinZ, planeMaxZ);
            float y;
            try {
                y = getHeight(x, z);
            } catch (System.Exception) {
                continue;
            }

            birdCount += 1;
            setBirdProgress(true);
            Instantiate(bird, new Vector3(x, y, z), Quaternion.identity);
        }

        /* Create frog */
        for (int i = 0; i < frogInitNum; i++) {
            float x = Random.Range(planeMinX, planeMaxX);
            float z = Random.Range(planeMinZ, planeMaxZ);
            float y;
            try {
                y = getHeight(x, z);
            } catch (System.Exception) {
                continue;
            }

            frogCount += 1;
            setFrogProgress(true);
            Instantiate(frog, new Vector3(x, y, z), Quaternion.identity);
        }

    }

    // Update is called once per frame
    void Update()
    {
        /* Pause */
        if (Input.GetKeyDown(KeyCode.P)) {
            if (panel.activeSelf) {
                panel.SetActive(false);
            } else {
                panel.SetActive(true);
            }
        }

        if (Input.GetKeyDown(KeyCode.Keypad1)) {
            Time.timeScale = 1;
        }
        if (Input.GetKeyDown(KeyCode.Keypad2)) {
            Time.timeScale = 2;
        }
        if (Input.GetKeyDown(KeyCode.Keypad3)) {
            Time.timeScale = 3;
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
                Debug.Log("grass generated error");
                return;
            }
            Debug.Log("grass generated");

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
            // Debug.Log("grass generated at: " + hit.point);
            return hit.point.y;
        }
        throw new System.Exception("Invalid coordinate");
    }

    void setGrassProgress(bool isIncrease) {
        grassProgressText.text = "Grass:" + grassCount.ToString();
        if (isIncrease) {
            grassProgressImage.fillAmount += 1.0f / grassMax;
        } else {
            grassProgressImage.fillAmount -= 1.0f / grassMax;
        }

    }

    void setSquirrelProgress(bool isIncrease) {
        squirrelProgressText.text = "Squirrel: " + squirrelCount.ToString();
        if (isIncrease) {
            squirrelProgressImage.fillAmount += 1.0f / squirrelMax;
        } else {
            squirrelProgressImage.fillAmount -= 1.0f / squirrelMax;
        }
    }

    void setRabbitProgress(bool isIncrease) {
        rabbitProgressText.text = "Rabbit: " + rabbitCount.ToString();
        if (isIncrease) {
            rabbitProgressImage.fillAmount += 1.0f / rabbitMax;
        } else {
            rabbitProgressImage.fillAmount -= 1.0f / rabbitMax;
        }
    }

    void setButterflyProgress(bool isIncrease) {
        butterflyProgressText.text = "Butterfly: " + butterflyCount.ToString();
        if (isIncrease) {
            butterflyProgressImage.fillAmount += 1.0f / butterflyMax;
        } else {
            butterflyProgressImage.fillAmount -= 1.0f / butterflyMax;
        }
    }

    void setEagleProgress(bool isIncrease) {
        eagleProgressText.text = "Eagle: " + eagleCount.ToString();
        if (isIncrease) {
            eagleProgressImage.fillAmount += 1.0f / eagleMax;
        } else {
            eagleProgressImage.fillAmount -= 1.0f / eagleMax;
        }
    }

    void setDeerProgress(bool isIncrease) {
        deerProgressText.text = "Deer: " + deerCount.ToString();
        if (isIncrease) {
            deerProgressImage.fillAmount += 1.0f / deerMax;
        } else {
            deerProgressImage.fillAmount -= 1.0f / deerMax;
        }
    }

    void setTigerProgress(bool isIncrease) {
        tigerProgressText.text = "Tiger: " + tigerCount.ToString();
        if (isIncrease) {
            tigerProgressImage.fillAmount += 1.0f / tigerMax;
        } else {
            tigerProgressImage.fillAmount -= 1.0f / tigerMax;
        }
    }

    void setIguanaProgress(bool isIncrease) {
        iguanaProgressText.text = "Iguana: " + iguanaCount.ToString();
        if (isIncrease) {
            iguanaProgressImage.fillAmount += 1.0f / iguanaMax;
        } else {
            iguanaProgressImage.fillAmount -= 1.0f / iguanaMax;
        }
    }

    void setBirdProgress(bool isIncrease) {
        birdProgressText.text = "Bird: " + birdCount.ToString();
        if (isIncrease) {
            birdProgressImage.fillAmount += 1.0f / birdMax;
        } else {
            birdProgressImage.fillAmount -= 1.0f / birdMax;
        }
    }

    void setFrogProgress(bool isIncrease) {
        frogProgressText.text = "Frog: " + frogCount.ToString();
        if (isIncrease) {
            frogProgressImage.fillAmount += 1.0f / frogMax;
        } else {
            frogProgressImage.fillAmount -= 1.0f / frogMax;
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
        } else if (tag == "Bird") {
            birdCount -= 1;
            setBirdProgress(false);
        } else if (tag == "Frog") {
            frogCount -= 1;
            setFrogProgress(false);
        } else if (tag == "Butterfly") {
            butterflyCount -= 1;
            setButterflyProgress(false);
        }

        Destroy(item);
    }
}
