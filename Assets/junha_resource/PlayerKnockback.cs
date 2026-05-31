using System.Collections;
using UnityEngine;

public class PlayerKnockback : MonoBehaviour
{
    [SerializeField] private FirstPersonMovement movement;
    [SerializeField] private float upwardForce = 2f;

    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        if (movement == null) movement = GetComponent<FirstPersonMovement>();
    }

    public void Knock(Vector3 direction, float force, float stunDuration)
    {
        StartCoroutine(KnockRoutine(direction, force, stunDuration));
    }

    private IEnumerator KnockRoutine(Vector3 direction, float force, float stunDuration)
    {
        if (movement != null) movement.enabled = false;

        Vector3 knockVelocity = direction * force + Vector3.up * upwardForce;
        rb.linearVelocity = knockVelocity;

        yield return new WaitForSeconds(stunDuration);

        if (movement != null) movement.enabled = true;
    }
}