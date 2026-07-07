using UnityEngine;

[CreateAssetMenu(fileName = "weaponSO", menuName = "Scriptable Objects/weaponSO")]
public class WeaponSO : ScriptableObject
{
    // 카드 선택용 데이터
    // 등급 : Normal < Rare < Epic < Unique < Legendary

    public Item.EWeaponType Type;
    public GameObject prefab;
}
