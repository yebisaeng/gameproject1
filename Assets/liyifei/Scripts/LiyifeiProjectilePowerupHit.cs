using System;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class LiyifeiProjectilePowerupHit : MonoBehaviour
{
    private string[] targetKeywords = Array.Empty<string>();
    private float expiresAt;
    private bool destroyProjectileOnHit = true;

    public void Configure(string[] keywords, float activeUntil, bool destroyProjectile)
    {
        targetKeywords = keywords ?? Array.Empty<string>();
        expiresAt = activeUntil;
        destroyProjectileOnHit = destroyProjectile;
    }

    private void OnCollisionEnter(Collision collision)
    {
        TryDestroyTarget(collision.collider);
    }

    private void OnTriggerEnter(Collider other)
    {
        TryDestroyTarget(other);
    }

    private void TryDestroyTarget(Collider other)
    {
        if (Time.time > expiresAt || other == null) return;

        Transform target = FindMatchingTarget(other.transform);
        if (target == null) return;

        string matchedKeyword = GetMatchingKeyword(target.gameObject);
        if (!string.IsNullOrEmpty(matchedKeyword))
            DestroyAllMatchingTargets(matchedKeyword);
        else
            RemoveTarget(target.gameObject);

        if (destroyProjectileOnHit) RemoveTarget(gameObject);
    }

    private Transform FindMatchingTarget(Transform start)
    {
        Transform current = start;
        while (current != null)
        {
            if (Matches(current.gameObject)) return current;
            current = current.parent;
        }

        return null;
    }

    private void DestroyAllMatchingTargets(string matchedKeyword)
    {
        HashSet<GameObject> destroyTargets = new HashSet<GameObject>();
        Transform[] transforms = FindObjectsByType<Transform>(FindObjectsSortMode.None);

        for (int i = 0; i < transforms.Length; i++)
        {
            GameObject candidate = transforms[i].gameObject;
            if (!MatchesKeyword(candidate, matchedKeyword)) continue;
            if (!IsDestroyableTarget(candidate)) continue;

            GameObject destroyRoot = FindDestroyRoot(candidate.transform, matchedKeyword);
            if (destroyRoot != null)
                destroyTargets.Add(destroyRoot);
        }

        foreach (GameObject destroyTarget in destroyTargets)
            RemoveTarget(destroyTarget);
    }

    private GameObject FindDestroyRoot(Transform start, string matchedKeyword)
    {
        Transform root = start;
        Transform current = start.parent;

        while (current != null && MatchesKeyword(current.gameObject, matchedKeyword) && IsDestroyableTarget(current.gameObject))
        {
            root = current;
            current = current.parent;
        }

        return root.gameObject;
    }

    private bool Matches(GameObject candidate)
    {
        if (candidate == null || candidate == gameObject) return false;
        if (candidate.CompareTag("Player") || candidate.CompareTag("Stone")) return false;

        for (int i = 0; i < targetKeywords.Length; i++)
        {
            string keyword = targetKeywords[i];
            if (string.IsNullOrWhiteSpace(keyword)) continue;

            if (candidate.name.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0)
                return true;

            if (candidate.tag.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0)
                return true;
        }

        return false;
    }

    private string GetMatchingKeyword(GameObject candidate)
    {
        if (candidate == null) return null;

        for (int i = 0; i < targetKeywords.Length; i++)
        {
            string keyword = targetKeywords[i];
            if (string.IsNullOrWhiteSpace(keyword)) continue;

            if (MatchesKeyword(candidate, keyword))
                return keyword;
        }

        return null;
    }

    private bool MatchesKeyword(GameObject candidate, string keyword)
    {
        if (candidate == null || string.IsNullOrWhiteSpace(keyword)) return false;

        if (candidate.name.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0)
            return true;

        return candidate.tag.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0;
    }

    private bool IsDestroyableTarget(GameObject candidate)
    {
        if (candidate == null || candidate == gameObject) return false;
        if (candidate.CompareTag("Player") || candidate.CompareTag("Stone")) return false;
        if (candidate.GetComponent<DebuffBase>() != null) return false;
        if (candidate.GetComponent<LiyifeiPowerupSpawner>() != null) return false;
        if (candidate.GetComponent<LiyifeiPowerupPickup>() != null) return false;

        return candidate.GetComponentInChildren<Renderer>() != null ||
               candidate.GetComponentInChildren<Collider>() != null;
    }

    private void RemoveTarget(GameObject target)
    {
        if (target == null) return;

        target.SetActive(false);
        Destroy(target);
    }
}
