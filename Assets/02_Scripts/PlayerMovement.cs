using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    Rigidbody rigidbody;

    [SerializeField] public float speed;
    private Vector3 playerDirection;

    public Vector3 PlayerDirection
    {
        get => playerDirection;
    }

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
        Vector3 direction = new Vector3(Input.GetAxis("Horizontal") * speed * Time.fixedDeltaTime,
            Input.GetAxis("Vertical") * speed * Time.fixedDeltaTime, 0);
        rigidbody.MovePosition(this.rigidbody.position + direction);

        playerDirection = direction;
    }
}
