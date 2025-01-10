using System;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody rb;
    public float speed;
    public float jumpHeight;
    public float dash;
    public float rotSpeed;
    
    private Vector3 dir;
    
    public bool isGrounded = false;
    public LayerMask layer;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        speed = 5f;
        jumpHeight = 10f;
        dash = 5f;
        rotSpeed = 3f;
        dir = Vector3.zero;
    }

    // Update is called once per frame
    void Update()
    {
        if (!GameManager.Instance.isEventAnim)
        {
            dir.x = Input.GetAxis("Horizontal");
            dir.z = Input.GetAxis("Vertical");
            dir.Normalize();

            CheckGround();

            if (Input.GetButtonDown("Jump") && isGrounded)
            {
                Vector3 jumpPower = Vector3.up * jumpHeight;
                rb.AddForce(jumpPower, ForceMode.VelocityChange);
            }

            if (!isGrounded)
            {
                rb.linearDamping = 0;
            }
            else
            {
                rb.linearDamping = 10;
            }

            if (Input.GetButtonDown("Dash"))
            {
                Vector3 dashPower = this.transform.forward * -Mathf.Log(1 / rb.linearDamping) * dash;
                rb.AddForce(dashPower, ForceMode.VelocityChange);
            }
        }
    }

    private void FixedUpdate()
    {
        if (!GameManager.Instance.isEventAnim)
        {
            if (dir != Vector3.zero)
            {
                if (Mathf.Sign(transform.forward.x) != Mathf.Sign(dir.x) ||
                    Mathf.Sign(transform.forward.z) != Mathf.Sign(dir.z))
                {
                    transform.Rotate(0, 1, 0);
                }

                transform.forward = Vector3.Lerp(transform.forward, dir, Time.deltaTime * rotSpeed);
            }

            rb.MovePosition(transform.position + dir * speed * Time.deltaTime);
        }
    }

    void CheckGround()
    {
        RaycastHit hit;

        if (Physics.Raycast(transform.position + (Vector3.up * 0.2f), Vector3.down, out hit, 0.4f, layer))
        {
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }
    }
}
