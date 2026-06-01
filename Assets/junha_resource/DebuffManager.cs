using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebuffManager : MonoBehaviour
{
    [Header("참조")]
    [SerializeField] private GameTimer gameTimer;

    [Header("디버프 목록 (인스펙터에서 연결)")]
    [SerializeField] private List<DebuffBase> debuffs = new List<DebuffBase>();

    [Header("타이밍 (초)")]
    [SerializeField] private float startDelay = 30f; // 14:30 시점 (시작 30초 후)
    [SerializeField] private float interval = 30f;

    [Header("옵션")]
    [SerializeField] private bool avoidRepeat = true; // 같은 디버프 연속 방지

    private int lastIndex = -1;

    private void Start()
    {
        StartCoroutine(DebuffLoop());
    }

    private IEnumerator DebuffLoop()
    {
        yield return new WaitForSeconds(startDelay);

        while (gameTimer != null && gameTimer.IsRunning)
        {
            ActivateRandomDebuff();
            yield return new WaitForSeconds(interval);
        }

        Debug.Log("[DebuffManager] 게임 종료 — 디버프 루프 정지");
    }

    private void ActivateRandomDebuff()
    {
        // 지금 실행 중이 아닌 디버프만 후보로 모은다
        List<int> candidates = new List<int>();
        for (int i = 0; i < debuffs.Count; i++)
        {
            if (debuffs[i] == null || debuffs[i].IsActive) continue;
            if (avoidRepeat && i == lastIndex && debuffs.Count > 1) continue;
            candidates.Add(i);
        }

        if (candidates.Count == 0)
        {
            Debug.Log("[DebuffManager] 발동 가능한 디버프 없음 — 이번 턴 스킵");
            return;
        }

        int pick = candidates[Random.Range(0, candidates.Count)];
        lastIndex = pick;

        debuffs[pick].Trigger();
        Debug.Log($"[DebuffManager] 디버프 발동 → {debuffs[pick].debuffName}  (남은 시간 {gameTimer.RemainingTime:0}초)");
    }
}