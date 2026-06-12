using UnityEngine;

[CreateAssetMenu(fileName = "PlayerStatsRef", menuName = "Scriptable Objects/PlayerStatsRef")]
public class PlayerStatsRef : EntityRef
{
    [SerializeField] public float MagneticPower;    // 자석
    [SerializeField] public float EXPMagnification;
    [SerializeField] public float ProjectileAmount; // 투사체 증가
    [SerializeField] public float AttackRange;      // 범위 증가
    [SerializeField] public float AttackSpeed;      // 공격속도 증가
    [SerializeField] public float ProjectileSpeed;  // 투사체 속도 증가
    [SerializeField] public float Duration;         // 투사체 지속시간 증가
}
