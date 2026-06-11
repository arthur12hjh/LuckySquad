using UnityEngine;

public class Test : MonoBehaviour
{

    public GameObject test1;

    void Start()
    {
        var handle = AddressablesManager.Instance.LoadLabel<GameObject>("Logo", "obj");

        handle.Completed += _ =>
        {
            test1 = AddressablesManager.Instance.GetLabelObject<GameObject>("Logo","obj","MonsterTest");

            Debug.Log(test1);
        };
    }

    void Update()
    {
        
    }
}
