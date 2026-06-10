using System.Collections.Generic;
using UnityEngine;

public class TestBound : MonoBehaviour
{
    [SerializeField] List<GameObject> WallList;
    List<Vector3> ScreenPoints = new List<Vector3>();

    private void Start()
    {
        int ScreenWidth = Screen.width;
        int ScreenHeight = Screen.height;

        ScreenPoints.Add(new Vector3(ScreenWidth * 0.5f, 0, 0));
        ScreenPoints.Add(new Vector3(ScreenWidth, ScreenHeight * 0.5f, 0));
        ScreenPoints.Add(new Vector3(ScreenWidth * 0.5f, ScreenHeight, 0));
        ScreenPoints.Add(new Vector3(0, ScreenHeight * 0.5f, 0));
    }

    private void Update()
    {
        Vector3 bl = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, 0));
        Vector3 tr = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0));

        for (int i = 0; i < WallList.Count; i++)
        {
            WallList[i].transform.position = Camera.main.ScreenToWorldPoint(ScreenPoints[i]);
            if(i % 2 == 0)
                WallList[i].transform.localScale = new Vector3((tr.x - bl.x), 0.2f, 0f);
            else
                WallList[i].transform.localScale = new Vector3(0.2f, (tr.y - bl.y), 0f);
        }
    }
}
