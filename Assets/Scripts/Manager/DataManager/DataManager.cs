using UnityEngine;
using Newtonsoft.Json;
using System.Collections.Generic;
using Item;
using System.Linq;
using System;

[CreateAssetMenu(fileName = "Manager", menuName = "Scriptable Objects/DataManager/DataManager", order = 1)]
public class DataManager : ScriptableObject
{
    public string                               JsonUrl;
    private Dictionary<int, ItemData> Items = new Dictionary<int, ItemData>();

    [SerializeField] private List<WeaponSO>             _weaponPrefab;
    private Dictionary<Item.EWeaponType, GameObject>    WeaponPrefabs;

    [SerializeField] private List<EfffectRef>    _Effects = new List<EfffectRef>();
    private Dictionary<int, ObjectPoolRef>       _EffectSO = new Dictionary<int, ObjectPoolRef>();
    #region Default

    public ItemData FindItemData(int id)
    {
        if(Items.TryGetValue(id, out var item))
            return item;
       
        return null;
    }

    public GameObject GetWeaponPrefab(Item.EWeaponType Type)
    {
        if (WeaponPrefabs.TryGetValue(Type, out var item))
            return item;

        return null;
    }

    public ObjectPoolRef FindEffectSO(int id)
    {
        if (_EffectSO.TryGetValue(id, out var EffectSO))
            return EffectSO;

        return null;
    }

    public ItemData GetRandomItemData()
    {
        if (Items == null || Items.Count == 0)
            return null;

        int index = UnityEngine.Random.Range(0, Items.Count);
        return Items.Values.ElementAt(index);
    }

    private bool Initialize()
    {
        if (LoadJsonFile() == false)
            return false;

        _EffectSO = _Effects.ToDictionary(x => x.EffectID, x => (ObjectPoolRef)x);
        WeaponPrefabs = _weaponPrefab.ToDictionary(x => x.Type, x => x.prefab);
        return true;
    }

    private bool LoadJsonFile()
    {
        TextAsset jsonAsset = Resources.Load<TextAsset>(JsonUrl);

        if (jsonAsset != null)
        {
            var settings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All // Ÿ�� ������ JSON�� ���Խ�Ŵ
            };

            string jsonString = jsonAsset.text;
            List<ItemData> items = JsonConvert.DeserializeObject<List<ItemData>>(jsonString, settings);
            Items = items.ToDictionary((info => info.iID));
        }

        return true;
    }

    private static DataManager instance = null;
    public static DataManager Instance {
        get
        {
            if(instance == null)
            {
                instance = Resources.Load<DataManager>("Manager/DataManager"); ;
                if(instance.Initialize() == false)
                {
                    Debug.Log("Log : Initalize Fail (Data Manager)");
                }
            }
            return instance;
        }
    }

    private DataManager() { }
    #endregion
}
