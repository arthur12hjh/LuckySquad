using UnityEngine;

[CreateAssetMenu(fileName = "MonsterRef", menuName = "Scriptable Objects/MonsterRef")]
public class MonsterRef : ObjectPoolRef
{
    [SerializeField] private float HP;
    [SerializeField] private float Power;
    [SerializeField] private float Speed;
}
