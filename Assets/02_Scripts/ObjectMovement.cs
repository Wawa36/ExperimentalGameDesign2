﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectMovement : MonoBehaviour
{
    Rigidbody rigidbody;
    ObjectPlayerLine objectPlayerLine;
    bool isMoving;

    [SerializeField] private float speed;
    [SerializeField] private PlayerMovement player;

  
    void Start()
    {
        rigidbody = this.GetComponent<Rigidbody>();
        objectPlayerLine = this.GetComponent<ObjectPlayerLine>();
        isMoving = false;
    }

    private void Update()
    {
        SetMovement();
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
            rigidbody.MovePosition(this.rigidbody.position + player.PlayerDirection * -1);
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
}
