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

    //public ObjectPlayerLine[] objects;
    public List<GameObject> objectList = new List<GameObject>();
    public List<ObjectPlayerLine> hiddenObjects;
    bool allAreVisible;
    PlayerMovement player;

    public GameEvent winningCondition;
    float timer = 0;

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
        frozenObjects = new ListQueue<GameObject>();
        player = GameObject.FindObjectOfType<PlayerMovement>();
        Physics.autoSyncTransforms = true;
        UpdateObjects();
    }



    // Update is called once per frame
    void Update()
    {
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
        foreach(GameObject obj in objectList)
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

                // 2. Check for each object if the hiddenObject can hide behind one, until one is found
                foreach (GameObject object2HideBehindGO in objectList)
                {
                    ObjectPlayerLine object2HideBehind = object2HideBehindGO.GetComponent<ObjectPlayerLine>();
                    if (object2HideBehind == hiddenObject)
                        continue;

                    if (foundNewObjectToHide)
                        break;

                    // TODO
                    // if object2HideBehind == currentObjectThatIHideBehinde
                    //  Break;
                    if (object2HideBehind == currentlyHidingObj)
                        continue;

                    Vector3 mainLine = object2HideBehind.transform.position - player.transform.position;
                    //Vector3 downLine;
                    //Vector3 upLine;
                    int counter = 0;
                    Vector3 position2Check;// = new Vector3();

                    do
                    {
                        // (Check on 3 lines (behind objToHideBehind): player-to-objToHideBehind, downwards from that, upwards from that)
                        // Check in steps (radius of objToHide)
                        counter++;
                        position2Check = object2HideBehind.transform.position + mainLine.normalized * hiddenObj_radius * counter;
                        hiddenObj_rigid.position = position2Check; // TODO: check if position of the rigid and collider get updated instantly, for the following check

                        // (1) check if collision with other objects
                        //hiddenObject.transform.
                        Collider[] colliders = Physics.OverlapBox(position2Check, hiddenObj_collider.bounds.extents);

                        //print("in viewport? " + ObjectIsWithinGameView(position2Check, hiddenObj_radius));
                        //print("counter: " + counter + ", Collision.count: " + colliders.Length);
                        if (colliders.Length <= 1)
                        {
                            // (2) Check if totally hidden
                            hiddenObject.CheckForVisibility(); // TODO: check if this works correct in this frame
                            if (hiddenObject.isTotallyHidden)
                            {
                                // (3) Check if is within game view
                                if (ObjectIsWithinGameView(position2Check, hiddenObj_radius))
                                {
                                    print("TELEPORT");
                                    foundNewObjectToHide = true;
                                    timer = 0;
                                    break;
                                }
                            }
                        }

                        Debug.DrawLine(Vector3.zero, hiddenObj_originalPosition, Color.blue, 3f);
                        Debug.DrawLine(object2HideBehind.transform.position, position2Check, Color.green, 3f);
                        //Debug.DrawLine()

                        if (!foundNewObjectToHide)
                            hiddenObj_rigid.position = hiddenObj_originalPosition;

                    }
                    // check if within game view
                    while (ObjectIsWithinGameView(position2Check, hiddenObj_radius)); // TODO: check if this really works

                }
                //ObjectPlayerLine newObjectToHide = 
                // if geklappt, dann timer = 0;
            }
        }
    }

    bool ObjectIsWithinGameView(Vector3 position, float radius)
    {
        //print("position.x (world): " + position.x + ", position.x (viewport): " + Camera.main.WorldToViewportPoint(position).x);
        //print("position.y (world): " + position.y + ", position.y (viewport): " + Camera.main.WorldToViewportPoint(position).y);
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
}
