using System.Collections;
using UnityEngine;

public abstract class DebuffBase : MonoBehaviour
{
    [Tooltip("콘솔 로그/구분용 이름")]
    public string debuffName = "Debuff";

    
    public bool IsActive { get; protected set; }


    public void Trigger()
    {
        if (IsActive) return;        
        StartCoroutine(RunInternal());
    }


    private IEnumerator RunInternal()
    {
        IsActive = true;
        yield return StartCoroutine(Run());
        IsActive = false;
    }


    protected abstract IEnumerator Run();
}