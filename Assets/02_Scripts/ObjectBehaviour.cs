﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ObjectBehaviour : MonoBehaviour
{
    Rigidbody rigidbody;
    [SerializeField] ObjectPlayerLine objectPlayerLine;
    bool isMoving;
    bool isTotallyVisible { get { return objectPlayerLine.isTotallyVisible; } set {; } }
    bool isTotallyHidden { get { return objectPlayerLine.isTotallyHidden; } set {; } }
    bool isIntersected { get { return objectPlayerLine.isIntersected; } set {; } }
    SpriteRenderer spriteRenderer;

    [SerializeField] private float playerSpeedMultiplier;
    private PlayerMovement player;
    AudioManager audioManager;
    bool wasTotallyVisible = false;
    bool wasTotallyHidden = false;
    bool wasIntersected = false;

    public bool isFrozen;
    void Awake()
    {
        audioManager = this.GetComponent<AudioManager>();
    }

    void Start()
    {
        GetComponentInChildren<SpriteRenderer>().color = ObjectManager.Instance.ColorNormal;
        rigidbody = this.GetComponent<Rigidbody>();
        objectPlayerLine = this.GetComponent<ObjectPlayerLine>();
        isMoving = false;
        // TO DO: initialize spriteRenderer
        spriteRenderer = this.GetComponentInChildren<SpriteRenderer>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();
    }

    private void Update()
    {
        if (!isFrozen)
        {
            SetMovement();
        }

        SetVisibility();

        ManageSound();
    }

    void FixedUpdate()
    {
        Move();
    }

    void Move()
    {
        if(isMoving)
        {
            //rigidbody.constraints = RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
            rigidbody.mass = 1;
            rigidbody.drag = 0;
            rigidbody.angularDrag = 0.05f;
            rigidbody.MovePosition(this.rigidbody.position + player.PlayerDirection * playerSpeedMultiplier * -1);
        }
        else
        {
            rigidbody.mass = 1000000;
            rigidbody.drag = 1000000;
            rigidbody.angularDrag = 1000000;
        }

    }

    void ManageSound()
    {
        if (audioManager != null)
        {
            // (1) Move-sound
            if (isTotallyHidden && player.PlayerDirection.magnitude >= 0.01)
            {
                audioManager.Play("Move", true);
            }
            else
                audioManager.Stop("Move");

            // (2) Visibility-sound
            if (isTotallyVisible && !wasTotallyVisible)
            {
                audioManager.Play("GetsTotallyVisible");
                wasTotallyVisible = true;
                wasIntersected = false;
                wasTotallyHidden = false;
            }
            //else if (isIntersected && !wasIntersected)
            //{
            //    //audioManager.Play("GetsIntersected");
            //    wasIntersected = true;
            //    wasTotallyVisible = false;
            //    wasTotallyHidden = false;
            //    print("intersected");
            //}
            else if (isTotallyHidden && !wasTotallyHidden)
            {
                audioManager.Play("GetsTotallyHidden");
                wasTotallyHidden = true;
                wasIntersected = false;
                wasTotallyVisible = false;
                print("gets totally Hidden");
            }
        }
        else
            Debug.LogError("AudioManager on Obj (" + this.gameObject.name + ") == null");
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
