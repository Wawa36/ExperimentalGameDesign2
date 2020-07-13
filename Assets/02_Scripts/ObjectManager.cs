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

    public float objectTeleportFrequence = 10f;

    //[HideInInspector]
    public ObjectPlayerLine[] objects;
    //[HideInInspector]
    public List<GameObject> objectList = new List<GameObject>();
    List<ObjectPlayerLine> objects_scripts;
    //[HideInInspector]
    public List<ObjectPlayerLine> hiddenObjects;
    bool allAreVisible;
    PlayerMovement player;

    public GameEvent winningCondition;
    public float timer = 0;

    public ListQueue<Coroutine> freezeCoroutines;
    // I need this because I am to lazy to find a better solution
    public ListQueue<GameObject> frozenObjects;

    public float freezeCountdown;
    public int maxFreezeNumber;

    [Header("COLORS")] 
    public Color ColorNormal;
    public Color ColorFrozen;
    public Color ColorWinning;

    [Header("MATERIALS")]
    public Material lit; //lol das ist Julians Spiel! 
    public Material unlit; //und das ist das gegenteil von Julians Spiel!

    // Start is called before the first frame update
    void Start()
    {
        frozenObjects = new ListQueue<GameObject>();
        player = GameObject.FindObjectOfType<PlayerMovement>();
        Physics.autoSyncTransforms = true;
        UpdateObjects();
    }



    // Update is called once per frame
    void Update()
    {
        ConvertGameobjectList2Script();

        allAreVisible = true;
        foreach(GameObject obj in objectList){
            if(obj != null)
            {
                if (!(obj.GetComponent<ObjectPlayerLine>().isTotallyVisible && !obj.GetComponent<ObjectPlayerLine>().isIntersected))
                {
                    allAreVisible = false;
                }
                obj.GetComponent<ObjectPlayerLine>().isIntersected = false;
            }

        }

        if (allAreVisible)
        {
            foreach (GameObject obj in objectList)
            {
                obj.GetComponentInChildren<Light2D>().enabled = true;
                obj.GetComponentInChildren<SpriteRenderer>().color = ColorWinning;
                obj.GetComponent<ObjectBehaviour>().enabled = false;
            }
            winningCondition.Raise();
        }

        foreach (GameObject obj in objectList)
        {
            
            if(obj != null)
            {
                if (obj.GetComponent<ObjectPlayerLine>() != null)
                {
                    obj.GetComponent<ObjectPlayerLine>().CheckForVisibility();
                }
            }
        }

        FindHiddenObjects();
        ManageTeleport();
    }

    public void UpdateObjects()
    {
        objectList.Clear();
        objectList.AddRange(GameObject.FindGameObjectsWithTag("Object"));
    }

    void FindHiddenObjects()
    {
        hiddenObjects = new List<ObjectPlayerLine>();
        foreach(ObjectPlayerLine obj in objects_scripts)
        {
            if (obj.GetComponent<ObjectPlayerLine>().isTotallyHidden)
                hiddenObjects.Add(obj.GetComponent<ObjectPlayerLine>());
        }
    }

    void ManageTeleport()
    {
        // ULTRA-POWERFUL EVENT
        // if the timer is up, one hidden object will jump to another hidden spot

        timer += Time.deltaTime;
        if (timer >= objectTeleportFrequence)
        {
            // shuffle hiddenObjects
            List<ObjectPlayerLine> shuffledList = hiddenObjects.OrderBy(x => Random.value).ToList(); // TODO: check if this really works
            hiddenObjects = shuffledList;

            bool foundNewObjectToHide = false;

            // 1. Check each hiddenObject until one is found that can hide behind another
            foreach (ObjectPlayerLine hiddenObject in hiddenObjects)
            {
                if (foundNewObjectToHide)
                    break;

                Collider hiddenObj_collider = hiddenObject.GetComponent<Collider>();
                float hiddenObj_radius = hiddenObj_collider.bounds.extents.x * 1.1f;
                Rigidbody hiddenObj_rigid = hiddenObject.GetComponent<Rigidbody>();
                Vector3 hiddenObj_originalPosition = hiddenObject.transform.position;
                
                RaycastHit hitInfo;
                Physics.Raycast(hiddenObj_originalPosition, player.transform.position - hiddenObj_originalPosition, out hitInfo);
                ObjectPlayerLine currentlyHidingObj = hitInfo.collider.GetComponent<ObjectPlayerLine>();

                // 2. Check for each other object if the hiddenObject can hide behind one, until one is found
                foreach (ObjectPlayerLine object2HideBehind in objects_scripts)
                {
                    if (object2HideBehind == hiddenObject)
                        continue;

                    if (foundNewObjectToHide)
                        break;

                    if (object2HideBehind == currentlyHidingObj)
                        continue;

                    // (Check on 3 lines (behind objToHideBehind): player-to-objToHideBehind, downwards from that, upwards from that)
                    Vector3 mainLine = object2HideBehind.transform.position - player.transform.position;
                    //Vector3 downLine;
                    //Vector3 upLine;
                    int counter = 0;
                    Vector3 position2Check;

                    do
                    {
                        // Check in steps (radius of objToHide)
                        counter++;
                        position2Check = object2HideBehind.transform.position + mainLine.normalized * hiddenObj_radius * counter;
                        hiddenObj_rigid.position = position2Check; // TODO: check if position of the rigid and collider get updated instantly, for the following check

                        // (1) check if collision with other objects
                        Collider[] colliders = Physics.OverlapBox(position2Check, hiddenObj_collider.bounds.extents);
                        if (colliders.Length <= 1) // 1, weil OverlapBox mit dem eigenen Collider kollidiert; zu faul layermask zu erstellen
                        {
                            // (2) Check if totally hidden
                            hiddenObject.CheckForVisibility(); // TODO: check if this works correct in this frame
                            if (hiddenObject.isTotallyHidden)
                            {
                                // (3) Check if is within game view
                                if (ObjectIsWithinGameView(position2Check, hiddenObj_radius))
                                {
                                    // (4) Check if hiddenObject is not frozen
                                    if (!hiddenObject.GetComponent<ObjectFreezeBehaviour>().isCoroutineRunning)
                                    {
                                        print("TELEPORT");
                                        foundNewObjectToHide = true;
                                        timer = 0;
                                        break;
                                    }
                                }
                            }
                        }

                        if (!foundNewObjectToHide)
                            hiddenObj_rigid.position = hiddenObj_originalPosition;

                    }
                    // check if within game view
                    while (ObjectIsWithinGameView(position2Check, hiddenObj_radius)); // TODO: check if this really works

                }
            }
        }
    }

    bool ObjectIsWithinGameView(Vector3 position, float radius)
    {
        Vector3 xLeft = new Vector3(position.x - radius, position.y, position.z);
        Vector3 xRight = new Vector3(position.x + radius, position.y, position.z);
        Vector3 yUp = new Vector3(position.x, position.y - radius, position.z);
        Vector3 yDown = new Vector3(position.x, position.y + radius, position.z);
        return
            Camera.main.WorldToViewportPoint(xLeft).x > 0 &&
            Camera.main.WorldToViewportPoint(xRight).x < 1 &&
            Camera.main.WorldToViewportPoint(yUp).y > 0 &&
            Camera.main.WorldToViewportPoint(yDown).y < 1;
    }

    public void FreezeObject(GameObject obj)
    {
        ObjectFreezeBehaviour freeze = obj.GetComponent<ObjectFreezeBehaviour>();
        if (freeze != null)
        {
            if (freeze.isCoroutineRunning)
            {
                freeze.StopFreezeCoroutine();
            }

            if (frozenObjects.Count >= ObjectManager.Instance.maxFreezeNumber)
            {
                StopCoroutine(frozenObjects.Peek().GetComponent<ObjectFreezeBehaviour>().runningFreezeCoroutine);
                frozenObjects.Peek().GetComponent<ObjectBehaviour>().enabled = true;
                frozenObjects.Peek().GetComponentInChildren<SpriteRenderer>().color = ObjectManager.Instance.ColorNormal;
                frozenObjects.Dequeue();
            }

            player.GetComponent<PlayerFreezing>().ExpendFreeze();
            freeze.runningFreezeCoroutine = StartCoroutine(freeze.Freeze(obj));
            ObjectManager.Instance.frozenObjects.Enqueue(obj);
        }
        
    }

    void ConvertGameobjectList2Script()
    {
        objects_scripts = new List<ObjectPlayerLine>();
        foreach (GameObject obj in objectList)
            objects_scripts.Add(obj.GetComponent<ObjectPlayerLine>());
    }
}
