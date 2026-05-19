using UnityEngine;
//돌과의 충돌을 감지하여 점수를 증가시키는 것을 관리하는 스크립트
public class Basket_Controller : MonoBehaviour
{
    public int score = 0; //현재 점수
    public int goal = 10; //임시로 설정한 스테이지 목표
    public GameObject p; //플레이어를 담는 객체
    public int dam; //플레이어에게서 가져온 데미지를 저장하는 변수

    void Start()
    {
        p=GameObject.Find("Player"); //p에 플레이어의 정보를 넣음
    }

    void Update()
    {
        dam=p.GetComponent<Stat_Controller>().damage; //계속 플레이어가 가진 공격력 값 갱신
    }
    private void OnCollisionEnter(Collision collision) //충돌 발생 시 작동
    {
        if (collision.gameObject.CompareTag("Stone")) //'Stone' 태그를 가진 물체와 부딪혔을 때만 점수 증가. 여기서는 Stone_Prefab의 클론
        {
            if(score+dam<goal) //현재 점수와 데미지 합계가 목표치를 넘으면 현재 점수를 목표치로 변경
            {
                score += dam; //데미지만큼 점수 증가
            }
            else if(score+dam>=goal){
                score=goal;
            }
            else
            {
                Debug.Log("상정하지 않은 경우");
            }
        }
    }
}