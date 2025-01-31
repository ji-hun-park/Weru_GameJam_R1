using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraFollow : MonoBehaviour
{
    public Transform target; // 따라갈 대상
    public Vector3 offset; // 초기 오프셋
    public float rotationSpeed = 100f; // 회전 속도
    public float offsetAdjustSpeed = 100f; // 오프셋 조정 속도

    private Vector2 lookInput;

    public Vector3 dir = Vector3.zero;
    public Vector3 fixedPosition = new Vector3(0, 10, -15);
    public Vector3 fixedRotation = new Vector3(5, 0, 0);
    public Vector3 fixedRotN = new Vector3(0, 0, 0);
    public Vector3 fixedRotR = new Vector3(0, 180, 0);
    
    private void Start()
    {
        InitSet();
    }

    void Update()
    {
        if (target != null)
        {
            // 오프셋 조정 키 입력 처리
            if (Keyboard.current.iKey.isPressed)
                offset.y += offsetAdjustSpeed * Time.deltaTime; // 위로 이동
            if (Keyboard.current.kKey.isPressed)
                offset.y -= offsetAdjustSpeed * Time.deltaTime; // 아래로 이동
            if (Keyboard.current.jKey.isPressed)
                offset.z += offsetAdjustSpeed * Time.deltaTime; // 더 가까이
            if (Keyboard.current.lKey.isPressed)
                offset.z -= offsetAdjustSpeed * Time.deltaTime; // 더 멀리
            if (Keyboard.current.uKey.isPressed)
                offset.x += offsetAdjustSpeed * Time.deltaTime; // 좌로 돌리기
            if (Keyboard.current.oKey.isPressed)
                offset.x -= offsetAdjustSpeed * Time.deltaTime; // 우로 돌리기
        }

        /*dir.x = Input.GetAxis("Horizontal") * Time.deltaTime * rotationSpeed;
        offset.x += dir.x;
        dir.z = Input.GetAxis("Vertical") * Time.deltaTime * rotationSpeed;
        offset.z += dir.z;*/
    }

    void LateUpdate()
    {
        if (GameManager.Instance.isEventAnim)
        {
            // 카메라 위치 고정
            transform.position = fixedPosition;
            transform.rotation = Quaternion.Euler(fixedRotation);
        }
        
        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (GameManager.Instance.isRevers)
            {
                //transform.rotation = Quaternion.Euler(fixedRotN);
                //transform.rotation = Quaternion.Euler(fixedRotN);
                GameManager.Instance.isRevers = false;
            }
            else
            {
                //transform.rotation = Quaternion.Euler(fixedRotR);
                //transform.rotation = Quaternion.Euler(fixedRotR);
                GameManager.Instance.isRevers = true;
            }
        }
        
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (GameManager.Instance.isRevers2)
            {
                //transform.rotation = Quaternion.Euler(fixedRotN);
                //transform.rotation = Quaternion.Euler(fixedRotN);
                GameManager.Instance.isRevers2 = false;
            }
            else
            {
                //transform.rotation = Quaternion.Euler(fixedRotR);
                //transform.rotation = Quaternion.Euler(fixedRotR);
                GameManager.Instance.isRevers2 = true;
            }
        }
        
        if (target != null && !GameManager.Instance.isEventAnim)
        {
            // 마우스 입력
            //lookInput = Mouse.current.delta.ReadValue();
            lookInput = Vector2.zero;
            float yaw = lookInput.x * rotationSpeed * Time.deltaTime;
            float pitch = -lookInput.y * rotationSpeed * Time.deltaTime;

            // 카메라 회전
            Quaternion rotation = Quaternion.Euler(pitch, yaw, 0f);
            transform.position = target.position + rotation * offset;
            //transform.rotation = Quaternion.Euler(new Vector3(0f, 100f, 0f));
            transform.LookAt(target);
        }
    }

    private void InitSet()
    {
        offset = new Vector3(0f, 100f, 120f);
        rotationSpeed = 100f;
        offsetAdjustSpeed = 100f;
        StartCoroutine(InitPlayer());
        fixedPosition = new Vector3(0, 10, -15);
        fixedRotation = new Vector3(5, 0, 0);
    }

    IEnumerator InitPlayer()
    {
        yield return new WaitForSeconds(0.1f);
        target = GameManager.Instance.player.transform;
    }
}