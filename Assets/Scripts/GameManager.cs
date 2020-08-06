using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    /* External variables */
    /* For singleton pattern */
    public static GameManager instance;

    public TextMeshProUGUI HuntCountText;
    public int HuntCount = 0;
    private int extinctCount = 0;
    public TextMeshProUGUI GameOverKills;
    public TextMeshProUGUI GameOverTime;
    public GameObject HuntOverlayPanel;
    public GameObject GameOverPanel;

    public ParticleSystem[] lightnings;
    public bool lightningOn;
    public bool intermittentLightning;
    public float lightningCooltime;

    public TextMeshProUGUI timerText;
    private float gameTimer;
    private int aliveCount;

    public enum ButtonState {
        None,
        Grass,
        Bird,
        Butterfly,
        Deer,
        Eagle,
        Frog,
        Iguana,
        Rabbit,
        Squirrel,
        Tiger,
        Infection
    }
    public Button grassButton;
    public Button birdButton;
    public Button butterflyButton;
    public Button deerButton;
    public Button eagleButton;
    public Button frogButton;
    public Button iguanaButton;
    public Button rabbitButton;
    public Button squirrelButton;
    public Button tigerButton;
    public Button infectionButton;
    public ButtonState currentButtonState;
    public bool isHuntMode = false;

    /* Grass */
    public GameObject grass;
    private float grassTimer;
    private int grassCount;
    public float grassCoolTime;
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
    public float planeMinX;
    public float planeMinZ;
    public float planeMaxX;
    public float planeMaxZ;
    public float planeMaxY;
    public float planeOffset = 5;

    public GameObject panel;
    public GameObject menu;
    public GameObject animalPanel;

    /* Debug */
    public bool initCreate = true;

    void Awake() {
        instance = this;
    }

    void Start()
    {
        foreach (var lightning in lightnings) {
            lightning.Stop();
        }

        if (!isHuntMode) {
            currentButtonState = ButtonState.None;
            grassButton.onClick.AddListener(() => { currentButtonState = ButtonState.Grass; grassButton.GetComponent<Image>().color = Color.white; });
            birdButton.onClick.AddListener(() => { currentButtonState = ButtonState.Bird; birdButton.GetComponent<Image>().color = Color.white; });
            butterflyButton.onClick.AddListener(() => { currentButtonState = ButtonState.Butterfly; butterflyButton.GetComponent<Image>().color = Color.white; });
            deerButton.onClick.AddListener(() => { currentButtonState = ButtonState.Deer; deerButton.GetComponent<Image>().color = Color.white; });
            eagleButton.onClick.AddListener(() => { currentButtonState = ButtonState.Eagle; eagleButton.GetComponent<Image>().color = Color.white; });
            frogButton.onClick.AddListener(() => { currentButtonState = ButtonState.Frog; frogButton.GetComponent<Image>().color = Color.white; });
            iguanaButton.onClick.AddListener(() => { currentButtonState = ButtonState.Iguana; iguanaButton.GetComponent<Image>().color = Color.white; });
            rabbitButton.onClick.AddListener(() => { currentButtonState = ButtonState.Rabbit; rabbitButton.GetComponent<Image>().color = Color.white; });
            squirrelButton.onClick.AddListener(() => { currentButtonState = ButtonState.Squirrel; squirrelButton.GetComponent<Image>().color = Color.white; });
            tigerButton.onClick.AddListener(() => { currentButtonState = ButtonState.Tiger; tigerButton.GetComponent<Image>().color = Color.white; });
            infectionButton.onClick.AddListener(() => { currentButtonState = ButtonState.Infection; infectionButton.GetComponent<Image>().color = Color.white; });
        } else {
            HuntCountText.text = "Kills:\t0";
        }


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
        if (!initCreate)
            return;
        /* Create squirrel */
        for (int i = 0; i < squirrelInitNum; i++) {
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

        gameTimer = 0f;
        aliveCount = new int[]{
            birdInitNum,
            butterflyInitNum,
            eagleInitNum,
            iguanaInitNum,
            rabbitInitNum,
            deerInitNum,
            squirrelInitNum,
            tigerInitNum,
            frogInitNum
        }.Aggregate(0, (acc, cur) => acc + (cur > 0 ? 1 : 0));

        if (intermittentLightning)
            StartCoroutine(IntermittentLightning());
    }

    // Update is called once per frame
    void Update()
    {
        UpdateAndDisplayTime();
        /* Pause */
        if (Input.GetKeyDown(KeyCode.F)) {
            panel.SetActive(!panel.activeSelf);
            if (!isHuntMode)
                animalPanel.SetActive(!animalPanel.activeSelf);
            if (isHuntMode)
                menu.SetActive(!menu.activeSelf);
        }

        if (Input.GetKeyDown(KeyCode.Alpha1)) {
            Time.timeScale = 1;
        }
        if (Input.GetKeyDown(KeyCode.Alpha2)) {
            Time.timeScale = 2;
        }
        if (Input.GetKeyDown(KeyCode.Alpha3)) {
            Time.timeScale = 3;
        }
        if (Input.GetKeyDown(KeyCode.Alpha4)) {
            Time.timeScale = 4;
        }
        if (Input.GetKeyDown(KeyCode.Alpha5)) {
            Time.timeScale = 5;
        }
        if (Input.GetKeyDown(KeyCode.Alpha6)) {
            Time.timeScale = 6;
        }
        if (Input.GetKeyDown(KeyCode.Alpha7)) {
            Time.timeScale = 7;
        }
        if (Input.GetKeyDown(KeyCode.Alpha8)) {
            Time.timeScale = 8;
        }
        if (Input.GetKeyDown(KeyCode.Alpha9)) {
            Time.timeScale = 9;
        }
        if (Input.GetKeyDown(KeyCode.Escape)) {
            if (SceneManager.GetActiveScene().name == "HuntScene") {
                Cursor.lockState = CursorLockMode.None;
            } else {
                CameraManager.instance.finishSubCamera();
            }
        }

        if (Input.GetKeyDown(KeyCode.L)) {
            StartCoroutine(FlashLightning());
        }

        if (!isHuntMode) {
            if (Input.GetMouseButtonDown(0)) {
                if (EventSystem.current.IsPointerOverGameObject())
                    return;
                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out hit)) {
                    // Debug.Log(hit.point);
                    GameObject obj = null;
                    switch (currentButtonState) {
                        case ButtonState.Bird:
                            obj = ObjectPool.GetObject("Bird");
                            break;
                        case ButtonState.Butterfly:
                            obj = ObjectPool.GetObject("Butterfly");
                            break;
                        case ButtonState.Deer:
                            obj = ObjectPool.GetObject("Deer");
                            break;
                        case ButtonState.Eagle:
                            obj = ObjectPool.GetObject("Eagle");
                            break;
                        case ButtonState.Frog:
                            obj = ObjectPool.GetObject("Frog");
                            break;
                        case ButtonState.Grass:
                            obj = ObjectPool.GetObject("Grass");
                            break;
                        case ButtonState.Iguana:
                            obj = ObjectPool.GetObject("Iguana");
                            break;
                        case ButtonState.Rabbit:
                            obj = ObjectPool.GetObject("Rabbit");
                            break;
                        case ButtonState.Squirrel:
                            obj = ObjectPool.GetObject("Squirrel");
                            break;
                        case ButtonState.Tiger:
                            obj = ObjectPool.GetObject("Tiger");
                            break;
                        default:
                            break;
                    }
                    if (obj != null) {
                        obj.transform.position = hit.point;
                        obj.transform.rotation = Quaternion.identity;
                        breed(obj.tag);
                    }
                }
            }
        }
    }
    void FixedUpdate() {
        /* Create grass */
        grassTimer += Time.deltaTime;
        if (grassTimer > grassCoolTime) {
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
            // Debug.Log("grass generated");

            grassCount += 1;
            setGrassProgress(true);
            ObjectPool.GetObject("Grass");
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
        grassCount = (grassCount < 0) ? 0 : grassCount;
        grassProgressText.text = "Grass:" + grassCount.ToString();
        if (isIncrease) {
            grassProgressImage.fillAmount += 1.0f / grassMax;
        } else {
            grassProgressImage.fillAmount -= 1.0f / grassMax;
        }

    }

    void setSquirrelProgress(bool isIncrease) {
        squirrelCount = (squirrelCount < 0) ? 0 : squirrelCount;
        squirrelProgressText.text = "Squirrel: " + squirrelCount.ToString();
        if (isIncrease) {
            squirrelProgressImage.fillAmount += 1.0f / squirrelMax;
        } else {
            squirrelProgressImage.fillAmount -= 1.0f / squirrelMax;
        }
    }

    void setRabbitProgress(bool isIncrease) {
        rabbitCount = (rabbitCount < 0) ? 0 : rabbitCount;
        rabbitProgressText.text = "Rabbit: " + rabbitCount.ToString();
        if (isIncrease) {
            rabbitProgressImage.fillAmount += 1.0f / rabbitMax;
        } else {
            rabbitProgressImage.fillAmount -= 1.0f / rabbitMax;
        }
    }

    void setButterflyProgress(bool isIncrease) {
        butterflyCount = (butterflyCount < 0) ? 0 : butterflyCount;
        butterflyProgressText.text = "Butterfly: " + butterflyCount.ToString();
        if (isIncrease) {
            butterflyProgressImage.fillAmount += 1.0f / butterflyMax;
        } else {
            butterflyProgressImage.fillAmount -= 1.0f / butterflyMax;
        }
    }

    void setEagleProgress(bool isIncrease) {
        grassCount = (grassCount < 0) ? 0 : grassCount;
        eagleProgressText.text = "Eagle: " + eagleCount.ToString();
        if (isIncrease) {
            eagleProgressImage.fillAmount += 1.0f / eagleMax;
        } else {
            eagleProgressImage.fillAmount -= 1.0f / eagleMax;
        }
    }

    void setDeerProgress(bool isIncrease) {
        deerCount = (deerCount < 0) ? 0 : deerCount;
        deerProgressText.text = "Deer: " + deerCount.ToString();
        if (isIncrease) {
            deerProgressImage.fillAmount += 1.0f / deerMax;
        } else {
            deerProgressImage.fillAmount -= 1.0f / deerMax;
        }
    }

    void setTigerProgress(bool isIncrease) {
        tigerCount = (tigerCount < 0) ? 0 : tigerCount;
        tigerProgressText.text = "Tiger: " + tigerCount.ToString();
        if (isIncrease) {
            tigerProgressImage.fillAmount += 1.0f / tigerMax;
        } else {
            tigerProgressImage.fillAmount -= 1.0f / tigerMax;
        }
    }

    void setIguanaProgress(bool isIncrease) {
        iguanaCount = (iguanaCount < 0) ? 0 : iguanaCount;
        iguanaProgressText.text = "Iguana: " + iguanaCount.ToString();
        if (isIncrease) {
            iguanaProgressImage.fillAmount += 1.0f / iguanaMax;
        } else {
            iguanaProgressImage.fillAmount -= 1.0f / iguanaMax;
        }
    }

    void setBirdProgress(bool isIncrease) {
        birdCount = (birdCount < 0) ? 0 : birdCount;
        birdProgressText.text = "Bird: " + birdCount.ToString();
        if (isIncrease) {
            birdProgressImage.fillAmount += 1.0f / birdMax;
        } else {
            birdProgressImage.fillAmount -= 1.0f / birdMax;
        }
    }

    void setFrogProgress(bool isIncrease) {
        frogCount = (frogCount < 0) ? 0 : frogCount;
        frogProgressText.text = "Frog: " + frogCount.ToString();
        if (isIncrease) {
            frogProgressImage.fillAmount += 1.0f / frogMax;
        } else {
            frogProgressImage.fillAmount -= 1.0f / frogMax;
        }
    }

    public void breed(string tag) {
        if (tag == "Rabbit") {
            rabbitCount += 1;
            setRabbitProgress(true);
        } else if (tag == "Eagle") {
            eagleCount += 1;
            setEagleProgress(true);
        } else if (tag == "Deer") {
            deerCount += 1;
            setDeerProgress(true);
        } else if (tag == "Squirrel") {
            squirrelCount += 1;
            setSquirrelProgress(true);
        } else if (tag == "Tiger") {
            tigerCount += 1;
            setTigerProgress(true);
        } else if (tag == "Iguana") {
            iguanaCount += 1;
            setIguanaProgress(true);
        } else if (tag == "Bird") {
            birdCount += 1;
            setBirdProgress(true);
        } else if (tag == "Frog") {
            frogCount += 1;
            setFrogProgress(true);
        } else if (tag == "Butterfly") {
            butterflyCount += 1;
            setButterflyProgress(true);
        }
    }

    public void delete(GameObject item, string tag) {
        if (item == null)
            return;
        if (tag == "Grass") {
            grassCount -= 1;
            setGrassProgress(false);
        } else if (tag == "Rabbit") {
            rabbitCount -= 1;
            setRabbitProgress(false);

            if (rabbitCount == 0) {
                extinctCount += 1;
                Debug.Log(extinctCount);
                aliveCount -= 1;
            }
        } else if (tag == "Eagle") {
            eagleCount -= 1;
            setEagleProgress(false);

            if (eagleCount == 0) {
                extinctCount += 1;
                Debug.Log(extinctCount);
                aliveCount -= 1;
            }
        } else if (tag == "Deer") {
            deerCount -= 1;
            setDeerProgress(false);

            if (deerCount == 0) {
                extinctCount += 1;
                Debug.Log(extinctCount);
                aliveCount -= 1;
            }
        } else if (tag == "Squirrel") {
            squirrelCount -= 1;
            setSquirrelProgress(false);

            if (squirrelCount == 0) {
                extinctCount += 1;
                Debug.Log(extinctCount);
                aliveCount -= 1;
            }
        } else if (tag == "Tiger") {
            tigerCount -= 1;
            setTigerProgress(false);

            if (tigerCount == 0) {
                extinctCount += 1;
                Debug.Log(extinctCount);
                aliveCount -= 1;
            }
        } else if (tag == "Iguana") {
            iguanaCount -= 1;
            setIguanaProgress(false);

            if (iguanaCount == 0) {
                extinctCount += 1;
                Debug.Log(extinctCount);
                aliveCount -= 1;
            }
        } else if (tag == "Bird") {
            birdCount -= 1;
            setBirdProgress(false);

            if (birdCount == 0) {
                extinctCount += 1;
                Debug.Log(extinctCount);
                aliveCount -= 1;
            }
        } else if (tag == "Frog") {
            frogCount -= 1;
            setFrogProgress(false);

            if (frogCount == 0) {
                extinctCount += 1;
                Debug.Log(extinctCount);
                aliveCount -= 1;
            }
        } else if (tag == "Butterfly") {
            butterflyCount -= 1;
            setButterflyProgress(false);

            if (butterflyCount == 0) {
                extinctCount += 1;
                Debug.Log(extinctCount);
                aliveCount -= 1;
            }
        }
        ObjectPool.ReturnObject(item, item.tag);

        if (!isHuntMode && aliveCount == 0) {
            timerText.transform.position = new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0);
            Time.timeScale = 0;
        } else if (isHuntMode && extinctCount == 3) {
            HuntOverlayPanel.SetActive(false);
            GameOverPanel.SetActive(true);

            GameOverKills.text = "Kills:\t" + HuntCount;
            GameOverTime.text = "Time:\t" + ((float)Mathf.Round(gameTimer * 100f) / 100f).ToString() + "s";
            Time.timeScale = 0;
        }
    }

    IEnumerator FlashLightning() {
        foreach (var lightning in lightnings) {
            lightning.Play();
        }
        yield return new WaitForSeconds(2);
        lightningOn = true;
        yield return new WaitForSeconds(2);
        foreach (var lightning in lightnings) {
            lightning.Stop();
        }
        lightningOn = false;
    }

    IEnumerator IntermittentLightning() {
        while (true) {
            yield return new WaitForSeconds(lightningCooltime);
            StartCoroutine(FlashLightning());
        }
    }

    void UpdateAndDisplayTime() {
        gameTimer += Time.deltaTime * Time.timeScale;
        timerText.text = "Time:\t" + ((float)Mathf.Round(gameTimer * 100f) / 100f).ToString() + "s";
    }
}
