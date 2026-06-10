using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using static UnityEngine.Rendering.DebugUI;

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
    private Dictionary<Type, Dictionary<string, object>> cache
        = new Dictionary<Type, Dictionary<string, object>>();

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



    // 모든 객체를 다 지운다.
    public void ReleaseAll()
    {
        foreach (var typeDict in cache.Values)
        {
            foreach (var obj in typeDict.Values)
            {
                if (obj is CacheEntry<object> entry)
                {
                    Addressables.Release(entry.handle);
                }
            }
        }

        cache.Clear();
    }

    //label
    public AsyncOperationHandle<IList<T>> LoadLabel<T>(string label, string labelType)
    {
        // 캐시 키
        string cacheKey = $"{label}_{labelType}";

        // 캐시 체크
        if (TryGetLabel(cacheKey, out List<T> cached))
        {
            return Addressables.ResourceManager.CreateCompletedOperation<IList<T>>(
                cached,
                null
            );
        }

        // 라벨 2개
        List<object> labels = new List<object>()
        {
            label,
            labelType
        };

        // 두 라벨 모두 포함된 Asset 로드
        var handle = Addressables.LoadAssetsAsync<T>(
            labels,
            null,
            Addressables.MergeMode.Intersection
        );

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

    public bool TryGetLabel<T>(string key, out List<T> value)
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

    // 데이터 해제
    public void ReleaseLabel(string label)
    {
        // 실패시 반환
        if (!labelCache.TryGetValue(label, out var cache))
            return;

        // Addressables 메모리 해제
        Addressables.Release(cache);

        // 캐시 제거
        labelCache.Remove(label);
    }
}