using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class WindDebuff : DebuffBase
{
    [SerializeField] private float duration = 15f;
    [SerializeField] private float windForce = 8f;
    [SerializeField] private string ballTag = "Stone";

    [SerializeField] private Canvas targetCanvas;
    [SerializeField] private Vector2 arrowPosition = new Vector2(120f, -120f);
    [SerializeField] private Vector2 arrowSize = new Vector2(100f, 100f);
    [SerializeField] private Color arrowColor = Color.white;
    [SerializeField] private Sprite arrowSprite;
    [SerializeField] private float rotationOffset = -90f;

    private RectTransform arrowRect;

    protected override IEnumerator Run()
    {
        float angle = Random.Range(0f, 360f);
        Vector3 windDir = new Vector3(
            Mathf.Cos(angle * Mathf.Deg2Rad),
            0f,
            Mathf.Sin(angle * Mathf.Deg2Rad));

        CreateArrowIfNeeded();
        if (arrowRect != null) arrowRect.gameObject.SetActive(true);

        Camera cam = Camera.main;

        float t = 0f;
        while (t < duration)
        {
            GameObject[] balls = GameObject.FindGameObjectsWithTag(ballTag);
            foreach (var ball in balls)
            {
                if (ball == null) continue;
                if (ball.TryGetComponent<Rigidbody>(out var rb))
                    rb.AddForce(windDir * windForce, ForceMode.Force);
            }

            if (arrowRect != null && cam != null)
                UpdateArrowRotation(cam, windDir);

            t += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        if (arrowRect != null) arrowRect.gameObject.SetActive(false);
    }

    private void CreateArrowIfNeeded()
    {
        if (arrowRect != null) return;
        if (targetCanvas == null)
        {
            Debug.LogWarning($"[{debuffName}] targetCanvas가 연결되지 않았습니다.");
            return;
        }

        GameObject arrowObj = new GameObject("WindArrow", typeof(Image));
        arrowObj.transform.SetParent(targetCanvas.transform, false);

        Image img = arrowObj.GetComponent<Image>();
        img.color = arrowColor;
        img.sprite = arrowSprite;
        img.preserveAspect = true;

        arrowRect = arrowObj.GetComponent<RectTransform>();
        arrowRect.anchorMin = new Vector2(0f, 1f);
        arrowRect.anchorMax = new Vector2(0f, 1f);
        arrowRect.pivot = new Vector2(0.5f, 0.5f);
        arrowRect.anchoredPosition = arrowPosition;
        arrowRect.sizeDelta = arrowSize;
    }

    private void UpdateArrowRotation(Camera cam, Vector3 worldDir)
    {
        Vector3 fwd = cam.transform.forward; fwd.y = 0f; fwd.Normalize();
        Vector3 right = cam.transform.right; right.y = 0f; right.Normalize();

        float x = Vector3.Dot(worldDir, right);
        float y = Vector3.Dot(worldDir, fwd);
        float screenAngle = Mathf.Atan2(x, y) * Mathf.Rad2Deg;

        arrowRect.localEulerAngles = new Vector3(0f, 0f, -screenAngle + rotationOffset);
    }
}