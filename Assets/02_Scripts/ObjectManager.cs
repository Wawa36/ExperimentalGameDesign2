using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal; //2019 VERSIONS


public class ObjectManager : MonoBehaviour
{
    #region SINGLETON
    private static ObjectManager instance = null;

    // Game Instance Singleton
    public static ObjectManager Instance
    {
        get
        {
            return instance;
        }
    }

    private void Awake()
    {
        // if the singleton hasn't been initialized yet
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }

        instance = this;
        DontDestroyOnLoad(this.gameObject);
    }
    #endregion

    ObjectPlayerLine[] objects;
    bool allAreVisible;

    public GameEvent winningCondition;

    public ListQueue<Coroutine> freezeCoroutines;
    // I need this because I am to lazy to find a better solution
    public ListQueue<GameObject> frozenObjects;

    public float freezeCountdown;
    public int maxFreezeNumber;

    [Header("COLORS")] 
    public Color ColorNormal;
    public Color ColorFrozen;
    public Color ColorWinning;

    // Start is called before the first frame update
    void Start()
    {
        objects = Object.FindObjectsOfType<ObjectPlayerLine>();
        freezeCoroutines = new ListQueue<Coroutine>();
        frozenObjects = new ListQueue<GameObject>();
    }



    // Update is called once per frame
    void Update()
    {
        allAreVisible = true;
        foreach(ObjectPlayerLine obj in objects){
            if(!(obj.isTotallyVisible && !obj.isIntersected))
            {
                allAreVisible = false;
            }
            obj.isIntersected = false;
        }

        if (allAreVisible)
        {
            foreach(ObjectPlayerLine obj in objects)
            {
                obj.GetComponentInChildren<Light2D>().enabled = true;
                obj.GetComponentInChildren<SpriteRenderer>().color = ColorWinning;
                obj.GetComponent<ObjectBehaviour>().enabled = false;
            }
            winningCondition.Raise();
        }

        foreach(ObjectPlayerLine obj in objects)
        {
            obj.CheckForVisibility();
        }
    }

    public void FreezeObject(GameObject obj)
    {
        ObjectFreezeBehaviour freeze = obj.GetComponent<ObjectFreezeBehaviour>();
        if (obj.GetComponent<ObjectFreezeBehaviour>().isCoroutineRunning)
        {
            freeze.StopFreezeCoroutineRegular();
            StopCoroutine(freeze.runningFreezeCoroutine);
        }
        if (ObjectManager.Instance.freezeCoroutines.Count >= ObjectManager.Instance.maxFreezeNumber)
        {
            StopCoroutine(ObjectManager.Instance.freezeCoroutines.Dequeue());
            frozenObjects.Peek().GetComponent<ObjectBehaviour>().enabled = true;
            frozenObjects.Peek().GetComponentInChildren<SpriteRenderer>().color = ObjectManager.Instance.ColorNormal;
            //FindObjectOfType<PlayerFreezing>().RetrieveFreeze();
            frozenObjects.Dequeue();
        }

        freeze.runningFreezeCoroutine = StartCoroutine(freeze.Freeze(obj));

        ObjectManager.Instance.freezeCoroutines.Enqueue(freeze.runningFreezeCoroutine);
        ObjectManager.Instance.frozenObjects.Enqueue(obj);
    }

}
