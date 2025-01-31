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
    public float fallMultiplier = 2.5f;  // 빠른 낙하를 위한 중력 배수
    public float lowJumpMultiplier = 2f; // 짧은 점프 제어
    
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
        speed = 40f;
        jumpHeight = 40f;
        dash = 120f;
        rotSpeed = 10f;
        dir = Vector3.zero;
        
        body = transform.Find("Body");
        objectRenderer = body.gameObject.GetComponent<Renderer>();
        if (objectRenderer != null && body != null )
        {
            originalMaterial = objectRenderer.material;
        }
    }

    void Update()
    {
        if (GameManager.Instance.superJumping)
        {
            jumpHeight = 80f;
        }
        else
        {
            jumpHeight = 40f;
        }
        
        if (!GameManager.Instance.isEventAnim)
        {
            /*if (!GameManager.Instance.isRevers2)
            {
                dir.x = Input.GetAxis("Horizontal");
                dir.z = Input.GetAxis("Vertical");
            }
            else
            {
                dir.x = Input.GetAxis("Vertical");
                dir.z = Input.GetAxis("Horizontal");
            }*/
            
            dir.x = GameManager.Instance.isRevers2 ? Input.GetAxis("Vertical") : Input.GetAxis("Horizontal");
            dir.z = GameManager.Instance.isRevers2 ? Input.GetAxis("Horizontal") : Input.GetAxis("Vertical");

            dir.Normalize();

            CheckGround();

            if (Time.timeScale >= 1 && Input.GetButtonDown("Jump") && isGrounded && GameManager.Instance.playerMP >= 20f)
            {
                Vector3 jumpPower = Vector3.up * jumpHeight;
                rb.AddForce(jumpPower, ForceMode.VelocityChange);
                GameManager.Instance.playerMP -= 20f;
                GameManager.Instance.playerMP = Mathf.Max(0, GameManager.Instance.playerMP);
                GameManager.Instance.playerMP = Mathf.Clamp(GameManager.Instance.playerMP, 0f, 100f);
            }

            if (!isGrounded)
            {
                rb.linearDamping = 0;
            }
            else
            {
                rb.linearDamping = 10;
            }
            
            // 낙하 속도 조정 (빠른 낙하 적용)
            if (rb.linearVelocity.y < 0)
            {
                rb.linearVelocity += Vector3.up * Physics.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
            }
            else if (rb.linearVelocity.y > 0 && !Input.GetButton("Jump"))
            {
                rb.linearVelocity += Vector3.up * Physics.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
            }

            if (Input.GetButtonDown("Dash") && isGrounded && GameManager.Instance.playerMP >= 10f)
            {
                Vector3 dashVelocity = transform.forward * dash;
                rb.linearVelocity = new Vector3(dashVelocity.x, rb.linearVelocity.y, dashVelocity.z);
                GameManager.Instance.playerMP -= 10f;
                GameManager.Instance.playerMP = Mathf.Max(0, GameManager.Instance.playerMP);
                GameManager.Instance.playerMP = Mathf.Clamp(GameManager.Instance.playerMP, 0f, 100f);
            }

            if (GameManager.Instance.playerMP <= 100f)
            {
                GameManager.Instance.playerMP += 3f * Time.deltaTime;
                GameManager.Instance.playerMP = Mathf.Min(100f, GameManager.Instance.playerMP);
                GameManager.Instance.playerMP = Mathf.Clamp(GameManager.Instance.playerMP, 0f, 100f);
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

                if (GameManager.Instance.isRevers)
                {
                    transform.forward = Vector3.Lerp(transform.forward, dir, Time.deltaTime * rotSpeed);
                }
                else
                {
                    transform.forward = Vector3.Lerp(transform.forward, -dir, Time.deltaTime * rotSpeed);
                }
            }

            if (GameManager.Instance.isRevers)
            {
                rb.MovePosition(rb.position + (dir * speed * Time.deltaTime));
            }
            else
            {
                rb.MovePosition(rb.position - (dir * speed * Time.deltaTime));
            }
        }
    }

    void CheckGround()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position + Vector3.up * 0.2f, Vector3.down, out hit, 0.5f, layer))
        {
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }

        Debug.DrawRay(transform.position + Vector3.up * 0.2f, Vector3.down * 0.5f, Color.red);
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
        GameManager.Instance.playerHP = Mathf.Clamp(GameManager.Instance.playerHP, 0f, 100f);

        if (GameManager.Instance.playerHP <= 0f)
        {
            Debug.Log("HP 0!");
            objectRenderer.material = originalMaterial; // 원래 Material 적용
            GameManager.Instance.failFlag = true;
        }
        
        yield return new WaitForSeconds(1f);
        if (objectRenderer != null)
        {
            objectRenderer.material = originalMaterial; // 원래 Material 적용
        }
        curCo = null;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Wall"))
        {
            Debug.Log("Wall");
        }
    }
}
