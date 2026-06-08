using Item;
using UnityEngine;

public class game : MonoBehaviour
{
    public ObjectPoolRef prefab;

    // Start is called before the first frame update
    void Start()
    {
        DataManager Ins = DataManager.Instance;
       
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            var ob = ObjectPoolManager.Instance.Get(prefab) ;
            AttackHitBox HitBox = ob.GetComponent<AttackHitBox>();

            ob.SetActive(true);
            HitBox.Initialized(Vector2.zero, Vector2.one, AttackHitBox.HitBoxType.Circle);
        }
    }
}
