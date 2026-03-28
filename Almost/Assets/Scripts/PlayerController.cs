using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Rigidbody rb;

    public float speed = 0.1f;

    private float moveX;
    private float moveY;
    
    // Start is called before the first frame update
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        moveX = 0f;
        moveY = 0f;
        
        if (Input.GetKey(KeyCode.W))
        {
            moveY = 1f;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            moveY = -1f;
        }
        else if (Input.GetKey(KeyCode.A))
        {
            moveX = -1f;
        }
        else if(Input.GetKey(KeyCode.D))
        {
            moveX = 1f;
        }
    }

    private void FixedUpdate()
    {
        rb.velocity = new Vector3(moveX * speed, moveY * speed, rb.velocity.z);
    }
}


