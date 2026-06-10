using UnityEngine;

public abstract class EntityRef : ScriptableObject
{
    [SerializeField] public float HP;
    [SerializeField] public float Power;
    [SerializeField] public float Speed;
}
