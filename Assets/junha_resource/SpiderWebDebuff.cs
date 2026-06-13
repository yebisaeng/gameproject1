using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderWebDebuff : DebuffBase
{
    [SerializeField] private GameObject cobwebPrefab;
    [SerializeField] private float duration = 10f;

    [SerializeField] private float distance = 0.5f;
    [SerializeField] private int columns = 3;
    [SerializeField] private int rows = 3;
    [SerializeField] private float spreadX = 0.6f;
    [SerializeField] private float spreadY = 0.4f;

    [SerializeField] private float positionJitter = 0.08f;
    [SerializeField] private float minScale = 0.15f;
    [SerializeField] private float maxScale = 0.25f;
    [SerializeField] private float zJitter = 0.05f;

    private static readonly List<GameObject> activeWebs = new List<GameObject>();

    public static int ClearActiveWebs()
    {
        int clearedCount = 0;

        for (int i = activeWebs.Count - 1; i >= 0; i--)
        {
            GameObject web = activeWebs[i];
            activeWebs.RemoveAt(i);

            if (web == null) continue;

            web.SetActive(false);
            UnityEngine.Object.Destroy(web);
            clearedCount++;
        }

        return clearedCount;
    }

    protected override IEnumerator Run()
    {
        Camera cam = Camera.main;
        if (cam == null || cobwebPrefab == null)
        {
            Debug.LogWarning($"[{debuffName}] 카메라 또는 프리팹이 없습니다.");
            yield break;
        }

        if (TryBlockWithGhostMask(cam))
        {
            Debug.Log("\u3010GhostMask\u3011\u5df2\u963b\u6b62\u8718\u86db\u7f51\u751f\u6210\u5728\u6444\u50cf\u673a\u524d\u3002");
            yield break;
        }

        List<GameObject> webs = new List<GameObject>();

        for (int x = 0; x < columns; x++)
        {
            for (int y = 0; y < rows; y++)
            {
                float u = columns > 1 ? (float)x / (columns - 1) - 0.5f : 0f;
                float v = rows > 1 ? (float)y / (rows - 1) - 0.5f : 0f;

                float posX = u * spreadX + Random.Range(-positionJitter, positionJitter);
                float posY = v * spreadY + Random.Range(-positionJitter, positionJitter);
                float posZ = distance + Random.Range(-zJitter, zJitter);

                GameObject web = Instantiate(cobwebPrefab, cam.transform);
                web.transform.localPosition = new Vector3(posX, posY, posZ);
                web.transform.localEulerAngles = new Vector3(0f, 0f, Random.Range(0f, 360f));
                web.transform.localScale = Vector3.one * Random.Range(minScale, maxScale);

                webs.Add(web);
                activeWebs.Add(web);
            }
        }

        yield return new WaitForSeconds(duration);

        foreach (var web in webs)
        {
            if (web == null) continue;

            activeWebs.Remove(web);
            Destroy(web);
        }
    }

    private bool TryBlockWithGhostMask(Camera cam)
    {
        LiyifeiPlayerPowerupController powerups = cam.GetComponentInParent<LiyifeiPlayerPowerupController>();
        if (powerups == null)
            powerups = FindFirstObjectByType<LiyifeiPlayerPowerupController>();

        return powerups != null &&
               powerups.TryConsumeHazardImmunity("\u8718\u86db\u7f51", "SpiderWeb", "Spider", "Cobweb");
    }
}
