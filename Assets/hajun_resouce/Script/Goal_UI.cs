using UnityEngine;
using TMPro;

public class Goal_UI : MonoBehaviour
{
    // 1. 참조할 대상과 UI를 인스펙터에서 직접 연결 (가장 권장)
    [SerializeField] private Basket_Controller basket; 
    [SerializeField] private TextMeshProUGUI gaugeText;

    void Update()
    {
        // 2. 안전장치: 참조 대상이 있을 때만 실행
        if (basket != null && gaugeText != null) 
        {
            // 3. 원본 스크립트의 변수를 직접 참조하여 실시간 반영
            gaugeText.text = basket.score + " / " + basket.goal;
        }
    }
}
