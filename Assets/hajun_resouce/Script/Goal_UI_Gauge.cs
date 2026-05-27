using UnityEngine;
using UnityEngine.UI; // UI 사용을 위해 필수
//점수 UI에서 빨간 게이지를 증가시키는 스크립트
public class Goal_UI_Gauge : MonoBehaviour
{
    [SerializeField] private Basket_Controller basket; //점수를 가져올 바구니 스크립트 객체
    [SerializeField] public Image gauge; // 게이지로 쓸 Image 객체
    void Start()
    {
        gauge.fillAmount=0; //게이지 초기값
    }

    void Update()
    {
        // (점수 / 목표) 비율을 fillAmount에 대입
        gauge.fillAmount = (float) basket.score / basket.goal;
    }
}