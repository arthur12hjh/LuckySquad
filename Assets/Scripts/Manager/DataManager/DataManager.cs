using UnityEngine;
using Newtonsoft.Json;
using System.Collections.Generic;
using Item;
using System.Linq;

[CreateAssetMenu(fileName = "Manager", menuName = "Scriptable Objects/DataManager/DataManager", order = 1)]
public class DataManager : ScriptableObject
{
    public string                               JsonUrl;
    [SerializeField] private Dictionary<int, ItemData> Items = new Dictionary<int, ItemData>();

    [SerializeField] private List<EfffectRef>    _Effects = new List<EfffectRef>();
    private Dictionary<int, ObjectPoolRef>       _EffectSO = new Dictionary<int, ObjectPoolRef>();
    #region Default

    public ItemData FindItemData(int id)
    {
        if(Items.TryGetValue(id, out var item))
            return item;

       
        return null;
    }

    public ObjectPoolRef FindEffectSO(int id)
    {
        if (_EffectSO.TryGetValue(id, out var EffectSO))
            return EffectSO;

        return null;
    }

    private bool Initialize()
    {
        if (LoadJsonFile() == false)
            return false;

        _EffectSO = _Effects.ToDictionary(x => x.EffectID, x => (ObjectPoolRef)x);
        return true;
    }

    private bool LoadJsonFile()
    {
        TextAsset jsonAsset = Resources.Load<TextAsset>(JsonUrl);

        if (jsonAsset != null)
        {
            var settings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All // 타입 정보를 JSON에 포함시킴
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
