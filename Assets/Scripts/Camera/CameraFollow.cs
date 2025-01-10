using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraFollow : MonoBehaviour
{
    public Transform target; // 따라갈 대상
    public Vector3 offset; // 초기 오프셋
    public float rotationSpeed = 5f; // 회전 속도
    public float offsetAdjustSpeed = 5f; // 오프셋 조정 속도

    private Vector2 lookInput;

    public Vector3 fixedPosition = new Vector3(0, 10, -15);
    public Vector3 fixedRotation = new Vector3(25, 0, 0);
    
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
    }

    void LateUpdate()
    {
        if (GameManager.Instance.isEventAnim)
        {
            // 카메라 위치 고정
            transform.position = fixedPosition;
            transform.rotation = Quaternion.Euler(fixedRotation);
        }
        
        if (target != null && !GameManager.Instance.isEventAnim)
        {
            // 마우스 입력
            lookInput = Mouse.current.delta.ReadValue();
            float yaw = lookInput.x * rotationSpeed * Time.deltaTime;
            float pitch = -lookInput.y * rotationSpeed * Time.deltaTime;

            // 카메라 회전
            Quaternion rotation = Quaternion.Euler(pitch, yaw, 0f);
            transform.position = target.position + rotation * offset;
            transform.LookAt(target);
        }
    }

    private void InitSet()
    {
        offset = new Vector3(0, 5f, -15f);
        rotationSpeed = 5f;
        offsetAdjustSpeed = 5f;
        StartCoroutine(InitPlayer());
    }

    IEnumerator InitPlayer()
    {
        yield return new WaitForSeconds(0.1f);
        target = GameManager.Instance.player.transform;
    }
}