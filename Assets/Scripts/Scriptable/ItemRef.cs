using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Item Ref", menuName = "Scriptable Objects/Item/Item Ref")]
public class ItemRef : ScriptableObject
{
    [SerializeField] private int ID;
    [SerializeField] private List<Sprite>  Icons = new List<Sprite>();
}
