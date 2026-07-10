using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class ObjectPoolManager : MonoBehaviour
{
    public static ObjectPoolManager Instance { get; private set; }
    public bool IsReady { get; private set; }

    [SerializeField] private List<ObjectPoolRef> objectPoolRefs;            // ?°ì´??ë¦¬ìŠ¤??

    private readonly Dictionary<ObjectPoolRef, ObjectPool<GameObject>> _poolDictionary = new(); // ?€ë§ìœ¼ë¡?ë§Œë“¤?´ì§„ ?ˆë¹„ê°ì²´??
    private readonly Dictionary<ObjectPoolRef, GameObject> _prefabPool = new(); // ?´ë‹¹ SO???„ë¦¬??
    private readonly Dictionary<ObjectPoolRef, AsyncOperationHandle<GameObject>> _asyncOperationHandles = new(); // ?´ë‹¹ SO???´ë“œ?ˆì„œë¸??¸ë“¤

    private void Awake()
    {
        if (Instance != null
            && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        // foreach (var refSO in objectPoolRefs)
        // {
        //     var parent = new GameObject($"{refSO.name}Pool").transform;
        //     parent.SetParent(transform);
        //     _poolDictionary[refSO] = CreatePool(refSO, parent);
        // }
    }

    private async void Start()
    {
        await InitializeAsync();
        IsReady = true;
    }

    private async Task InitializeAsync()
    {
        foreach (var refSO in objectPoolRefs)
        {
            // ë¹„ë™ê¸??‘ì—… ?¸ë“¤. ??ì¢€ ?¤ë˜ê±¸ë¦¬?ˆê¹Œ ë¹„ë™ê¸°ë¡œ ?”ê²Œ
            AsyncOperationHandle<GameObject> handle = refSO.prefab.LoadAssetAsync<GameObject>();
            
            // Load Asset ?ë‚  ?Œê¹Œì§€ ê¸°ë‹¤ë¦¬ê¸°
            await handle.Task;

            if (handle.Status != AsyncOperationStatus.Succeeded)
            {
                Debug.LogError($"Error : Object Pool Ref {refSO.poolName} cannot be loaded");
                continue; // ?¼ì´ë¸?ë¹Œë“œ?ì„œ??ê·¸ë˜???¼ë‹¨ ?¤í–‰?€ ?¼ì•¼?˜ë‹ˆê°€...
            }

            _asyncOperationHandles[refSO] = handle;
            _prefabPool[refSO] = handle.Result;
            
            var parent = new GameObject($"{refSO.name}Pool").transform;
            parent.SetParent(transform);
            _poolDictionary[refSO] = CreatePool(refSO, parent);
        }
    }
    
    
    private ObjectPool<GameObject> CreatePool(ObjectPoolRef refSO, Transform parent)
    {
        ObjectPool<GameObject> pool = null;
        GameObject prefab = _prefabPool[refSO];
        pool = new ObjectPool<GameObject>(
            createFunc: () =>
            { 
                var obj =  Instantiate(prefab, parent);
                if(obj.TryGetComponent<IPoolable>(out IPoolable poolable))
                    poolable.OnSpawn(() => pool.Release(obj));
                return obj;
            },            // ?ì„± ë°©ì‹
            actionOnGet: obj => obj.SetActive(true),              // Get. ?¸ê²Œ???„ë“œë¡?ë¶ˆëŸ¬????ë°©ì‹
            actionOnRelease: obj  => obj.SetActive(false),        // Release. ?„ë“œ?ì„œ ?´íƒˆ????ë°©ì‹
            actionOnDestroy: obj => Destroy(obj),                 // Destroy. ?„ì˜ˆ ?? œ????ë°©ì‹
            collectionCheck: false,                                         // Release?????€???¤ì–´ê°€?ˆëŠ” ?¤ë¸Œ?íŠ¸?¸ì? ì²´í¬.
            defaultCapacity: refSO.initializePoolSize,                      // ì²˜ìŒ ?ì„±??ê°ì²´??
            maxSize: refSO.initializePoolSize * 2                           // ìµœë? ?í•œ??
            );
        Prewarm(pool, refSO.initializePoolSize);

        return pool;
    }

    // ?¬ì „ ?ì„±. defaultCapacityë§Œí¼ ë¯¸ë¦¬ ë§Œë“¤?´ë‘ê¸?
    private void Prewarm(ObjectPool<GameObject> pool, int iSize)
    {
        var tempObjectList = new GameObject[iSize];
        for (int i = 0; i < iSize; ++i)
            tempObjectList[i] = pool.Get();
        for (int i = 0; i < iSize; ++i)
            pool.Release(tempObjectList[i]);
    }

    public GameObject Get(ObjectPoolRef refSO) => _poolDictionary[refSO].Get();
    public void Release(ObjectPoolRef refSO, GameObject obj) => _poolDictionary[refSO].Release(obj);

    public bool Clear(ObjectPoolRef refSO)
    {
    //     ë¹„ì›Œ?¼í•˜?”ê±°
    // private readonly Dictionary<ObjectPoolRef, ObjectPool<GameObject>> _poolDictionary = new(); // ?€ë§ìœ¼ë¡?ë§Œë“¤?´ì§„ ?ˆë¹„ê°ì²´??
    // private readonly Dictionary<ObjectPoolRef, GameObject> _prefabPool = new(); // ?´ë‹¹ SO???„ë¦¬??
    // private readonly Dictionary<ObjectPoolRef, AsyncOperationHandle<GameObject>> _asyncOperationHandles = new(); // ?´ë‹¹ SO???´ë“œ?ˆì„œë¸??¸ë“¤
    
        // ì§€?Œì¤˜?¼í•  ê²?: ObjectPool<GameObject>, ScriptableObject, handle, ScriptableObject.AssetReferenceGameObject
        // 1. ë¨¼ì? ObjectPool<GameObject>ë¥?ë¹„ìš°??(?ˆë¹„ ê°ì²´ ëª¨ìŒ)
        if (!_poolDictionary.TryGetValue(refSO, out var pool))
            return false;
        
        pool.Clear();
        
        // 2. ?´ì œ ScriptableObject ê¸°ë°˜?¼ë¡œ Addressable ?´ì?, handle, ?„ë¦¬??ëª¨ìŒ, ObjectPool ëª¨ìŒ?ì„œ ?´ë‹¹ ê°’ì„ ë¹¼ì¤˜?¼í•œ??
        if(_asyncOperationHandles.TryGetValue(refSO, out var handle) && handle.IsValid())
            refSO.prefab.ReleaseAsset();
        
        _poolDictionary.Remove(refSO);
        _asyncOperationHandles.Remove(refSO);
        _prefabPool.Remove(refSO);
        
        return true;
    }

    // Addressable ê¸°ë°˜?´ê¸°?? ?´ì œë¥??´ì¤˜?¼í•œ??
    private void OnDestroy()
    {
        // Dispose => Clear?¸ë° C#?ì„œ ì´ˆê¸°?”ë?”ì? ì»´íŒŒ???€?„ì— ?ë™ ì¶”ì ?´ì¤Œ.
        foreach(var pool in _poolDictionary.Values)
            pool.Dispose();
        _poolDictionary.Clear();

        foreach (var refSO in _asyncOperationHandles.Keys)
        {
            if (refSO.prefab.IsValid())
                refSO.prefab.ReleaseAsset();
        }
        
        _asyncOperationHandles.Clear();
        _prefabPool.Clear();
        
        if(Instance == this) Instance = null;
    }
    
    
}