using UnityEngine;

public class Stone_Controller : MonoBehaviour
{
    public int damage=1;
    public void Shoot(Vector3 dir)
    {
        GetComponent<Rigidbody>().AddForce(dir);        
    }

    void OnTriggerEnter(Collider other)
    {
        Destroy(gameObject); //충돌 시 삭제
    }

    void OnCollisionEnter(Collision collision)
    {
        Destroy(gameObject,1f);
        GetComponent<Rigidbody>().isKinematic=true;
    }
    void Start()
    {
        Application.targetFrameRate = 60;
        // 못 맞추고 날아가는 경우를 대비해 3초 뒤 삭제
        Destroy(gameObject, 3f);
    }
}