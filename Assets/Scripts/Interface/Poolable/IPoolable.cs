using System;

public interface IPoolable
{
    void OnSpawn(Action releaseSelf);
}
