using UnityEngine;

public class Player : BaseEntity
{
    
    [SerializeField] private Animator _animator;
    
    private static readonly int _isMoveID = Animator.StringToHash("isMove");
    private static readonly int _directionID = Animator.StringToHash("Direction");

    // Monster Layer 캐싱. Layer 비교로 충돌 대상을 판단한다
    private int _monsterLayer;

    public override void Initialize(ScriptableObject initRef)
    {
    }

    protected override void Move(Vector3 position, float speedModifier = 1f)
    {
        
    }

    void Awake()
    {
        _animator = GetComponent<Animator>();
        _monsterLayer = LayerMask.NameToLayer("Monster");
    }

    public void UpdatePlayer(Vector2 inputVec)
    {
        _animator.SetBool(_isMoveID, !Mathf.Approximately(inputVec.magnitude, 0f));

        if (!Mathf.Approximately(inputVec.x, 0f))
        {
            _animator.SetFloat(_directionID, inputVec.x);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 충돌 대상이 Monster Layer인 경우에만 데미지 처리
        if (other.gameObject.layer != _monsterLayer)
        {
            return;
        }

        // MonsterRef의 Power만큼 플레이어가 데미지를 받는다
        Monster monster = other.GetComponent<Monster>();
        if (monster is null)
        {
            return;
        }

        TakeDamage(monster.Power);
    }

    private void TakeDamage(float damage)
    {
        // 실제 플레이어 HP는 PlayerStats가 관리하므로 InGameManager를 통해 접근한다
        if (InGameManager.Instance is null)
        {
            return;
        }

        PlayerStats stats = InGameManager.Instance.GetPlayerStats();
        if (stats is null)
        {
            return;
        }

        float nextHp = stats.CurrentHp - damage;
        Debug.Log($"Damaged : {nextHp}");
        if (nextHp < 0f)
        {
            nextHp = 0f;
        }

        stats.CurrentHp = nextHp;
    }

}
