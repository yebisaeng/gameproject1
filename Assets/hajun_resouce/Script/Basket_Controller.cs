using UnityEngine;

public class Basket_Controller : MonoBehaviour
{
    public int score = 0;
    public int goal = 100;

    private void OnTriggerEnter(Collider other) 
    {
        // 1. 충돌한 물체 본인 또는 부모에게서 스크립트 찾기
        Stone_Generator stone = other.GetComponent<Stone_Generator>();
        if (stone == null) stone = other.GetComponentInParent<Stone_Generator>();

        if (stone != null)
        {
            score += stone.damage;
            // 만약 여기서 점수가 오른다면 UI 연결 문제입니다.
        }
    }
}