using UnityEngine;

public class UtilitySystem
{
    public static Vector3 GetRandomWorldPoint(Camera cam)
    {
        float distance = Mathf.Abs(cam.transform.position.z);

        return cam.ViewportToWorldPoint(new Vector3(
                Random.value,
                Random.value,
                distance));
    }
}
