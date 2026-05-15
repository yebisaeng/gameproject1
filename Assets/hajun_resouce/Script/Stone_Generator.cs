using UnityEngine;
using UnityEngine.InputSystem; //입출력용

public class Stone_Generator : MonoBehaviour
{
    public GameObject Stone_Prefab; //인스펙터에서 연결할 객체
    

    void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame) //마우스 왼쪽 클릭 시
        {
            // 1. 메인 카메라의 위치와 회전 정보를 가져옵니다.
            Transform camTransform = Camera.main.transform;

            // 2. 생성 위치: 카메라 좌표에서 카메라가 바라보는 정면(forward)으로 1미터 앞.
            // 이렇게 하면 위를 보든 아래를 보든 항상 화면 중앙에서 생성됩니다.
            Vector3 spawnPos = camTransform.position + camTransform.forward * 2.0f;

            // 3. 돌 생성 (카메라와 같은 회전값 적용)
            GameObject stone = Instantiate(Stone_Prefab, spawnPos, camTransform.rotation); //

            // 4. 발사 방향: 화면 정중앙을 향하는 레이 생성
            Ray ray = Camera.main.ScreenPointToRay(new Vector2(Screen.width / 2, Screen.height / 2));
            
            Stone_Controller controller = stone.GetComponent<Stone_Controller>(); //controller.Shoot을 위한 사전 작업.

            if (controller != null)
            {
                // ray.direction을 사용하여 카메라가 바라보는 정중앙으로 힘을 줍니다.
                controller.Shoot(ray.direction * 2000); //dir 값이 카메라 방향*2000
            }
        }
    }
}