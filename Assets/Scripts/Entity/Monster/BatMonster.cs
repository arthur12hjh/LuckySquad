using UnityEngine;

// 박쥐는 살짝 다르게, Move를 _playerTransform 추적이 아니라
// 상태 전환 시점에 고정된 진행 방향으로 이동할 것
public class BatMonster : Monster
{
    [SerializeField] private Vector2 _moveDir = default;

    // 박쥐는 플레이어를 추적하지 않고 고정된 _moveDir 방향으로 직진
    protected override void Move(Vector3 position, float speedModifier = 1f)
    {
        Vector2 currentPos = _rigidbody2D.position;
        Vector2 targetPos = currentPos + _moveDir * (Speed * speedModifier * Time.fixedDeltaTime);
        _rigidbody2D.MovePosition(targetPos);

        _animator.SetBool(_isMoveID, true);
        _animator.SetFloat(_directionID, _playerTransform.position.x - transform.position.x);
    }

    protected override void OnStateChanged(MonsterState newState)
    {
        _moveDir = Vector2.Normalize(_playerTransform.position - transform.position);
    }
}