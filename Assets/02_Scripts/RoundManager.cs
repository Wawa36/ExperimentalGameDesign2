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
        bool isLightOut;
        float time;

        [SerializeField] GameObject[] prefabs;
        public Color wallColor;


        // Start is called before the first frame update
        void Start()
        {
            player = GameObject.FindGameObjectWithTag("Player");
            objectManager = GameObject.FindGameObjectWithTag("ObjectManager").GetComponent<ObjectManager>();
            isLightOut = false;
            time = 0;
        }

        // Update is called once per frame
        void Update()
        {
            if (isLightOut)
            {
                time += Time.deltaTime;
                if (time > lightOutTime)
                {
                    isLightOut = false;
                    player.GetComponent<Light2D>().enabled = true;
                }
            }
        }

        public void AllSeen()
        {
            roundCount++;
            isLightOut = true;
            player.GetComponent<Light2D>().enabled = false;
            FindObjectOfType<PlayerFreezing>().ActivateNewFreezeShot();
            foreach(ObjectPlayerLine oPL in ObjectManager.Instance.objects)
            {
                oPL.GetComponentInChildren<SpriteRenderer>().color = ObjectManager.Instance.ColorNormal;
                if (oPL.GetComponent<ObjectFreezeBehaviour>().isCoroutineRunning)
                {
                StopCoroutine(oPL.GetComponent<ObjectFreezeBehaviour>().runningFreezeCoroutine);

                }
            }
            time = 0;
            MakeOneObjectToWall();
            StartCoroutine(AddObject());
            ChangeObjectPlace();
        }



        IEnumerator AddObject()
        {
            yield return new WaitForEndOfFrame();
            if(objectManager.objectList.Count < maxObjects)
            { 
                
              /*  int count = 0;
                int nbrOfColliders = int.MaxValue;
                GameObject newObject = null;

                while (nbrOfColliders > 0 && count < 100) //max 100 tries
                {
                    Destroy(newObject);

                    Vector3 position = new Vector3(Random.Range(-16, 16), Random.Range(-8, 8), -0.8f);
                    newObject = Instantiate(prefabs[Random.Range(0, prefabs.Length)], position, Quaternion.Euler(-90, 0, 0));

                    Collider[] colliders = Physics.OverlapBox(position, newObject.GetComponent<Collider>().bounds.extents);
                    nbrOfColliders = colliders.Length;

                    count++;
                }*/

                Vector3 position = new Vector3(Random.Range(-16, 16), Random.Range(-8, 8), -0.8f);
                GameObject newObject = Instantiate(prefabs[Random.Range(0, prefabs.Length)], position, Quaternion.Euler(-90, 0, 0));
            }
            objectManager.UpdateObjects();
        }

        void MakeOneObjectToWall()
        {
            GameObject newWall = objectManager.objectList[Random.Range(0, objectManager.objectList.Count)].gameObject;
            newWall.transform.GetChild(0).GetComponent<SpriteRenderer>().color = wallColor;
            Destroy(newWall.GetComponent<ObjectFreezeBehaviour>());
            Destroy(newWall.GetComponent<ObjectPlayerLine>());
            Destroy(newWall.GetComponent<ObjectBehaviour>());
            Destroy(newWall.GetComponent<Rigidbody>());
            ObjectManager.Instance.objects = FindObjectsOfType<ObjectPlayerLine>();

            newWall.tag = "Untagged";

            objectManager.UpdateObjects();
        }

        void ChangeObjectPlace()
        {
           foreach(GameObject gameObject in objectManager.objectList)
           {
                int count = 0;
                int nbrOfColliders = int.MaxValue;

                while(nbrOfColliders > 0 && count < 100) //max 100 tries
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
