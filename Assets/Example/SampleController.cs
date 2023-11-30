using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SampleController : MonoBehaviour
{
    private float jmpLeft;
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    
    bool GroundCheck()
    {
        RaycastHit2D hit;
        float distance = 0.01f;
        Vector2 dir = new Vector2(0, -1);

        return Physics2D.Raycast((Vector2)transform.position - Vector2.up * 0.51f, dir, distance);
    }

    // Update is called once per frame
    void Update()
    {
        if (GroundCheck()) {
            jmpLeft = 0.25f;
        }

        if (Input.GetKey("a"))
        {
            rb.AddForce(transform.right * -2f);
        }
        if (Input.GetKey("d"))
        {
            rb.AddForce(transform.right * 2f);
        }
        if (Input.GetKey("w") || Input.GetKey("space"))
        {
            if (jmpLeft > 0) {
                jmpLeft -= Time.deltaTime;
                Debug.Log("lolo");
                rb.AddForce(transform.up * 10f);
            }
        } else {
            jmpLeft = 0f;
        }
    }
}
