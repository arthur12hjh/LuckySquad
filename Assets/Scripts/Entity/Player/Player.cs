using UnityEngine;

public class Player : BaseEntity
{
    
    [SerializeField] private Animator _animator;
    
    private static readonly int _isMoveID = Animator.StringToHash("isMove");
    private static readonly int _directionID = Animator.StringToHash("Direction");
    
    public override void Initialize(ScriptableObject initRef)
    {
    }

    protected override void Move(Vector3 position, float speedModifier = 1f)
    {
        
    }

    void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    public void UpdatePlayer(Vector2 inputVec)
    {
        _animator.SetBool(_isMoveID, !Mathf.Approximately(inputVec.magnitude, 0f));
        
        if (!Mathf.Approximately(inputVec.x, 0f))
        {
            _animator.SetFloat(_directionID, inputVec.x);
        }
    }
    
}
