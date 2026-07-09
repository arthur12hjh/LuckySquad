using DG.Tweening;
using UnityEngine;

public class JumpState : BaseState
{
    // 스킬 연출 단계
    private enum JumpPhase
    {
        Ready,      // 점프 준비. 부르르 떨면서 예고하는 중
        Shadow,     // 그림자 형태로 플레이어를 쫓아가는 중
        Slam,       // 착지 지점 상공에서 내려찍는 중
        Recovery,   // 착지 후 경직
        Finished,   // 스킬 종료
    }

    private const float ReadyDuration = 1f;         // 점프 준비 시간
    private const float ShakeStrength = 0.08f;      // 떨림 폭
    private const int ShakeVibrato = 40;            // 떨림 빈도
    private const float ShadowDuration = 3f;        // 그림자 상태 유지 시간
    private const float ShadowSpeedModifier = 1.5f; // 그림자 이동 속도 배율
    private const float SlamHeight = 8f;            // 내려찍기를 시작하는 높이
    private const float SlamDuration = 0.25f;       // 내려찍는 시간
    private const float RecoveryDuration = 3f;      // 착지 후 경직 시간

    private OtakuBoss _boss;
    private JumpPhase _phase;
    private float _phaseTimer;
    private Vector2 _readyStartPos;
    private Vector2 _slamTargetPos;
    private Tween _shakeTween;

    public JumpState(OtakuBoss boss) : base(boss)
    {
        _boss = boss;
    }

    public override void OnStateEnter()
    {
        _boss.StartJumpCooldown();

        _boss.PlayReadyToJump();
        _readyStartPos = _boss.Position;
        _shakeTween = _boss.transform.DOShakePosition(ReadyDuration, ShakeStrength, ShakeVibrato);

        _phase = JumpPhase.Ready;
        _phaseTimer = 0f;
        _slamTargetPos = _boss.Position;
    }

    public override void OnStateUpdate()
    {
        
    }

    public override void OnStateFixedUpdate()
    {
        _phaseTimer += Time.fixedDeltaTime;

        switch (_phase)
        {
            case JumpPhase.Ready:
                UpdateReady();
                break;
            case JumpPhase.Shadow:
                UpdateShadow();
                break;
            case JumpPhase.Slam:
                UpdateSlam();
                break;
            case JumpPhase.Recovery:
                UpdateRecovery();
                break;
            case JumpPhase.Finished:
                break;
        }
    }

    public override void OnStateExit()
    {
        KillShake();
        _boss.SetJumpFlyState(false);
        _boss.SetLayerByJumpFly(true);
    }

    private void ChangePhase(JumpPhase nextPhase)
    {
        _phase = nextPhase;
        _phaseTimer = 0f;
    }

    private void KillShake()
    {
        if (_shakeTween != null && _shakeTween.IsActive())
            _shakeTween.Kill();

        _shakeTween = null;
    }

    private void UpdateReady()
    {
        if (_phaseTimer >= ReadyDuration)
        {
            KillShake();
            _boss.Teleport(_readyStartPos);
            _boss.SetLayerByJumpFly(false);
            _boss.SetJumpFlyState(true);
            ChangePhase(JumpPhase.Shadow);
        }
    }

    private void UpdateShadow()
    {
        Vector2 nextPos = Vector2.MoveTowards(
            _boss.Position,
            _boss.PlayerPosition,
            _boss.Speed * ShadowSpeedModifier * Time.fixedDeltaTime);
        _boss.JumpTo(nextPos);

        if (_phaseTimer >= ShadowDuration)
        {
            _slamTargetPos = _boss.Position;
            _boss.Teleport(_slamTargetPos + Vector2.up * SlamHeight);
            _boss.SetJumpFlyState(false);
            ChangePhase(JumpPhase.Slam);
        }
    }

    private void UpdateSlam()
    {
        float t = Mathf.Clamp01(_phaseTimer / SlamDuration);
        Vector2 startPos = _slamTargetPos + Vector2.up * SlamHeight;
        _boss.JumpTo(Vector2.Lerp(startPos, _slamTargetPos, t));

        if (_phaseTimer >= SlamDuration)
        {
            _boss.SetLayerByJumpFly(true);
            // 착지
            ChangePhase(JumpPhase.Recovery);
        }
    }

    private void UpdateRecovery()
    {
        if (_phaseTimer >= RecoveryDuration)
        {
            ChangePhase(JumpPhase.Finished);
            _boss.OnSkillFinished();
        }
    }
}