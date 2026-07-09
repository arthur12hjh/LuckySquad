using UnityEngine;

[CreateAssetMenu(fileName = "EfffectRef", menuName = "Scriptable Objects/EfffectRef")]
public class EfffectRef : ObjectPoolRef
{
    public enum EEffectSOType { 
        Effect,
        DamageEffect
    };


    public int              EffectID = 0;
    public EEffectSOType    _Type = EEffectSOType.Effect;
}
