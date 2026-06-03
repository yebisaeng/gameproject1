using UnityEngine;

[DisallowMultipleComponent]
public class LiyifeiPowerupPickup : MonoBehaviour
{
    [SerializeField] private LiyifeiPowerupSettings settings = new LiyifeiPowerupSettings();
    [SerializeField] private LiyifeiPlayerPowerupController targetPlayer;

    private LiyifeiPowerupSpawner owner;
    private bool consumed;

    public LiyifeiPowerupSettings Settings => settings;

    public void Initialize(
        LiyifeiPowerupSettings sourceSettings,
        LiyifeiPlayerPowerupController player,
        LiyifeiPowerupSpawner spawner)
    {
        settings = sourceSettings;
        targetPlayer = player;
        owner = spawner;
        consumed = false;
        EnsurePickupPhysics();
    }

    private void Awake()
    {
        EnsurePickupPhysics();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (consumed) return;

        LiyifeiPlayerPowerupController player = ResolvePlayer(other);
        if (player == null) return;
        if (targetPlayer != null && player != targetPlayer) return;

        consumed = true;
        player.ApplyPowerup(settings);
        if (owner != null) owner.NotifyPickupRemoved(this);
        Destroy(gameObject);
    }

    private LiyifeiPlayerPowerupController ResolvePlayer(Collider other)
    {
        LiyifeiPlayerPowerupController player = other.GetComponentInParent<LiyifeiPlayerPowerupController>();
        if (player != null) return player;

        if (other.CompareTag("Player"))
            return other.GetComponent<LiyifeiPlayerPowerupController>();

        return null;
    }

    private void EnsurePickupPhysics()
    {
        Collider pickupCollider = GetComponent<Collider>();
        if (pickupCollider == null)
        {
            SphereCollider sphere = gameObject.AddComponent<SphereCollider>();
            sphere.radius = settings != null ? settings.pickupRadius : 1.1f;
            pickupCollider = sphere;
        }

        pickupCollider.isTrigger = true;

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb == null) rb = gameObject.AddComponent<Rigidbody>();
        rb.useGravity = false;
        rb.isKinematic = true;
    }
}
