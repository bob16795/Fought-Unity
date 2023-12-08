using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SampleController : MonoBehaviour
{
    private float jmpLeft;
    private Rigidbody2D rb;

    [SerializeField]
    private float jmpTime;
    [SerializeField]
    private SpriteRenderer sr;

    private bool jmp;


    [SerializeField]
    private MoveController mc;

    private float dir = 0;

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

    public void processEvent(string s) {
        switch (s) {
            case "Dash":
                sr.color = new Color(0, 255 ,0);
                {
                    rb.velocity += new Vector2(
                        50f * dir,
                        rb.velocity.y
                    );
                };
                break;
            case "Dive":
                sr.color = new Color(0, 0, 255);
                {
                    rb.velocity += new Vector2(
                        rb.velocity.x,
                        -50f
                    );
                };
                break;
            case "Jump":
                sr.color = new Color(255, 0 ,0);
                jmp = true;
                rb.velocity = new Vector2(
                    rb.velocity.x,
                    20f
                );
                break;
            case "Fall":
                sr.color = new Color(128, 0 ,0);
                break;
            default:
                sr.color = new Color(255, 255, 255);
                jmp = false;
                Debug.Log(s);
                break;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        rb.velocity = Vector2.Lerp(rb.velocity, new Vector2(
            150f * Input.GetAxis("Horizontal") * Time.deltaTime,
            rb.velocity.y
        ), Mathf.Clamp(Time.deltaTime * 50f, 0.0f, 1.0f));

        if (Input.GetAxis("Horizontal") < 0) {
            dir = -1;
        }
        if (Input.GetAxis("Horizontal") > 0) {
            dir = 1;
        }

        if (Input.GetKeyDown("x")) {
            mc.Action("q");
        }

        if (Input.GetKeyDown("z")) {
            mc.Action("e");
        }

        if (Input.GetAxis("Vertical") > 0.5)
        {
            if (jmp) {
                 rb.velocity += new Vector2(
                     0f,
                     150f * Time.deltaTime
                 );
             } else if (GroundCheck()) {
                 mc.Action("space");
            }
        } else {
            mc.Action("rel_space");
        }
    }
}
