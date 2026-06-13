using System.Collections;
using System.Reflection;
using UnityEngine;

[DisallowMultipleComponent]
public class LiyifeiPlayerPowerupController : MonoBehaviour
{
    [Header("Scene References")]
    [SerializeField] private Stat_Controller statController;
    [SerializeField] private FirstPersonMovement movement;
    [SerializeField] private GameTimer gameTimer;
    [SerializeField] private Basket_Controller basket;

    [Header("Projectile Buff")]
    [SerializeField] private string projectileTag = "Stone";
    [SerializeField] private float projectileScanInterval = 0.1f;

    [Header("Ghost Mask")]
    [SerializeField] private float shieldRadius = 1.7f;
    [SerializeField] private bool destroyBlockedDamageSource = true;

    private static readonly FieldInfo CurrentTimeField =
        typeof(GameTimer).GetField("currentTime", BindingFlags.Instance | BindingFlags.NonPublic);

    private Coroutine damageRoutine;
    private Coroutine broomRoutine;
    private Coroutine speedRoutine;
    private Coroutine immunityRoutine;

    private int baseDamage;
    private int baseStatSpeed;
    private float baseWalkSpeed;
    private float baseRunSpeed;
    private bool baseStatsCaptured;

    private int immunityCharges;
    private float immunityExpiresAt;
    private string[] activeDamageSourceKeywords;
    private LiyifeiDamageShield shield;
    private Rigidbody playerRigidbody;

    private void Awake()
    {
        CacheReferences();
        CaptureBaseStats();
    }

    private void LateUpdate()
    {
        if (shield != null && shield.gameObject.activeSelf)
            shield.transform.position = transform.position;
    }

    private void OnTriggerEnter(Collider other)
    {
        TryConsumeDamageImmunity(other);
    }

    private void OnCollisionEnter(Collision collision)
    {
        TryConsumeDamageImmunity(collision.collider);
    }

    public void ApplyPowerup(LiyifeiPowerupSettings settings)
    {
        if (settings == null) return;

        CacheReferences();
        CaptureBaseStats();

        switch (settings.type)
        {
            case LiyifeiPowerupType.MagicStone:
                StartDamageBuff(settings);
                break;
            case LiyifeiPowerupType.Broom:
                StartBroomBuff(settings);
                break;
            case LiyifeiPowerupType.GhostMask:
                StartDamageImmunity(settings);
                break;
            case LiyifeiPowerupType.PumpkinPie:
                StartSpeedBuff(settings);
                break;
            case LiyifeiPowerupType.Stopwatch:
                AddTimerSeconds(settings.bonusSeconds);
                break;
        }

        Debug.Log($"【道具】已拾取：{GetPowerupDebugName(settings.type)}。");
    }

    private string GetPowerupDebugName(LiyifeiPowerupType type)
    {
        switch (type)
        {
            case LiyifeiPowerupType.MagicStone:
                return "魔法石";
            case LiyifeiPowerupType.Broom:
                return "扫帚";
            case LiyifeiPowerupType.GhostMask:
                return "GhostMask（幽灵面具）";
            case LiyifeiPowerupType.PumpkinPie:
                return "南瓜派";
            case LiyifeiPowerupType.Stopwatch:
                return "秒表";
            default:
                return type.ToString();
        }
    }

    public bool TryConsumeDamageImmunity(Collider source)
    {
        if (immunityCharges <= 0 || Time.time > immunityExpiresAt || source == null)
            return false;

        Transform damageSource = FindKeywordTarget(source.transform, activeDamageSourceKeywords);
        return TryConsumeDamageSource(damageSource);
    }

    public bool TryConsumeHazardImmunity(string hazardName, params string[] hazardKeywords)
    {
        if (immunityCharges <= 0 || hazardKeywords == null || hazardKeywords.Length == 0)
            return false;

        if (Time.time > immunityExpiresAt)
        {
            DisableShield();
            return false;
        }

        if (!MatchesActiveDamageKeyword(hazardKeywords))
            return false;

        immunityCharges--;
        if (string.IsNullOrWhiteSpace(hazardName)) hazardName = "危险效果";
        Debug.Log($"【GhostMask】已阻挡一次{hazardName}。剩余免伤次数：{immunityCharges}。");

        if (immunityCharges <= 0)
            DisableShield();

        return true;
    }

    private bool TryConsumeDamageSource(Transform damageSource)
    {
        if (immunityCharges <= 0 || Time.time > immunityExpiresAt || damageSource == null)
            return false;

        if (!IsDestroyableDamageSource(damageSource))
            return false;

        int scoreBeforeHit = basket != null ? basket.score : 0;
        immunityCharges--;
        Debug.Log($"【GhostMask】已阻挡一次伤害来源：{damageSource.name}。剩余免伤次数：{immunityCharges}。");

        if (destroyBlockedDamageSource)
            RemoveDamageSource(damageSource.gameObject);

        StartCoroutine(CancelBlockedDamageEffects(scoreBeforeHit));

        if (immunityCharges <= 0)
            DisableShield();

        return true;
    }

    private bool IsDestroyableDamageSource(Transform damageSource)
    {
        if (damageSource == null || damageSource == transform || damageSource.IsChildOf(transform))
            return false;

        GameObject sourceObject = damageSource.gameObject;
        if (sourceObject.CompareTag("Player") || sourceObject.CompareTag("Stone"))
            return false;

        if (sourceObject.GetComponent<DebuffBase>() != null)
            return false;

        return sourceObject.GetComponentInChildren<Renderer>() != null ||
               sourceObject.GetComponentInChildren<Collider>() != null;
    }

    private bool MatchesActiveDamageKeyword(string[] hazardKeywords)
    {
        if (activeDamageSourceKeywords == null) return false;

        for (int i = 0; i < activeDamageSourceKeywords.Length; i++)
        {
            string activeKeyword = activeDamageSourceKeywords[i];
            if (string.IsNullOrWhiteSpace(activeKeyword)) continue;

            for (int j = 0; j < hazardKeywords.Length; j++)
            {
                if (KeywordMatches(activeKeyword, hazardKeywords[j]))
                    return true;
            }
        }

        return false;
    }

    private bool KeywordMatches(string activeKeyword, string hazardKeyword)
    {
        if (string.IsNullOrWhiteSpace(activeKeyword) || string.IsNullOrWhiteSpace(hazardKeyword))
            return false;

        return activeKeyword.IndexOf(hazardKeyword, System.StringComparison.OrdinalIgnoreCase) >= 0 ||
               hazardKeyword.IndexOf(activeKeyword, System.StringComparison.OrdinalIgnoreCase) >= 0;
    }

    private void RemoveDamageSource(GameObject damageSource)
    {
        if (damageSource == null) return;

        damageSource.SetActive(false);
        Destroy(damageSource);
    }

    private void StartDamageBuff(LiyifeiPowerupSettings settings)
    {
        if (damageRoutine != null) StopCoroutine(damageRoutine);
        damageRoutine = StartCoroutine(DamageBuffRoutine(settings.damageMultiplier, settings.duration));
    }

    private IEnumerator DamageBuffRoutine(float multiplier, float duration)
    {
        if (statController != null)
            statController.damage = Mathf.Max(1, Mathf.RoundToInt(baseDamage * multiplier));

        yield return new WaitForSeconds(duration);

        if (statController != null)
            statController.damage = baseDamage;

        damageRoutine = null;
    }

    private void StartBroomBuff(LiyifeiPowerupSettings settings)
    {
        int clearedWebs = SpiderWebDebuff.ClearActiveWebs();
        if (clearedWebs > 0)
            Debug.Log($"【扫帚】已清除脸上的蜘蛛网，数量：{clearedWebs}。");

        if (broomRoutine != null) StopCoroutine(broomRoutine);
        broomRoutine = StartCoroutine(BroomBuffRoutine(settings));
    }

    private IEnumerator BroomBuffRoutine(LiyifeiPowerupSettings settings)
    {
        float endsAt = Time.time + settings.duration;
        WaitForSeconds wait = new WaitForSeconds(projectileScanInterval);

        while (Time.time < endsAt)
        {
            EnhanceActiveProjectiles(settings.destroyTargetKeywords, endsAt, settings.destroyProjectileOnTargetHit);
            yield return wait;
        }

        broomRoutine = null;
    }

    private void StartSpeedBuff(LiyifeiPowerupSettings settings)
    {
        if (speedRoutine != null) StopCoroutine(speedRoutine);
        speedRoutine = StartCoroutine(SpeedBuffRoutine(settings.moveSpeedMultiplier, settings.duration));
    }

    private IEnumerator SpeedBuffRoutine(float multiplier, float duration)
    {
        if (movement != null)
        {
            movement.speed = baseWalkSpeed * multiplier;
            movement.runSpeed = baseRunSpeed * multiplier;
        }

        if (statController != null)
            statController.speed = Mathf.Max(1, Mathf.RoundToInt(baseStatSpeed * multiplier));

        yield return new WaitForSeconds(duration);

        if (movement != null)
        {
            movement.speed = baseWalkSpeed;
            movement.runSpeed = baseRunSpeed;
        }

        if (statController != null)
            statController.speed = baseStatSpeed;

        speedRoutine = null;
    }

    private void StartDamageImmunity(LiyifeiPowerupSettings settings)
    {
        if (immunityRoutine != null) StopCoroutine(immunityRoutine);
        immunityRoutine = StartCoroutine(DamageImmunityRoutine(settings));
    }

    private IEnumerator DamageImmunityRoutine(LiyifeiPowerupSettings settings)
    {
        immunityCharges = Mathf.Max(1, settings.immunityCharges);
        immunityExpiresAt = Time.time + settings.immunityDuration;
        activeDamageSourceKeywords = settings.damageSourceKeywords;
        EnableShield(settings.damageSourceDestroyRadius);
        Debug.Log($"【GhostMask】已触发。免伤次数：{immunityCharges}，持续时间：{settings.immunityDuration:0.#}秒，可阻挡：{GetKeywordDebugNames(activeDamageSourceKeywords)}。");

        WaitForSeconds wait = new WaitForSeconds(Mathf.Max(0.02f, settings.damageSourceScanInterval));

        while (immunityCharges > 0 && Time.time < immunityExpiresAt)
        {
            TryDestroyNearbyDamageSources(settings.damageSourceDestroyRadius);
            yield return wait;
        }

        DisableShield();
        immunityRoutine = null;
    }

    private void TryDestroyNearbyDamageSources(float radius)
    {
        Transform[] transforms = FindObjectsByType<Transform>(FindObjectsSortMode.None);
        float sqrRadius = radius * radius;

        for (int i = 0; i < transforms.Length; i++)
        {
            Transform candidate = transforms[i];
            if (candidate == null || candidate == transform || candidate.IsChildOf(transform)) continue;
            if ((candidate.position - transform.position).sqrMagnitude > sqrRadius) continue;

            Transform damageSource = FindKeywordTarget(candidate, activeDamageSourceKeywords);
            if (TryConsumeDamageSource(damageSource))
                return;
        }
    }

    private string GetKeywordDebugNames(string[] keywords)
    {
        if (keywords == null || keywords.Length == 0)
            return "无";

        string[] names = new string[keywords.Length];
        for (int i = 0; i < keywords.Length; i++)
            names[i] = GetKeywordDebugName(keywords[i]);

        return string.Join("、", names);
    }

    private string GetKeywordDebugName(string keyword)
    {
        if (string.IsNullOrWhiteSpace(keyword))
            return "空";

        if (keyword.IndexOf("Ghost", System.StringComparison.OrdinalIgnoreCase) >= 0)
            return "幽灵";
        if (keyword.IndexOf("Bat", System.StringComparison.OrdinalIgnoreCase) >= 0)
            return "蝙蝠";
        if (keyword.IndexOf("Spider", System.StringComparison.OrdinalIgnoreCase) >= 0)
            return "蜘蛛";
        if (keyword.IndexOf("Cobweb", System.StringComparison.OrdinalIgnoreCase) >= 0)
            return "蜘蛛网";

        return keyword;
    }

    private IEnumerator CancelBlockedDamageEffects(int scoreBeforeHit)
    {
        yield return null;

        if (basket != null && basket.score < scoreBeforeHit)
            basket.score = scoreBeforeHit;

        if (movement != null)
            movement.enabled = true;

        if (playerRigidbody != null)
            playerRigidbody.linearVelocity = Vector3.zero;
    }

    private void AddTimerSeconds(float seconds)
    {
        if (gameTimer == null || CurrentTimeField == null) return;

        float currentTime = (float)CurrentTimeField.GetValue(gameTimer);
        CurrentTimeField.SetValue(gameTimer, currentTime + seconds);
    }

    private void EnhanceActiveProjectiles(string[] keywords, float endsAt, bool destroyProjectileOnHit)
    {
        Stone_Controller[] stones = FindObjectsByType<Stone_Controller>(FindObjectsSortMode.None);
        for (int i = 0; i < stones.Length; i++)
            ConfigureProjectile(stones[i].gameObject, keywords, endsAt, destroyProjectileOnHit);

        try
        {
            GameObject[] taggedProjectiles = GameObject.FindGameObjectsWithTag(projectileTag);
            for (int i = 0; i < taggedProjectiles.Length; i++)
                ConfigureProjectile(taggedProjectiles[i], keywords, endsAt, destroyProjectileOnHit);
        }
        catch (UnityException)
        {
            // The tag is configurable; ignore projects where it is not defined.
        }
    }

    private void ConfigureProjectile(GameObject projectile, string[] keywords, float endsAt, bool destroyProjectileOnHit)
    {
        if (projectile == null) return;

        LiyifeiProjectilePowerupHit hit = projectile.GetComponent<LiyifeiProjectilePowerupHit>();
        if (hit == null) hit = projectile.AddComponent<LiyifeiProjectilePowerupHit>();
        hit.Configure(keywords, endsAt, destroyProjectileOnHit);
    }

    private Transform FindKeywordTarget(Transform start, string[] keywords)
    {
        if (keywords == null) return null;

        Transform current = start;
        while (current != null)
        {
            for (int i = 0; i < keywords.Length; i++)
            {
                string keyword = keywords[i];
                if (string.IsNullOrWhiteSpace(keyword)) continue;

                if (current.name.IndexOf(keyword, System.StringComparison.OrdinalIgnoreCase) >= 0)
                    return current;

                if (current.gameObject.tag.IndexOf(keyword, System.StringComparison.OrdinalIgnoreCase) >= 0)
                    return current;
            }

            current = current.parent;
        }

        return null;
    }

    private void EnableShield(float radius)
    {
        if (shield == null)
        {
            GameObject shieldObject = new GameObject("LiyifeiGhostMaskShield");
            shield = shieldObject.AddComponent<LiyifeiDamageShield>();
        }

        shield.transform.position = transform.position;
        shield.Configure(this, Mathf.Max(shieldRadius, radius));
        shield.gameObject.SetActive(true);
    }

    private void DisableShield()
    {
        immunityCharges = 0;
        if (shield != null)
            shield.gameObject.SetActive(false);
    }

    private void CacheReferences()
    {
        if (statController == null) statController = GetComponent<Stat_Controller>();
        if (movement == null) movement = GetComponent<FirstPersonMovement>();
        if (playerRigidbody == null) playerRigidbody = GetComponent<Rigidbody>();
        if (basket == null) basket = FindFirstObjectByType<Basket_Controller>();

        if (gameTimer == null)
        {
            GameTimer[] timers = FindObjectsByType<GameTimer>(FindObjectsSortMode.None);
            for (int i = 0; i < timers.Length; i++)
            {
                if (timers[i] != null && timers[i].timerText != null)
                {
                    gameTimer = timers[i];
                    break;
                }
            }
        }
    }

    private void CaptureBaseStats()
    {
        if (baseStatsCaptured) return;

        baseDamage = statController != null ? statController.damage : 1;
        baseStatSpeed = statController != null ? statController.speed : 1;
        baseWalkSpeed = movement != null ? movement.speed : 5f;
        baseRunSpeed = movement != null ? movement.runSpeed : 9f;
        baseStatsCaptured = true;
    }
}
