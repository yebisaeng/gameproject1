using UnityEngine;
using TMPro; //텍스트 UI용

public class Goal_UI_txt : MonoBehaviour
{
    
    [SerializeField] private Basket_Controller basket; //바구니 스크립트 객체
    [SerializeField] private TextMeshProUGUI gaugeText; //텍스트 UI 객체

    void Update()
    {
        // 2. 안전장치: 참조 대상이 있을 때만 실행
        if (basket != null && gaugeText != null) 
        {
            // 3. 원본 스크립트의 변수를 직접 참조하여 실시간 반영
            gaugeText.text = basket.score + " / " + basket.goal; // 점수/목표
        }
    }
}
