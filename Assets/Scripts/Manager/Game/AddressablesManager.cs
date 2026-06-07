using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.Jobs;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AdaptivePerformance;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

public class AddressablesManager : MonoBehaviour
{
    private static AddressablesManager instance = null;

    public static AddressablesManager Instance
    {
        get { return instance; }
    }

    public event Action OnInitialized;

    // 오디오
    //[SerializeField]
    //private AssetReferenceT<AudioClip> sound;

    // 이미지
    //[SerializeField]
    //private AssetReferenceSprite flagSprite;

    // 오브젝트 해제용
    //[SerializeField] AssetReferenceGameObject[] objs;
    private List<GameObject> spawned = new List<GameObject>();

    [SerializeField]
    private GameObject BGMObj;
    [SerializeField]
    private Image Image;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;

            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Init()
    {
        // 코루틴을 사용해서 어드레서블 초기화 하고 스폰까지
        StartCoroutine(InitFlow());
    }

    IEnumerator InitFlow()
    {
        // 초기화
        yield return Addressables.InitializeAsync();

        yield return Spawn();

        OnInitialized?.Invoke();
    }

    // 객체 단일
    //IEnumerator Spawn()
    //{
    //    if (objs == null || objs.Length == 0)
    //        yield break;

    //    int remaining = objs.Length;

    //    foreach (var objRef in objs)
    //    {

    //        if (objRef == null)
    //        {
    //            remaining--;
    //            continue;
    //        }

    //        objRef.InstantiateAsync().Completed += handle =>
    //        {
    //            if (handle.Status == AsyncOperationStatus.Succeeded)
    //            {
    //                spawned.Add(handle.Result);
    //            }

    //            remaining--;
    //        };
    //    }

    //    while (remaining > 0)
    //        yield return null;
    //}

    // 오브젝트를 해제해야 할 때 사용할 함수

    IEnumerator Spawn()
    {
        var handle = Addressables.LoadAssetsAsync<GameObject>("Test", null);
        yield return handle;

        foreach (var prefab in handle.Result)
        {
            GameObject obj = Instantiate(prefab);
            spawned.Add(obj);
        }
    }

    private void ReleaseObj()
    {
        foreach (var obj in spawned)
        {
            Addressables.ReleaseInstance(obj);
        }
        spawned.Clear();
    }
}
