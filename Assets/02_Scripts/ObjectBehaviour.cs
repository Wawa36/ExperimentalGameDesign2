using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ObjectBehaviour : MonoBehaviour
{
    Rigidbody rigidbody;
    ObjectPlayerLine objectPlayerLine;
    bool isMoving;
    bool isTotallyVisible { get { return objectPlayerLine.isTotallyVisible; } set {; } }
    bool isTotallyHidden { get { return objectPlayerLine.isTotallyHidden; } set {; } }
    SpriteRenderer spriteRenderer;

    [SerializeField] private float playerSpeedMultiplier;
    [SerializeField] private PlayerMovement player;

  
    void Start()
    {
        rigidbody = this.GetComponent<Rigidbody>();
        objectPlayerLine = this.GetComponent<ObjectPlayerLine>();
        isMoving = false;
        // TO DO: initialize spriteRenderer
        spriteRenderer = this.GetComponentInChildren<SpriteRenderer>();
    }

    private void Update()
    {
        SetMovement();

        SetVisibility();
    }

    void FixedUpdate()
    {
        Move();
    }

    void Move()
    {
        if(isMoving)
        {
            rigidbody.isKinematic = false;
            rigidbody.MovePosition(this.rigidbody.position + player.PlayerDirection * playerSpeedMultiplier * -1);
        }
        else
        {
            rigidbody.isKinematic = true;
        }

    }

    void SetMovement()
    {
        if(objectPlayerLine.isTotallyHidden)
        {
            isMoving = true;
        }
        else
        {
            isMoving = false;
        }
    }

    public void SetMove()
    {
        isMoving = true;
    }

    public void SetStay()
    {
        isMoving = false;
    }

    void SetVisibility()
    {
        if (isTotallyVisible)
        {
            spriteRenderer.sortingLayerName = "IsTotallyVisible";
            //print("change to full visibility");
        }
        else // if (isTotallyHidden)
        {
            spriteRenderer.sortingLayerName = "Objects";
            //print("change to no");
        }
    }



  
}
