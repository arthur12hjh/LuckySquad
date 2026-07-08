using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "StageRef", menuName = "Scriptable Objects/StageRef")]
public class StageRef : ScriptableObject
{
    [SerializeField] public int StageIndex;                         // 스테이지
    [SerializeField] public int WaveIndex;                          // 몇 웨이브
    [SerializeField] public WaveData[] WaveDatas;                    // 웨이브 데이터
    [SerializeField] public List<GameObject> MonsterSpawnDate;      // 스테이지에서 추가적으로 나올 아이템 데이터
    [SerializeField] public List<int>        RandomItemDatas;      // 몬스터 종류
    [SerializeField] public List<GameObject> Boss;                  // 보스 몬스터
    [SerializeField] public float StageTime;                        // 스테이지 시간
}
