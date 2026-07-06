using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.U2D;

public class AddressablesManager : MonoBehaviour
{
    private static AddressablesManager instance;
    public static AddressablesManager Instance => instance;

    public event Action OnInitialized;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            StartCoroutine(InitFlow());
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public int Progress { get; private set; } = 1;

    // 구조체는 복사가 일어나서 클래스로 만듬
    private class CacheEntry<T>
    {
        // 실제 데이터
        public T value;
        // 메모리 해제를 위해서 필요함
        public AsyncOperationHandle<T> handle;
    }

    private class LabelCache<T>
    {
        public List<T> value;
        public AsyncOperationHandle<IList<T>> handle;
    }

    // 데이터를 캐싱해서 재사용하기 위함
    // 제네릭으로 데이터를 받고 이름이랑 결과로 저장한다
    // Hash를 이용함
    private Dictionary<Type, Dictionary<string, object>> cache = new();

    // Label로 한번에 받은 데이터를 캐싱해서 재사용하기 위함
    private Dictionary<string, object> labelCache = new();

    // 초기화
    private IEnumerator InitFlow()
    {
        yield return Addressables.InitializeAsync();
        OnInitialized?.Invoke();
    }

    // Addressables 에셋을 로드하는 함수
    // Key값으로 찾음
    public AsyncOperationHandle<T> LoadRoutine<T>(string key)
    {
        // 1. 캐시 체크
        // 만약 이미 로드가 된거라면 그냥 그거 찾아서  불러옴
        if (TryGetCache<T>(key, out T cached))
            return Addressables.ResourceManager.CreateCompletedOperation<T>(cached, null);

        // 2. 로딩 시작
        // 지금부터 비동기 시작
        var handle = Addressables.LoadAssetAsync<T>(key);

        // 3. 완료 후 캐싱

        handle.Completed += h =>
        {
            if (h.Status == AsyncOperationStatus.Succeeded)
            {
                SetCache<T>(key, h.Result, h);
            }
        };

        return handle;
    }

    // 캐시에 저장하는 함수
    private void SetCache<T>(string key, T value, AsyncOperationHandle<T> handle)
    {
        Type type = typeof(T);

        // 타입을 딕셔너리에 찾아서 없으면 추가
        if (!cache.ContainsKey(type))
            cache[type] = new Dictionary<string, object>();


        cache[type][key] = new CacheEntry<T>
        {
            value = value,
            handle = handle,
        };
    }


    public bool TryGetCache<T>(string key, out T value)
    {
        value = default;

        // 제네릭으로 받은 타입을 정의 해준다.
        Type type = typeof(T);

        // 만약 Dictionary가 없으면 함수를 종료한다.
        if (!cache.TryGetValue(type, out var dict))
            return false;

        // 만약 해당 타입의 캐시가 없으면 종료한다.
        if (!dict.TryGetValue(key, out var obj))
            return false;

        // 타입이 맞으면 반환
        if (obj is CacheEntry<T> entry)
        {
            value = entry.value;
            return true;
        }

        return false;
    }

    // 키값으로 해제
    public void Release<T>(string key)
    {
        // 제네릭 타입을 반환 해준다.
        Type type = typeof(T);

        // 실패시 종료
        if (!cache.TryGetValue(type, out var dict))
            return;

        if (!dict.TryGetValue(key, out var obj))
            return;

        // 제거
        if (obj is CacheEntry<T> entry)
        {
            Addressables.Release(entry.handle);
            dict.Remove(key);
        }

        if (dict.Count == 0)
            cache.Remove(type);
    }

    //label
    public AsyncOperationHandle<IList<T>> LoadLabel<T>(string label, string labelType)
    {
        // 캐시 키
        // label이랑 labelType을 합쳐서 키 값을 만든다.
        string cacheKey = $"{label}_{labelType}";

        // 1. 캐시 체크
        // 만약 이미 로드가 된거라면 Addressables를 호출 안함
        if (TryGetLabel(cacheKey, out List<T> cached))
            return Addressables.ResourceManager.CreateCompletedOperation<IList<T>>(cached, null);

        // 라벨 2개
        // Addressables에 label, labelType이 다 있는 걸 로드한다.
        List<object> labels = new List<object>()    
        {
            label,
            labelType
        };

        // 두 라벨 모두 포함된 Asset 로드
        var handle = Addressables.LoadAssetsAsync<T>(labels, null, Addressables.MergeMode.Intersection);

        // 캐시 저장
        handle.Completed += h =>
        {
            if (h.Status == AsyncOperationStatus.Succeeded)
            {
                labelCache[cacheKey] = new LabelCache<T>
                {
                    value = new List<T>(h.Result),
                    handle = h
                };
            }
        };

        return handle;
    }

    private bool TryGetLabel<T>(string key, out List<T> value)
    {
        value = null;

        // 존재하는지 확인 한다.
        if (!labelCache.TryGetValue(key, out var obj))
            return false;

        // 데이터 타입이 맞는지 확인 후 실제 데이터를 반환한다.
        if (obj is LabelCache<T> cache)
        {
            value = cache.value;
            return true;
        }

        return false;
    }

    // 캐시에 저장된 에셋 중 이름이 일치하는 에셋을 반환한다.
    // 사용 방법, label : Lobby, Stage 등 labelType : obj, img 등, assetName : 실제 객체이름
    // test1 = AddressablesManager.Instance.GetLabelObject<GameObject>("Logo","obj","MonsterTest");

    public T GetLabelObject<T>(string label, string labelType, string assetName)
            // name로 찾을 때 int형이나 그런건 name가 없기 때문에 제네릭에서 Object라고 고정을 해준다.
            where T : UnityEngine.Object
    {
        string key = $"{label}_{labelType}";

        // 해당 라벨이 아직 로드되지 않았다면 null 반환
        if (!TryGetLabel<T>(key, out var assets))
            return null;

        // 로드된 에셋 목록 중 이름이 일치하는 에셋 검색
        return assets.Find(x => x.name == assetName);
    }

    public List<T> GetLabelDictionary<T>(string label, string labelType)
    {
        // 키 값을 그대로 받는다.
        string key = $"{label}_{labelType}";

        // 해당 라벨이 아직 로드되지 않았다면 null 반환
        if (!labelCache.TryGetValue(key, out var obj))
            return null;

        // 현재 딕셔너리 타입을 그대록 가지고 온다.
        if (obj is LabelCache<T> cache)
            return cache.value;

        return null;
    }

    public void ReleaseStage(string stageName)
    {
        ReleaseLabel<GameObject>(stageName, "obj");
        ReleaseLabel<AudioClip>(stageName, "sound");
        ReleaseLabel<Sprite>(stageName, "img");
        ReleaseLabel<SpriteAtlas>(stageName, "imgAtlas");
        ReleaseLabel<StageRef>(stageName, "ref");
    }

    // 데이터 해제
    private void ReleaseLabel<T>(string label, string labelType)
    {
        string cacheKey = $"{label}_{labelType}";

        // 실패시 반환
        if (!labelCache.TryGetValue(cacheKey, out var obj))
            return;

        // Addressables 메모리 해제
        // 캐시 제거
        if (obj is LabelCache<T> cache)
        {
            Addressables.Release(cache.handle);
            labelCache.Remove(cacheKey);
        }
    }

}