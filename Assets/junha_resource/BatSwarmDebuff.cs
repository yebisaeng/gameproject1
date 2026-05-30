using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatSwarmDebuff : DebuffBase
{
    [SerializeField] private GameObject batPrefab;
    [SerializeField] private Transform basket;
    [SerializeField] private float duration = 10f;

    [SerializeField] private int batCount = 12;
    [SerializeField] private float innerRadius = 2f;
    [SerializeField] private float outerRadius = 10f;
    [SerializeField] private float blockAngle = 60f;
    [SerializeField] private float heightOffset = 1f;
    [SerializeField] private float heightRange = 3f;

    protected override IEnumerator Run()
    {
        if (batPrefab == null || basket == null)
        {
            Debug.LogWarning($"[{debuffName}] batPrefab 또는 basket이 연결되지 않았습니다.");
            yield break;
        }

        Camera cam = Camera.main;
        if (cam == null)
        {
            Debug.LogWarning($"[{debuffName}] Camera.main을 찾을 수 없습니다.");
            yield break;
        }

        Vector3 toPlayer = cam.transform.position - basket.position;
        toPlayer.y = 0f;
        float centerAngle = Mathf.Atan2(toPlayer.z, toPlayer.x) * Mathf.Rad2Deg;

        List<GameObject> bats = new List<GameObject>();

        for (int i = 0; i < batCount; i++)
        {
            float angleDeg = centerAngle + Random.Range(-blockAngle * 0.5f, blockAngle * 0.5f);
            float angle = angleDeg * Mathf.Deg2Rad;
            float r = Random.Range(innerRadius, outerRadius);

            Vector3 spawnPos = basket.position;
            spawnPos.x += Mathf.Cos(angle) * r;
            spawnPos.z += Mathf.Sin(angle) * r;
            spawnPos.y += heightOffset + Random.Range(-heightRange, heightRange) * 0.5f;

            GameObject bat = Instantiate(batPrefab, spawnPos, Random.rotation);
            bats.Add(bat);
        }

        yield return new WaitForSeconds(duration);

        foreach (var bat in bats)
            if (bat != null) Destroy(bat);
    }
}