using UnityEngine;

public class ScoreGhost : MonoBehaviour
{
    private Transform target;
    private float moveSpeed;
    private Basket_Controller basket;
    private bool hasHit = false;

    public void Init(Transform target, float moveSpeed, Basket_Controller basket)
    {
        this.target = target;
        this.moveSpeed = moveSpeed;
        this.basket = basket;
    }

    private void Update()
    {
        if (hasHit || target == null) return;

        Vector3 dir = target.position - transform.position;
        dir.y = 0f;
        dir.Normalize();

        transform.position += dir * moveSpeed * Time.deltaTime;
        if (dir != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(dir);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (hasHit) return;

        if (other.CompareTag("Stone"))
        {
            hasHit = true;
            Destroy(other.gameObject);
            Destroy(gameObject);
            return;
        }

        PlayerKnockback knockback = other.GetComponentInParent<PlayerKnockback>();
        if (knockback != null)
        {
            hasHit = true;

            if (basket != null)
                basket.score = Mathf.RoundToInt(basket.score / 2f);

            Destroy(gameObject);
        }
    }
}
  