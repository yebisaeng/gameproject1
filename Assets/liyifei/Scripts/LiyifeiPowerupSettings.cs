using System;
using UnityEngine;

[Serializable]
public class LiyifeiPowerupSettings
{
    [Header("Common")]
    public LiyifeiPowerupType type;
    public string displayName;
    public GameObject prefab;
    [Min(0f)] public float spawnWeight = 1f;
    [Min(0.1f)] public float pickupRadius = 1.1f;

    [Header("Timed Buff")]
    [Min(0f)] public float duration = 15f;

    [Header("Magic Stone")]
    [Tooltip("Damage is read from Stat_Controller.damage and rounded to an int.")]
    [Min(1f)] public float damageMultiplier = 2f;

    [Header("Broom")]
    public bool destroyProjectileOnTargetHit = true;
    public string[] destroyTargetKeywords = { "Bat", "Spider", "Cobweb" };

    [Header("Ghost Mask")]
    [Min(1)] public int immunityCharges = 1;
    [Min(0f)] public float immunityDuration = 30f;
    [Min(0.1f)] public float damageSourceDestroyRadius = 2.5f;
    [Min(0.02f)] public float damageSourceScanInterval = 0.05f;
    public string[] damageSourceKeywords = { "Ghost", "Bat", "Spider" };

    [Header("Pumpkin Pie")]
    [Min(1f)] public float moveSpeedMultiplier = 1.5f;

    [Header("Stopwatch")]
    [Min(0f)] public float bonusSeconds = 30f;
}
