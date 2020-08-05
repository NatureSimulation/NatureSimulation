using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public static ObjectPool instance;
    public GameObject grassPrefab;
    public GameObject birdPrefab;
    public GameObject butterflyPrefab;
    public GameObject eaglePrefab;
    public GameObject iguanaPrefab;
    public GameObject rabbitPrefab;
    public GameObject deerPrefab;
    public GameObject squirrelPrefab;
    public GameObject tigerPrefab;
    public GameObject frogPrefab;

    Queue<GameObject> grassQueue = new Queue<GameObject>();
    Queue<GameObject> birdQueue = new Queue<GameObject>();
    Queue<GameObject> butterflyQueue = new Queue<GameObject>();
    Queue<GameObject> eagleQueue = new Queue<GameObject>();
    Queue<GameObject> iguanaQueue = new Queue<GameObject>();
    Queue<GameObject> rabbitQueue = new Queue<GameObject>();
    Queue<GameObject> deerQueue = new Queue<GameObject>();
    Queue<GameObject> squirrelQueue = new Queue<GameObject>();
    Queue<GameObject> tigerQueue = new Queue<GameObject>();
    Queue<GameObject> frogQueue = new Queue<GameObject>();




    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        Initialize(100);
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void Initialize(int initCount) {
        for (int i = 0; i < initCount * 5; ++i) {
            butterflyQueue.Enqueue(CreateNewObject("Butterfly"));
            rabbitQueue.Enqueue(CreateNewObject("Rabbit"));
            deerQueue.Enqueue(CreateNewObject("Deer"));
            squirrelQueue.Enqueue(CreateNewObject("Squirrel"));
            frogQueue.Enqueue(CreateNewObject("Frog"));
            grassQueue.Enqueue(CreateNewObject("Grass"));
        }
        for (int i = 0; i < initCount; ++i) {
            birdQueue.Enqueue(CreateNewObject("Bird"));
            eagleQueue.Enqueue(CreateNewObject("Eagle"));
            iguanaQueue.Enqueue(CreateNewObject("Iguana"));
            tigerQueue.Enqueue(CreateNewObject("Tiger"));
        }


    }

    private GameObject CreateNewObject(string tag) {
        GameObject obj;
        if (tag == "Bird") {
            obj = Instantiate(birdPrefab);
        } else if (tag == "Butterfly") {
            obj = Instantiate(butterflyPrefab);
        } else if (tag == "Eagle") {
            obj = Instantiate(eaglePrefab);
        } else if (tag == "Iguana") {
            obj = Instantiate(iguanaPrefab);
        } else if (tag == "Rabbit") {
            obj = Instantiate(rabbitPrefab);
        } else if (tag == "Deer") {
            obj = Instantiate(deerPrefab);
        } else if (tag == "Squirrel") {
            obj = Instantiate(squirrelPrefab);
        } else if (tag == "Tiger") {
            obj = Instantiate(tigerPrefab);
        } else if (tag == "Frog") {
            obj = Instantiate(frogPrefab);
        } else if (tag == "Grass") {
            obj = Instantiate(grassPrefab);
        } else {
            Debug.Assert(false, "Invalid tag: " + tag);
            return null;
        }

        obj.SetActive(false);
        obj.transform.SetParent(instance.transform);
        return obj;
    }

    public static GameObject GetObject(string tag) {
        GameObject obj;
        if (tag == "Bird") {
            obj = (instance.birdQueue.Count > 0) ? instance.birdQueue.Dequeue() : instance.CreateNewObject(tag);
        } else if (tag == "Butterfly") {
            obj = (instance.butterflyQueue.Count > 0) ? instance.butterflyQueue.Dequeue() : instance.CreateNewObject(tag);
        } else if (tag == "Eagle") {
            obj = (instance.eagleQueue.Count > 0) ? instance.eagleQueue.Dequeue() : instance.CreateNewObject(tag);
        } else if (tag == "Iguana") {
            obj = (instance.iguanaQueue.Count > 0) ? instance.iguanaQueue.Dequeue() : instance.CreateNewObject(tag);
        } else if (tag == "Rabbit") {
            obj = (instance.rabbitQueue.Count > 0) ? instance.rabbitQueue.Dequeue() : instance.CreateNewObject(tag);
        } else if (tag == "Deer") {
            obj = (instance.deerQueue.Count > 0) ? instance.deerQueue.Dequeue() : instance.CreateNewObject(tag);
        } else if (tag == "Squirrel") {
            obj = (instance.squirrelQueue.Count > 0) ? instance.squirrelQueue.Dequeue() : instance.CreateNewObject(tag);
        } else if (tag == "Tiger") {
            obj = (instance.tigerQueue.Count > 0) ? instance.tigerQueue.Dequeue() : instance.CreateNewObject(tag);
        } else if (tag == "Frog") {
            obj = (instance.frogQueue.Count > 0) ? instance.frogQueue.Dequeue() : instance.CreateNewObject(tag);
        } else if (tag == "Grass") {
            obj = (instance.grassQueue.Count > 0) ? instance.grassQueue.Dequeue() : instance.CreateNewObject(tag);
        } else {
            Debug.Assert(false, "Invalid tag: " + tag);
            return null;
        }

        obj.transform.SetParent(null);
        obj.SetActive(true);
        return obj;
    }

    public static void ReturnObject(GameObject obj, string tag) {
        obj.SetActive(false);
        obj.transform.SetParent(instance.transform);
        if (tag == "Bird") {
            instance.birdQueue.Enqueue(obj);
        } else if (tag == "Butterfly") {
            instance.butterflyQueue.Enqueue(obj);
        } else if (tag == "Eagle") {
            instance.eagleQueue.Enqueue(obj);
        } else if (tag == "Iguana") {
            instance.iguanaQueue.Enqueue(obj);
        } else if (tag == "Rabbit") {
            instance.rabbitQueue.Enqueue(obj);
        } else if (tag == "Deer") {
            instance.deerQueue.Enqueue(obj);
        } else if (tag == "Squirrel") {
            instance.squirrelQueue.Enqueue(obj);
        } else if (tag == "Tiger") {
            instance.tigerQueue.Enqueue(obj);
        } else if (tag == "Frog") {
            instance.frogQueue.Enqueue(obj);
        } else if (tag == "Grass") {
            instance.grassQueue.Enqueue(obj);
        } else {
            Debug.Assert(false, "Invalid tag: " + tag);
        }
    }
}
