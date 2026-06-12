using UnityEngine;

public class ProjectileSpawnPattern : SpawnPattern
{
    private GameObject Owner = null;

    private ProjectileSpawnPattern(GameObject target)
    {
        Owner = target;
    }

    static public ProjectileSpawnPattern Create(GameObject target)
    {
        return new ProjectileSpawnPattern(target);
    }

    public override Vector3 GetPosition(Vector3 offset)
    {
        Vector2 offPos = Owner.transform.localScale * 0.5f;

        return Owner.transform.localPosition + (Vector3)offPos + offset;
    }
}
