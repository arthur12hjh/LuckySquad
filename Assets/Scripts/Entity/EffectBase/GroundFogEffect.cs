using Attack;
using Item;
using System.Collections;
using UnityEngine;

public class GroundFogEffect : EffectBsae
{
    [SerializeField] LayerMask  _layerMask;
    Projectileinfo              _info;
    DamageEffect                _damageEffect = null;

    public void Initailize( Projectileinfo  info,
                            DamageEffect    damageEffect)
    {
        _info = info;
        _damageEffect = damageEffect;

        StartCoroutine(RepeatActionCoroutine());
    }

    IEnumerator RepeatActionCoroutine()
    {
        float Duration = _damageEffect.fDuration;
        while (Duration > 0)
        {
            Attack();
            Duration -= _damageEffect.fInterval;
            yield return new WaitForSeconds(_damageEffect.fInterval);
        }

        _releaseAct?.Invoke();
        yield return null;
    }

    void Attack()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, _info._config.fRange, _layerMask.value);
        foreach (var hit in hits)
        {
            hit.GetComponent<Monster>()?.Damaged(gameObject, new SAttackData(20));
        }
    }
}
