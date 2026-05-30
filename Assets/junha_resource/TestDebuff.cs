using System.Collections;
using UnityEngine;

public class TestDebuff : DebuffBase
{
    [SerializeField] private float duration = 3f;

    protected override IEnumerator Run()
    {
        Debug.Log($"[{debuffName}] 시작!");

        float t = 0f;
        while (t < duration)
        {
            Debug.Log($"[{debuffName}] 실행 중... {t:0.0}초");
            t += 1f;
            yield return new WaitForSeconds(1f);
        }

        Debug.Log($"[{debuffName}] 종료!");
    }
}