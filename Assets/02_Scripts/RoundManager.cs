using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEngine.Experimental.Rendering.Universal
{
    public class RoundManager : MonoBehaviour
    {

        public int roundCount;
        public int maxObjects;
        GameObject player;
        ObjectManager objectManager;

        [SerializeField] float lightOutTime;
        [SerializeField] float orangeTime;
        bool isOrange;
        bool isLightOut;
        float time;

        [SerializeField] GameObject[] prefabs;
        public Color wallColor;

        float playerLightOuterRadius;
        float playerSpeed;

        // Start is called before the first frame update
        void Start()
        {
            player = GameObject.FindGameObjectWithTag("Player");
            objectManager = GameObject.FindGameObjectWithTag("ObjectManager").GetComponent<ObjectManager>();
            isLightOut = false;
            time = 0;

            playerLightOuterRadius = player.GetComponent<Light2D>().pointLightOuterRadius;
            playerSpeed = player.GetComponent<PlayerMovement>().speed;
        }

        // Update is called once per frame
        void Update()
        {
            if(isOrange)
            {
                time += Time.deltaTime;
                if (time > orangeTime)
                {
                    isOrange = false;
                    isLightOut = true;

                    foreach (GameObject gameObject in objectManager.objectList)
                    {
                        gameObject.GetComponentInChildren<SpriteRenderer>().color = objectManager.ColorNormal;
                        gameObject.GetComponentInChildren<SpriteRenderer>().material = objectManager.lit;
                    }

                    foreach (GameObject gameObject in objectManager.wallList)
                    {
                        gameObject.GetComponentInChildren<SpriteRenderer>().material = objectManager.unlit;
                    }

                    player.GetComponent<PlayerMovement>().speed = 0;


                    player.GetComponent<Light2D>().pointLightOuterRadius = 4;

                    //FindObjectOfType<PlayerFreezing>().ActivateNewFreezeShot();
                    foreach (ObjectPlayerLine oPL in ObjectManager.Instance.objects)
                    {
                        if (oPL != null)
                        {
                            oPL.GetComponentInChildren<SpriteRenderer>().color = ObjectManager.Instance.ColorNormal;
                            if (oPL.GetComponent<ObjectFreezeBehaviour>().isCoroutineRunning)
                            {
                                StopCoroutine(oPL.GetComponent<ObjectFreezeBehaviour>().runningFreezeCoroutine);
                            }
                        }
                    }

                    if (Random.value > 0.5)
                    {
                        MakeOneObjectToWall();
                    }

                    AddObject();
                    ChangeObjectPlace();


                    time = 0;
                }
            }

            if (isLightOut)
            {
                time += Time.deltaTime;
                if (time > lightOutTime)
                {
                    isLightOut = false;
                    //player.GetComponent<Light2D>().enabled = true;
                    player.GetComponent<Light2D>().pointLightOuterRadius = playerLightOuterRadius;

                    player.GetComponent<PlayerMovement>().speed = playerSpeed;

                    foreach (GameObject gameObject in objectManager.wallList)
                    {
                        gameObject.GetComponentInChildren<SpriteRenderer>().material = objectManager.lit;
                    }
                }
            }
        }

        public void AllSeen()
        {
            if(!isOrange) //only if not already orange
            {
                roundCount++;

                foreach (GameObject gameObject in objectManager.objectList)
                {
                    gameObject.GetComponentInChildren<SpriteRenderer>().color = objectManager.ColorWinning;
                    gameObject.GetComponentInChildren<SpriteRenderer>().material = objectManager.unlit;
                }


                isOrange = true;
                time = 0;
            }
        }


        void MakeOneObjectToWall()
        {
            GameObject newWall = objectManager.objectList[Random.Range(0, objectManager.objectList.Count)].gameObject;
            newWall.transform.GetChild(0).GetComponent<SpriteRenderer>().color = wallColor;
            Destroy(newWall.GetComponent<ObjectFreezeBehaviour>());
            Destroy(newWall.GetComponent<ObjectPlayerLine>());
            Destroy(newWall.GetComponent<ObjectBehaviour>());
            Destroy(newWall.GetComponent<Rigidbody>());

            newWall.tag = "Wall";

            objectManager.UpdateObjects();
        }

        void AddObject()
        {

            if(objectManager.objectList.Count < maxObjects)
            { 
                Vector3 position = new Vector3(Random.Range(-16, 16), Random.Range(-8, 8), -0.8f);
                GameObject newObject = Instantiate(prefabs[Random.Range(0, prefabs.Length)], position, Quaternion.Euler(-90, 0, 0));
                newObject.transform.localScale = newObject.transform.localScale * Random.Range(1f, 3f);
            }
            objectManager.UpdateObjects();
        }

        void ChangeObjectPlace()
        {
           foreach(GameObject gameObject in objectManager.objectList)
           {
                int count = 0;
                int nbrOfColliders = int.MaxValue;

                while(nbrOfColliders > 1 && count < 100) //max 100 tries
                {
                    Vector3 position = new Vector3(Random.Range(-16, 16), Random.Range(-8, 8), -0.8f);
                    gameObject.transform.position = position;

                    Collider[] colliders = Physics.OverlapBox(position, gameObject.GetComponent<Collider>().bounds.extents);
                    nbrOfColliders = colliders.Length;

                    count++;
                }

            }
        }

    }
}
