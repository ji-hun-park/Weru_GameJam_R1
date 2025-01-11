using System;
using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody rb;
    public float speed;
    public float jumpHeight;
    public float dash;
    public float rotSpeed;
    
    public Material venomMaterial;  // 감염 Material
    private Material originalMaterial; // 원래 Material
    private Renderer objectRenderer;
    private Transform body;
    
    private Vector3 dir;
    
    public bool isGrounded = false;
    public LayerMask layer;
    public Coroutine curCo;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        speed = 80f;
        jumpHeight = 10f;
        dash = 40f;
        rotSpeed = 5f;
        dir = Vector3.zero;
        
        body = transform.Find("Body");
        objectRenderer = body.gameObject.GetComponent<Renderer>();
        if (objectRenderer != null && body != null )
        {
            originalMaterial = objectRenderer.material;
        }
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

    public void Damaged()
    {
        Debug.Log("Damaged!");
        
        if (curCo == null)
            curCo = StartCoroutine(DamCo());
    }

    private IEnumerator DamCo()
    {
        if (objectRenderer != null)
        {
            objectRenderer.material = venomMaterial; // 감염 Material 적용
        }

        GameManager.Instance.playerHP -= 20f;
        GameManager.Instance.playerHP = Mathf.Max(GameManager.Instance.playerHP, 0);
        
        yield return new WaitForSeconds(1f);
        if (objectRenderer != null)
        {
            objectRenderer.material = originalMaterial; // 원래 Material 적용
        }
        curCo = null;
    }
}
