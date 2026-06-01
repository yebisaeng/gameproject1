using System.Collections;
using UnityEngine;

public class ChargingGhostDebuff : DebuffBase
{
    [SerializeField] private GameObject ghostPrefab;
    [SerializeField] private float minSpawnDistance = 15f;
    [SerializeField] private float maxSpawnDistance = 25f;
    [SerializeField] private float heightOffset = 0f;
    [SerializeField] private float mapSize = 100f;
    [SerializeField] private float edgeMargin = 2f;

    [SerializeField] private float moveSpeed = 6f;
    [SerializeField] private float knockbackForce = 12f;
    [SerializeField] private float stunDuration = 0.5f;
    [SerializeField] private float lifeTime = 12f;

    protected override IEnumerator Run()
    {
        if (ghostPrefab == null)
        {
            Debug.LogWarning($"[{debuffName}] ghostPrefab이 연결되지 않았습니다.");
            yield break;
        }

        Camera cam = Camera.main;
        if (cam == null)
        {
            Debug.LogWarning($"[{debuffName}] Camera.main을 찾을 수 없습니다.");
            yield break;
        }

        float angle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
        float dist = Random.Range(minSpawnDistance, maxSpawnDistance);

        Vector3 playerPos = cam.transform.position;
        Vector3 spawnPos = playerPos;
        spawnPos.x += Mathf.Cos(angle) * dist;
        spawnPos.z += Mathf.Sin(angle) * dist;
        spawnPos.y = playerPos.y + heightOffset;

        float limit = mapSize * 0.5f - edgeMargin;
        spawnPos.x = Mathf.Clamp(spawnPos.x, -limit, limit);
        spawnPos.z = Mathf.Clamp(spawnPos.z, -limit, limit);

        GameObject ghostObj = Instantiate(ghostPrefab, spawnPos, Quaternion.identity);

        ChargingGhost ghost = ghostObj.GetComponent<ChargingGhost>();
        if (ghost == null) ghost = ghostObj.AddComponent<ChargingGhost>();
        ghost.Init(cam.transform, moveSpeed, knockbackForce, stunDuration);

        yield return new WaitForSeconds(lifeTime);

        if (ghostObj != null) Destroy(ghostObj);
    }
}