using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPlayerLine : MonoBehaviour
{
    Mesh mesh;
    Vector3[] vertices;

    Transform player;

    public bool isTotallyHidden;
    public bool isTotallyVisible;
    public bool isIntersected;

    // Start is called before the first frame update
    void Start()
    {
        mesh = GetComponent<MeshFilter>().mesh;
        vertices = mesh.vertices;

        player = FindObjectOfType<PlayerMovement>().transform;
    }

    void Update()
    {
        UpdateViewMode();
    }


    Vector3 startPosition;
    Vector3 direction;

    public void CheckForVisibility()
    {
        isTotallyHidden = true;
        isTotallyVisible = true;

        foreach (Vector3 vertex in vertices)
        {
            RaycastHit hitPoint;
            startPosition = transform.TransformPoint(vertex);
            direction = player.position - startPosition;
            gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
            if (Physics.Raycast(startPosition, direction, out hitPoint, Mathf.Infinity))
            {

                if (hitPoint.collider.tag == "Player")
                {
                    //Debug.DrawRay(startPosition, direction, Color.green);
                    CastIntersectingRay();
                    isTotallyHidden = false;
                }
                else if(hitPoint.collider.gameObject != transform.gameObject)
                {
                    //Debug.DrawRay(startPosition, direction, Color.red);

                    isTotallyVisible = false;
                    Debug.Log("Did Hit something else");
                }
            }
            gameObject.layer = LayerMask.NameToLayer("Default");

        }
        //if (isTotallyVisible)
        //{
        //    Debug.Log("Is Totally Visible" + name);
        //}
        //if (isTotallyHidden)
        //{
        //    Debug.Log("Is Totally Hidden" + name);
        //}
        //if (isIntersected)
        //{
        //    Debug.Log("Is partially hidden" + transform.name);
        //}
    }


    void CastIntersectingRay()
    {
        RaycastHit hitPoint;
        //Debug.DrawRay(startPosition, -direction, Color.blue);
        if (Physics.Raycast(startPosition, -direction, out hitPoint, Mathf.Infinity))
        {
            if(hitPoint.collider.tag == "Object")
            {
                hitPoint.collider.GetComponent<ObjectPlayerLine>().isIntersected = true;
            }
        }
    }

    void UpdateViewMode()
    {
        if (isTotallyVisible && !isIntersected)
        {
            GetComponent<MeshRenderer>().material.color = Color.white;
        } else if (isTotallyHidden)
        {
            GetComponent<MeshRenderer>().material.color = Color.black;
        }
        else
        {
            GetComponent<MeshRenderer>().material.color = Color.red;
        }
    }
}
