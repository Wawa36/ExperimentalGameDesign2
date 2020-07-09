using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectManager : MonoBehaviour
{
    ObjectPlayerLine[] objects;
    bool allAreVisible;

    public GameEvent winningCondition;


    // Start is called before the first frame update
    void Start()
    {
        objects = Object.FindObjectsOfType<ObjectPlayerLine>();
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
            Debug.Log("WON");
            winningCondition.Raise();
        }

        foreach(ObjectPlayerLine obj in objects)
        {
            obj.CheckForVisibility();
        }
    }
}
