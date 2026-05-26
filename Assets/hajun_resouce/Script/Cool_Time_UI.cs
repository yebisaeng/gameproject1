using UnityEngine;
using UnityEngine.UI;

public class Cool_Time_UI : MonoBehaviour
{
    [SerializeField] public Image gauge; //쿨타임 게이지 이미지
    [SerializeField] private Stone_Generator Cool; //스톤 제너레이터 객체
    void Start()
    {
        gauge.fillAmount=1;
    }

    // Update is called once per frame
    void Update()
    {
        gauge.fillAmount=1-(Cool.counting/Cool.cool); //  현재 쿨타임 카운팅 / 발사 당시 쿨타임
    }
}
