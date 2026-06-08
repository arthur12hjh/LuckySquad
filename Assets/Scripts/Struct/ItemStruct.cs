using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Item
{
    public enum EItemType
    {
        Weapon,         // 무기
        Armor,          // 방어구
        Consumable,     // 소비
        None,           // 없음
    }

    public enum EEffectType
    {
        Damage,
        Heal,
        Buff,
        MoveSpeed,
        None,
    }

    [Serializable]
    public abstract class Effect
    {
        [JsonProperty("type")]
        public EEffectType EffectType;
    }

    [Serializable]
    public class DamageEffect : Effect
    {
        public enum EDamageType
        {
            Instant,     // 즉시 회복
            OverTime,    // 지속 회복
            None,
        }

        [JsonProperty("EffectType")]
        public EDamageType  eDamageType;

        [JsonProperty("Damage")]
        public float        fDamage;        // 값

        [JsonProperty("Duration")]
        public float        fDuration;      // 지속 시간

        [JsonProperty("TickPeriod")]
        public float        fInterval;      // 틱 주기

        [JsonProperty("Range")]
        public float fRange;                // 사거리
    }

    [Serializable]
    public class BuffEffect : Effect
    {
        public enum EBuffType
        {
            Damage,        // 공격력 증가
            Defense,       // 방어력 증가

            MoveSpeed,     // 이동속도 증가
            AttackSpeed,   // 공격속도 증가

            CriticalRate,  // 치명타 확률 증가
            CriticalDamage,// 치명타 피해 증가

            MaxHP,         // 최대 체력 증가
            MaxMP,         // 최대 마나 증가

            HPRegen,       // 체력 재생
            MPRegen,       // 마나 재생

            Cooldown,      // 쿨타임 감소
            None,
        }

        [JsonProperty("EffectType")]
        public EBuffType    eBuffType;

        [JsonProperty("Amount")]
        public float        fMagnitude;     // 값

        [JsonProperty("Duration")]
        public float        fDuration;      // 지속 시간

        [JsonProperty("Interval")]
        public float        fInterval;      // 틱 주기
    }

    [Serializable]
    public class SlowEffect : Effect
    {
        [JsonProperty("Amount")]
        public float        fMagnitude;     // 값

        [JsonProperty("Duration")]
        public float        fDuration;      // 지속 시간

        [JsonProperty("Interval")]
        public float        fInterval;      // 틱 주기
    }

    [Serializable]
    public class HealEffect : Effect
    {
        public enum EHealType
        {
            Instant,     // 즉시 회복
            OverTime,    // 지속 회복
            None,
        }

        [JsonProperty("EffectType")]
        public EHealType    eHealType;

        [JsonProperty("Amount")]
        public int          fAmount;        // 값

        [JsonProperty("Duration")]
        public float        fDuration;      // 지속 시간

        [JsonProperty("Interval")]
        public float        fInterval;      // 틱 주기
    }

    public enum ProjectileType { END };

    [Serializable]
    public struct Projectileinfo
    {
        [JsonProperty("Type")]
        public ProjectileType   Type;

        [JsonProperty("Damage")]
        public float            fDamage;

        [JsonProperty("Speed")]
        public float            fSpeed;
    }

    public struct ItemData
    {
        [JsonProperty("id")]
        public readonly int             iID;

        [JsonProperty("name")]
        public readonly string          szName;

        [JsonProperty("type")]
        public readonly EItemType       eType;

        [JsonProperty("effects")]
        public readonly List<Effect>    Effects;

        // 생성자를 통해서 딱 한 번만 세팅 가능
        public ItemData(int id = 1,
                        string name = "", 
                        EItemType type = EItemType.None,
                        List<Effect> effects = null)
        {
            this.iID = id;
            this.szName = name;
            this.eType = type;
            this.Effects = effects ?? new List<Effect>();
        }
    }
}