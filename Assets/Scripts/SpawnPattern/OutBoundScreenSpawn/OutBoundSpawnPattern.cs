using UnityEngine;

public class OutBoundSpawnPattern : SpawnPattern
{
    private Vector3 vOffset;

    private OutBoundSpawnPattern()
    {

    }

    static public OutBoundSpawnPattern Create(Vector3 Offset)
    {
        var obj = new OutBoundSpawnPattern();
        obj.vOffset = Offset;

        return obj;
    }

    public override Vector3 GetPosition(Vector3 offset)
    {
        Vector3 PlayerPos = InGameManager.Instance.GetPlayerTransform().position;
        Vector2 dir = Random.insideUnitCircle.normalized;
    
        return PlayerPos + (Vector3)(dir * Random.Range(vOffset.x, vOffset.y));
    }
}
