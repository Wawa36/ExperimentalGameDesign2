using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    Rigidbody rigidbody;

    [SerializeField] private float speed;

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
            0, Input.GetAxis("Vertical") * speed * Time.fixedDeltaTime);
        rigidbody.MovePosition(this.rigidbody.position + direction);
    }
}
