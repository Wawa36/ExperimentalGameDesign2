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
            time = 0;
            MakeOneObjectToWall();
            AddObject();
            ChangeObjectPlace();
        }

        void AddObject()
        {
            if(ObjectManager.Instance.objectList.Count < maxObjects)
            {
                Vector3 position = new Vector3(Random.Range(-16, 16), Random.Range(-8, 8), -0.8f);
                Instantiate(prefabs[Random.Range(0, prefabs.Length)], position, Quaternion.Euler(-90, 0, 0));
            }
            objectManager.UpdateObjects();
        }

        void MakeOneObjectToWall()
        {
            GameObject newWall = objectManager.objectList[Random.Range(0, objectManager.objectList.Count)].gameObject;
            newWall.transform.GetChild(0).GetComponent<SpriteRenderer>().color = wallColor;
            Destroy(newWall.GetComponent<ObjectPlayerLine>());
            Destroy(newWall.GetComponent<ObjectBehaviour>());
            Destroy(newWall.GetComponent<Rigidbody>());
            newWall.tag = "Untagged";

            objectManager.UpdateObjects();
        }

        void ChangeObjectPlace()
        {
           foreach(GameObject gameObject in objectManager.objectList)
           {
                Vector3 position = new Vector3(Random.Range(-16, 16), Random.Range(-8, 8), -0.8f);
                gameObject.transform.position = position;
            }
        }
    }
}
