using UnityEngine;
using UnityEngine.InputSystem;

public class Player_Camera : MonoBehaviour
{
    public float sensitivity = 0.1f;
    public Transform cameraTransform; // 인스펙터에서 메인 카메라를 드래그해서 넣어주세요.

    private float xRotation = 0f;
    private float yRotation = 0f;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        // 물리 회전은 코드에서 제어하므로 축을 고정합니다.
        rb.freezeRotation = true;
        Cursor.lockState = CursorLockMode.Locked;

        if (cameraTransform == null)
            cameraTransform = Camera.main.transform;
    }

    void LateUpdate()
    {
        Vector2 mouseDelta = Mouse.current.delta.ReadValue();

        // 1. 값 누적 (Time.deltaTime은 InputSystem Delta에서 제외하는 것이 더 부드럽습니다)
        yRotation += mouseDelta.x * sensitivity;
        xRotation -= mouseDelta.y * sensitivity;

        // 2. 상하 회전 제한 (고개 꺾임 방지)
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        // 3. 회전 적용
        // 몸통(이 스크립트가 붙은 오브젝트)은 좌우로만 회전
        transform.rotation = Quaternion.Euler(0f, yRotation, 0f);
        
        // 카메라만 위아래로 회전
        cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }
}