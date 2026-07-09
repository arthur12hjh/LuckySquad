using System;
using UnityEngine;
public class EffectBsae : MonoBehaviour, IPoolable
{
    protected Action       _releaseAct;

    public void OnSpawn(Action releaseSelf)
    {
        _releaseAct = releaseSelf;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        var ParticleSys = GetComponent<ParticleSystem>();
    }

    private void OnParticleSystemStopped()
    {
        Debug.Log($"{gameObject.name} 파티클 재생이 끝났습니다!");
        _releaseAct.Invoke();
    }
}
