using UnityEngine;
//플레이어의 스탯 관리만을 위해 만든 스크립트
public class Stat_Controller : MonoBehaviour
{
    public int damage=1; //데미지를 선언하고 1로 대입
    public int speed=1; //이동속도와 쿨타임을 가속
    public float cool_time=1.0f; //쿨타임
    private FirstPersonMovement move;

    void Start()
    {
        move=GetComponent<FirstPersonMovement>();
    }


    // Update is called once per frame
    void Update()
    {
        cool_time=1.0f/speed; //스피드에 따른 쿨타임을 실시간 책정, 1초를 기준으로 반비례
        move.speedOverrides.Add(()=>move.speed+this.speed); //1인칭 이동 스크립트의 함수로 이 스크립트의 스피드 값만큼 이동속도 증가
    }
}
