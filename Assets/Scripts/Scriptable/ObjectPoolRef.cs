using UnityEngine;

[CreateAssetMenu(fileName = "ObjectPoolRef", menuName = "Scriptable Objects/ObjectPoolRef")]
public class ObjectPoolRef : ScriptableObject
{
    [SerializeField] public GameObject prefab;
    [SerializeField] public int initializePoolSize;
}
