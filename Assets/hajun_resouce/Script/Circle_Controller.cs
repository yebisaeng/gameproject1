using UnityEngine;

public class Circle_Controller : MonoBehaviour
{
    public Transform basket; //바구니 트랜스폼
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (basket != null)
        {
            // 대상의 위치를 그대로 내 위치로 설정
            transform.position = new Vector3(basket.position.x, transform.position.y, basket.position.z); //땅에 고정되엇 x, z 좌표만 바구니 따라 바뀜
        }
    }
}
