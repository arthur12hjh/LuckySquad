using UnityEngine;

[CreateAssetMenu(fileName = "ObjectPoolRef", menuName = "Scriptable Objects/ObjectPoolRef")]
public class ObjectPoolRef : ScriptableObject
{
    [SerializeField] public GameObject prefab;
    [SerializeField] public ScriptableObject initRef;
    [SerializeField] public int initializePoolSize;
}
