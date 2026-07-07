using UnityEngine;
using UnityEngine.AddressableAssets;

[CreateAssetMenu(fileName = "ObjectPoolRef", menuName = "Scriptable Objects/ObjectPoolRef")]
public class ObjectPoolRef : ScriptableObject
{
    public string poolName;
    [SerializeField] public AssetReferenceGameObject prefab;
    [SerializeField] public ScriptableObject initRef;
    [SerializeField] public int initializePoolSize;
}
