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
        public int             iDamage;
        public int             iHitCount;
        public EAttackType     AttackType;
    }
}
