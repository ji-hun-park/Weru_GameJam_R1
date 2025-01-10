using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraFollow : MonoBehaviour
{
    public Transform target; // 따라갈 대상
    public Vector3 offset = new Vector3(0, 5, -15); // 초기 오프셋
    public float rotationSpeed = 5f; // 회전 속도
    public float offsetAdjustSpeed = 2f; // 오프셋 조정 속도

    private Vector2 lookInput;

    private void Start()
    {
        StartCoroutine(InitPlayer());
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
        }
    }

    void LateUpdate()
    {
        if (target != null)
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

    IEnumerator InitPlayer()
    {
        yield return null;
        target = GameManager.Instance.player;
    }
}