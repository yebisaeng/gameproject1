using UnityEngine;

public class ChargingGhost : MonoBehaviour
{
    private Transform target;
    private float moveSpeed;
    private float knockbackForce;
    private float stunDuration;
    private bool hasHit = false;

    public void Init(Transform target, float moveSpeed, float knockbackForce, float stunDuration)
    {
        this.target = target;
        this.moveSpeed = moveSpeed;
        this.knockbackForce = knockbackForce;
        this.stunDuration = stunDuration;
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

        PlayerKnockback knockback = other.GetComponentInParent<PlayerKnockback>();
        if (knockback == null) return;

        hasHit = true;

        Vector3 knockDir = transform.forward;
        knockDir.y = 0f;
        knockDir.Normalize();

        knockback.Knock(knockDir, knockbackForce, stunDuration);

        Destroy(gameObject);
    }
}