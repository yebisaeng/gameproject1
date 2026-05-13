using UnityEngine;
using UnityEngine.InputSystem;

public class Player_Movement : MonoBehaviour
{
    
    public float moveSpeed = 5f;
    
    
    private Rigidbody rb;
    private Vector3 moveInput;

    

    void Update()
    {
        // 1. 입력값만 Update에서 미리 받습니다.
        float x = 0f;
        float z = 0f;

        if (Keyboard.current.wKey.isPressed) z = 1f;
        if (Keyboard.current.sKey.isPressed) z = -1f;
        if (Keyboard.current.aKey.isPressed) x = -1f;
        if (Keyboard.current.dKey.isPressed) x = 1f;

        moveInput = (transform.forward * z + transform.right * x).normalized;
    }

    void FixedUpdate()
    {
        // 2. 실제 이동은 물리 프레임에 맞춰 처리해야 흔들리지 않습니다.
        rb.MovePosition(rb.position + moveInput * moveSpeed * Time.fixedDeltaTime);
    }

    
}