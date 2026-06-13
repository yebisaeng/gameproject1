using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

[DisallowMultipleComponent]
public class LiyifeiPowerupSpawner : MonoBehaviour
{
    [Header("Scene References")]
    [SerializeField] private LiyifeiPlayerPowerupController targetPlayer;
    [SerializeField] private Collider groundCollider;
    [SerializeField] private Transform spawnedRoot;

    [Header("Spawn Timing")]
    [Min(0f)] public float initialDelay = 1f;
    [Min(0f)] public float spawnInterval = 8f;
    [Min(0)] public int initialSpawnCount = 5;
    [Min(1)] public int maxActivePickups = 8;

    [Header("Ground Area")]
    public Vector2 areaCenter = new Vector2(0.27f, 3.76f);
    public Vector2 areaSize = new Vector2(22f, 22f);
    [Min(0.01f)] public float raycastHeight = 25f;
    [Min(0f)] public float groundOffset = 0.35f;
    [Min(1)] public int maxPositionAttempts = 20;
    public LayerMask groundMask = ~0;

    [Header("Powerups")]
    public LiyifeiPowerupSettings[] powerups =
    {
        new LiyifeiPowerupSettings
        {
            type = LiyifeiPowerupType.MagicStone,
            displayName = "Magic Stone",
            duration = 15f,
            damageMultiplier = 2f
        },
        new LiyifeiPowerupSettings
        {
            type = LiyifeiPowerupType.Broom,
            displayName = "Broom",
            duration = 15f,
            destroyProjectileOnTargetHit = true,
            destroyTargetKeywords = new[] { "Bat", "Spider", "Cobweb" }
        },
        new LiyifeiPowerupSettings
        {
            type = LiyifeiPowerupType.GhostMask,
            displayName = "Ghost Mask",
            duration = 0f,
            immunityCharges = 1,
            immunityDuration = 30f,
            damageSourceKeywords = new[] { "Ghost", "Bat", "Spider" }
        },
        new LiyifeiPowerupSettings
        {
            type = LiyifeiPowerupType.PumpkinPie,
            displayName = "Pumpkin Pie",
            duration = 12f,
            moveSpeedMultiplier = 1.5f
        },
        new LiyifeiPowerupSettings
        {
            type = LiyifeiPowerupType.Stopwatch,
            displayName = "Stopwatch",
            bonusSeconds = 30f
        }
    };

    private readonly List<LiyifeiPowerupPickup> activePickups = new List<LiyifeiPowerupPickup>();

    private void Awake()
    {
        CacheReferences();
    }

    private IEnumerator Start()
    {
        yield return new WaitForSeconds(initialDelay);

        for (int i = 0; i < initialSpawnCount; i++)
            SpawnOne();

        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);
            SpawnOne();
        }
    }

    public void NotifyPickupRemoved(LiyifeiPowerupPickup pickup)
    {
        activePickups.Remove(pickup);
    }

    public void SpawnOne()
    {
        RemoveMissingPickups();
        if (activePickups.Count >= maxActivePickups) return;

        LiyifeiPowerupSettings settings = PickSettings();
        if (settings == null) return;

        Vector3 spawnPosition = PickGroundPosition();
        GameObject pickupObject = CreatePickupObject(settings);
        pickupObject.transform.SetPositionAndRotation(spawnPosition, Quaternion.identity);

        if (spawnedRoot != null)
            pickupObject.transform.SetParent(spawnedRoot, true);

        LiyifeiPowerupPickup pickup = pickupObject.GetComponent<LiyifeiPowerupPickup>();
        if (pickup == null) pickup = pickupObject.AddComponent<LiyifeiPowerupPickup>();
        pickup.Initialize(settings, targetPlayer, this);

        activePickups.Add(pickup);
    }

    private void CacheReferences()
    {
        if (targetPlayer == null)
            targetPlayer = FindFirstObjectByType<LiyifeiPlayerPowerupController>();

        if (groundCollider == null)
        {
            GameObject floor = GameObject.Find("Floor");
            if (floor != null) groundCollider = floor.GetComponent<Collider>();
        }

        if (spawnedRoot == null)
        {
            GameObject root = new GameObject("LiyifeiPowerup_RuntimeItems");
            spawnedRoot = root.transform;
        }
    }

    private LiyifeiPowerupSettings PickSettings()
    {
        if (powerups == null || powerups.Length == 0) return null;

        float totalWeight = 0f;
        for (int i = 0; i < powerups.Length; i++)
        {
            if (powerups[i] == null) continue;
            totalWeight += Mathf.Max(0f, powerups[i].spawnWeight);
        }

        if (totalWeight <= 0f) return null;

        float roll = Random.Range(0f, totalWeight);
        for (int i = 0; i < powerups.Length; i++)
        {
            if (powerups[i] == null) continue;

            roll -= Mathf.Max(0f, powerups[i].spawnWeight);
            if (roll <= 0f) return powerups[i];
        }

        return powerups[powerups.Length - 1];
    }

    private Vector3 PickGroundPosition()
    {
        for (int i = 0; i < maxPositionAttempts; i++)
        {
            float x = areaCenter.x + Random.Range(-areaSize.x * 0.5f, areaSize.x * 0.5f);
            float z = areaCenter.y + Random.Range(-areaSize.y * 0.5f, areaSize.y * 0.5f);
            Vector3 origin = new Vector3(x, transform.position.y + raycastHeight, z);

            if (Physics.Raycast(origin, Vector3.down, out RaycastHit hit, raycastHeight * 2f, groundMask, QueryTriggerInteraction.Ignore))
            {
                if (groundCollider == null || hit.collider == groundCollider)
                    return hit.point + Vector3.up * groundOffset;
            }
        }

        Vector3 fallback = new Vector3(areaCenter.x, transform.position.y, areaCenter.y);
        if (groundCollider != null)
            fallback.y = groundCollider.bounds.max.y;
        UnityEngine.Debug.LogWarning("랜덤 스폰 위치를 찾지 못해 고정 위치(Fallback)에 스폰합니다! 바닥 콜라이더 설정이나 레이어 마스크를 확인하세요.");
        return fallback + Vector3.up * groundOffset;

    }

    private GameObject CreatePickupObject(LiyifeiPowerupSettings settings)
    {
        GameObject pickupObject;
        if (settings.prefab != null)
        {
            pickupObject = Instantiate(settings.prefab);
        }
        else
        {
            pickupObject = CreateRuntimePlaceholder(settings.type);
        }

        pickupObject.name = $"Powerup_{settings.displayName}";
        return pickupObject;
    }

    private GameObject CreateRuntimePlaceholder(LiyifeiPowerupType type)
    {
        PrimitiveType primitive = type == LiyifeiPowerupType.Broom ? PrimitiveType.Capsule : PrimitiveType.Sphere;
        GameObject placeholder = GameObject.CreatePrimitive(primitive);
        placeholder.transform.localScale = GetPlaceholderScale(type);

        Renderer renderer = placeholder.GetComponent<Renderer>();
        if (renderer != null)
            renderer.material.color = GetPlaceholderColor(type);

        return placeholder;
    }

    private Vector3 GetPlaceholderScale(LiyifeiPowerupType type)
    {
        switch (type)
        {
            case LiyifeiPowerupType.Broom:
                return new Vector3(0.25f, 1.2f, 0.25f);
            case LiyifeiPowerupType.Stopwatch:
                return new Vector3(0.6f, 0.6f, 0.6f);
            default:
                return new Vector3(0.7f, 0.7f, 0.7f);
        }
    }

    private Color GetPlaceholderColor(LiyifeiPowerupType type)
    {
        switch (type)
        {
            case LiyifeiPowerupType.MagicStone:
                return new Color(0.35f, 0.65f, 1f);
            case LiyifeiPowerupType.Broom:
                return new Color(0.95f, 0.75f, 0.25f);
            case LiyifeiPowerupType.GhostMask:
                return new Color(0.85f, 0.85f, 0.95f);
            case LiyifeiPowerupType.PumpkinPie:
                return new Color(1f, 0.45f, 0.1f);
            case LiyifeiPowerupType.Stopwatch:
                return new Color(0.4f, 1f, 0.75f);
            default:
                return Color.white;
        }
    }

    private void RemoveMissingPickups()
    {
        for (int i = activePickups.Count - 1; i >= 0; i--)
        {
            if (activePickups[i] == null)
                activePickups.RemoveAt(i);
        }
    }
}
