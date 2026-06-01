using UnityEngine;

[DisallowMultipleComponent]
public class LiyifeiDamageShield : MonoBehaviour
{
    private LiyifeiPlayerPowerupController owner;

    public void Configure(LiyifeiPlayerPowerupController controller, float radius)
    {
        owner = controller;

        SphereCollider shieldCollider = GetComponent<SphereCollider>();
        if (shieldCollider == null) shieldCollider = gameObject.AddComponent<SphereCollider>();
        shieldCollider.isTrigger = true;
        shieldCollider.radius = radius;

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb == null) rb = gameObject.AddComponent<Rigidbody>();
        rb.useGravity = false;
        rb.isKinematic = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (owner != null) owner.TryConsumeDamageImmunity(other);
    }
}
