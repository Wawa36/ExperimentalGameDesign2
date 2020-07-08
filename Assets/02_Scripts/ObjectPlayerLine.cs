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

    // Start is called before the first frame update
    void Start()
    {
        mesh = GetComponent<MeshFilter>().mesh;
        vertices = mesh.vertices;

        player = FindObjectOfType<PlayerMovement>().transform;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        foreach(Vector3 vertex in vertices)
        {
            Gizmos.DrawWireSphere(transform.TransformPoint(vertex), .2f);
        }
    }

    // Update is called once per frame
    void Update()
    {
        CheckForVisibility();
    }


    void CheckForVisibility()
    {
        isTotallyHidden = true;
        isTotallyVisible = true;

        foreach (Vector3 vertex in vertices)
        {
            RaycastHit hitPoint;
            Vector3 startPosition = transform.TransformPoint(vertex);
            Vector3 direction = player.position - startPosition;

            Debug.DrawRay(startPosition, direction, Color.yellow);
            if (Physics.Raycast(startPosition, direction, out hitPoint, Mathf.Infinity))
            {
                
                if(hitPoint.collider.tag == "Player")
                {
                    Debug.Log("Did Hit Player");
                    isTotallyHidden = false;
                }
                else
                {
                    isTotallyVisible = false;
                    Debug.Log("Did Hit something else");
                }
            }
        }
        if (isTotallyVisible)
        {
            Debug.Log("Is Totally Visible");
        }
        if (isTotallyHidden)
        {
            Debug.Log("Is Totally Hidden");
        }
    }
}
