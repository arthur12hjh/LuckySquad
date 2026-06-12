using UnityEngine;
using System.Collections.Generic;

/*
 * 웨이브 시스템

   ScriptableObject SpawnEvent
   - 웨이브 단일 데이터를 보관한다.
   - 파티클 이펙트와 비슷한 느낌
   - Get할 Pool Object의 Key값
   - 몇 초부터 몇 초까지를 정한다.
   - 그리고 그 사이 주기를 정한다.
   - 한 번에 몇 마리를 내보낼지 정한다
   - enum으로 단일 보스웨이브, 몬스터 웨이브 등 설정한다

   ScriptableObject WaveData
   - 한 스테이지의 모든 웨이브 정보를 담는다
   - List로 SpawnEvent를 담고 있는다
 */
[CreateAssetMenu(fileName = "WaveData", menuName = "Scriptable Objects/WaveData")]
public class WaveData : ScriptableObject
{
    [SerializeField] public List<SpawnEvent> spawnEvents;
}
