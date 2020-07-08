using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectMovement : MonoBehaviour
{
    Rigidbody rigidbody;
    bool isMoving;

    [SerializeField] private float speed;
    [SerializeField] private PlayerMovement player;

    void Start()
    {
        rigidbody = this.GetComponent<Rigidbody>();
        isMoving = false;
    }

    void FixedUpdate()
    {
        Move();
    }

    void Move()
    {
        if(isMoving)
            rigidbody.MovePosition(this.rigidbody.position + player.PlayerDirection * -1);
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
