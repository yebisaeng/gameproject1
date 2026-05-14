using UnityEngine;

public class Stone_Controller : MonoBehaviour
{
    Rigidbody rb; //리지드 바디 객체
    public void Shoot(Vector3 dir) //3차원 벡터 객체
    {
        GetComponent<Rigidbody>().AddForce(dir); //dir 값만큼 힘을 가한다.       
    }

    void OnCollisionEnter(Collision collision) //충돌 시 작동
    {
        if(collision.gameObject.CompareTag("Basket")) //충돌 대상의 태그가 Basket이면 작동
        {
            Destroy(gameObject);  //즉시 삭제
        }
        else
        {
            rb.linearVelocity = rb.linearVelocity * 0.1f; //감속
            Destroy(gameObject,1f); //1f 후에 삭제
        }
    }
    void Start()
    {
        rb=GetComponent<Rigidbody>(); //리지드 바디 가져오기
        Application.targetFrameRate = 60; //프레임 60으로 설정
        // 못 맞추고 날아가는 경우를 대비해 3f 뒤 삭제
        Destroy(gameObject, 3f); 
    }
}