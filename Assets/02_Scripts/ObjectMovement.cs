using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectMovement : MonoBehaviour
{
    Rigidbody rigidbody;

    [SerializeField] private float speed;
    [SerializeField] private PlayerMovement player;

    void Start()
    {
        rigidbody = this.GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        Move();
    }

    void Move()
    {

        rigidbody.MovePosition(this.rigidbody.position + player.PlayerDirection * -1);
    }
}
