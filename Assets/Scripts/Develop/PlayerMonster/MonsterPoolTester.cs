using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class MonsterPoolTester : MonoBehaviour
{
    [SerializeField] private ObjectPoolRef objRef;
    [SerializeField] private ObjectPoolRef batRef;

    [SerializeField] private Vector2 Range_x = new Vector2(-7.5f, 7.8f);
    [SerializeField] private Vector2 Range_y = new Vector2(-3.3f, 2.7f);

    private bool atOnce = false;
    [SerializeField] private int count = 0;

    private float _timer;

    private void Start()
    {
    }

    private void Update()
    {
        if (atOnce == false)
        {
            for (int i = 0; i < 30; ++i)
            {
                var monster = ObjectPoolManager.Instance.Get(batRef);
                monster.transform.position = new Vector3(Random.Range(Range_x.x, Range_x.y), Random.Range(Range_y.x, Range_y.y), 0f);
                monster.GetComponent<BaseEntity>().Initialize(batRef.initRef);
            }
            atOnce = true;
        }
        
        _timer += Time.deltaTime;
        if(_timer > 0.1f)
        {
            for (int i = 0; i < 10; ++i)
            {
                var monster = ObjectPoolManager.Instance.Get(objRef);
                monster.transform.position = new Vector3(Random.Range(Range_x.x, Range_x.y), Random.Range(Range_y.x, Range_y.y), 0f);
                monster.GetComponent<BaseEntity>().Initialize(objRef.initRef);
                count++;
            }
            _timer = 0f;

        }
    }
}
