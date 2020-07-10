using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ObjectManager : MonoBehaviour
{
    public float objectTeleportFrequence = 10f;

    public ObjectPlayerLine[] objects;
    public List<GameObject> objectList = new List<GameObject>();
    public List<ObjectPlayerLine> hiddenObjects;
    bool allAreVisible;
    PlayerMovement player;

    public GameEvent winningCondition;
    float timer = 0;


    // Start is called before the first frame update
    void Start()
    {
        objects = Object.FindObjectsOfType<ObjectPlayerLine>();
        player = GameObject.FindObjectOfType<PlayerMovement>();
        Physics.autoSyncTransforms = true;
        UpdateObjects();
    }



    // Update is called once per frame
    void Update()
    {
        allAreVisible = true;
        foreach(ObjectPlayerLine obj in objects){
            if(obj != null)
            {
                if (!(obj.isTotallyVisible && !obj.isIntersected))
                {
                    allAreVisible = false;
                }
                obj.isIntersected = false;
            }

        }

        if (allAreVisible)
        {
            Debug.Log("WON");
            winningCondition.Raise();
        }

        foreach(ObjectPlayerLine obj in objects)
        {
            if(obj != null)
            {
                if (obj.GetComponent<ObjectPlayerLine>() != null)
                {
                    obj.CheckForVisibility();
                }
            }

          
        }

        //FindHiddenObjects();
        //ManageTeleport();
    }

    public void UpdateObjects()
    {
        objects = null;
        objects = Object.FindObjectsOfType<ObjectPlayerLine>();

        objectList.Clear();
        objectList.AddRange(GameObject.FindGameObjectsWithTag("Object"));
    }

    void FindHiddenObjects()
    {
        hiddenObjects = new List<ObjectPlayerLine>();
        foreach(ObjectPlayerLine obj in objects)
        {
            if (obj.isTotallyHidden)
                hiddenObjects.Add(obj);
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
            //List<ObjectPlayerLine> shuffledList = hiddenObjects.OrderBy(x => Random.value).ToList(); // TODO: check if this really works
            //hiddenObjects = shuffledList;

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
                ObjectPlayerLine currentObjectThatHides;

                // 2. Check for each object if the hiddenObject can hide behind one, until one is found
                foreach (ObjectPlayerLine object2HideBehind in objects)
                {
                    if (object2HideBehind == hiddenObject)
                        continue;

                    if (foundNewObjectToHide)
                        break;

                    print("Objekt, hinter dem sich " + hiddenObject + " verstecken könnte: " + object2HideBehind);

                    // (Check on 3 lines (behind objToHideBehind): player-to-objToHideBehind, downwards from that, upwards from that)
                    // Check in steps (radius of objToHide)
                    // (1) Check for collision with other objects
                    // (2) Check if not outside gameview
                    // (3) Check if obj is acutally totally hidden

                    Vector3 mainLine = object2HideBehind.transform.position - player.transform.position;
                    //Vector3 downLine;
                    //Vector3 upLine;
                    int counter = 1;
                    Vector3 position2Check;// = new Vector3();

                    do
                    {
                        counter++;
                        position2Check = hiddenObj_originalPosition + mainLine.normalized * hiddenObj_radius * counter;
                        hiddenObj_rigid.position = position2Check; // TODO: check if position of the rigid and collider get updated instantly, for the following check

                        // (1) check if collision with other objects
                        Collider[] colliders = Physics.OverlapBox(position2Check, hiddenObj_collider.bounds.extents);
                        if (colliders.Length == 0)
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
                                    break;
                                }
                            }
                        }

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
        return
            Camera.main.WorldToViewportPoint(position).x - radius > 0 &&
            Camera.main.WorldToViewportPoint(position).x + radius < 1 &&
            Camera.main.WorldToViewportPoint(position).y - radius > 0 &&
            Camera.main.WorldToViewportPoint(position).x - radius < 1;
    }
}
