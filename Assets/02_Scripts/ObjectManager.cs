using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectManager : MonoBehaviour
{
    ObjectPlayerLine[] objects;

    // Start is called before the first frame update
    void Start()
    {
        objects = Object.FindObjectsOfType<ObjectPlayerLine>();
    }

    // Update is called once per frame
    void Update()
    {
        foreach(ObjectPlayerLine obj in objects){
            obj.isIntersected = false;
        }
        foreach(ObjectPlayerLine obj in objects)
        {
            obj.CheckForVisibility();
        }
    }
}
