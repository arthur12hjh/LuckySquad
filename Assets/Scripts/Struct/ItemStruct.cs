using Newtonsoft.Json;
using System;
using System.Collections.Generic;

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
    public enum EWeaponType
    {
        Projectile,
        Bounce,
        Rotation,
        Throw,
        HitScan,
        None
    }

    [Serializable]
    public struct Projectileinfo
    {
        public float fSpeed;
        public float fDamage;

        public Projectileinfo(float fspeed, float fdamage)
        {
            this.fSpeed = fspeed;
            this.fDamage = fdamage;
        }
    }

    [Serializable]
    public class LevelData
    {
        [JsonProperty("Particles")]
        public readonly List<int>               ParticleIDs;

        [JsonProperty("ItemEffects")]
        public readonly Dictionary<EEffectType, List<Effect>>  ItemEffects;

        public LevelData(
            List<int>    particleIDs = null,
            Dictionary<EEffectType, List<Effect>> effects = null)
        {
           
            this.ParticleIDs = particleIDs;
            this.ItemEffects = effects ?? new Dictionary<EEffectType, List<Effect>>();
        }
    }

    [Serializable]
    public class WeaponConfig
    {
        [JsonProperty("fDamage")]
        public int          fDamage;         // Damage

        [JsonProperty("iCount")]
        public int          iCount;          // Projectile Count

        [JsonProperty("iMaxLineCount")]
        public int          iMaxLineCount;   // Projectile Line Count

        [JsonProperty("fSpeed")]
        public float        fSpeed;          // Weapon Speed

        [JsonProperty("fRange")]
        public float        fRange;          // Weapon Range

        [JsonProperty("fInterval")]
        public float        fInterval;       // Weapon Attack Interval
    }

    #region Effect Data
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
            Instant,     // 즉시 데미지
            OverTime,    // 지속 데미지
            None,
        }

        [JsonProperty("EffectType")]
        public EDamageType  eDamageType;

        [JsonProperty("Amount")]
        public float fMagnitude;     // 값

        [JsonProperty("Duration")]
        public float fDuration;      // 지속 시간

        [JsonProperty("Interval")]
        public float fInterval;      // 틱 주기
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
    #endregion
    public class ItemData
    {
        [JsonProperty("id")]
        public readonly int                 iID;

        [JsonProperty("name")]
        public readonly string              szName;

        [JsonProperty("Max_Level")]
        public readonly int                 MaxLevel;

        [JsonProperty("type")]
        public readonly EItemType           eType;

        [JsonProperty("IconTexture")]
        public readonly string              IconName;

        [JsonProperty("Textures")]
        public readonly string              TextureName;

        [JsonProperty("LevelData")]
        public readonly List<LevelData>     LevelDatas;

        // 생성자를 통해서 딱 한 번만 세팅 가능
        public ItemData(int id = 1,
                        string name = "",
                        int maxLevel = 1,
                        EItemType type = EItemType.None,
                        string iconName = null,
                        string textureName = null,
                        List<LevelData> levelDatas = null)
        {
            this.iID = id;
            this.szName = name;
            this.MaxLevel = maxLevel;
            this.eType = type;
            this.IconName = iconName;
            this.TextureName = textureName;
            this.LevelDatas = levelDatas ?? new List<LevelData>();
          
        }
    }

    public class WeaponData : ItemData
    {
        [JsonProperty("WeaponType")]
        public readonly EWeaponType WeaponType;

        [JsonProperty("WeaponConfigs")]
        public readonly List<WeaponConfig> WeaponConfigs;

        // 생성자를 통해서 딱 한 번만 세팅 가능
        public WeaponData(int id = 1,
                        string name = "",
                        int maxLevel = 1,
                        EItemType type = EItemType.None,
                        string iconName = null,
                        string textureName = null,
                        List<LevelData> levelDatas = null,
                        EWeaponType weaponType = EWeaponType.None,
                        List<WeaponConfig> weaponConfigs = null) :
            base(id, name, maxLevel, type, iconName, textureName, levelDatas)
        {
            this.WeaponType = weaponType;
            this.WeaponConfigs = weaponConfigs ?? new List<WeaponConfig>();
        }
    }
}