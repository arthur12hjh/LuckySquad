using System;

namespace Attack
{
    public enum EAttackType
    {
        Slash,      // 베기
        Pierce,     // 찌르기
        Strike,     // 타격
        Magic,      // 마법
        None,       // 없음
    }

    [Serializable]
    public struct SAttackData
    {
        public float           iDamage;
        public int             iHitCount;
        public EAttackType     AttackType;

        public SAttackData(float damage = 0, int HitCount = 1, EAttackType type = EAttackType.Slash)
        {
            iDamage = damage;
            iHitCount = HitCount;
            AttackType = type;
        }
    }
}
