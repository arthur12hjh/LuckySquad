using System;
using UnityEngine;

public class AttackHitBox : MonoBehaviour
{
    public enum     HitBoxType  { Capsule, Circle, Box, END};
    public          HitBoxType  HitType { get { return Type; } }
    public          Collider2D  Collider { get { return HitCollider; } }

    [SerializeField] private HitBoxType Type = HitBoxType.END;
    [SerializeField] private Boolean    Trigger = false;

    Collider2D       HitCollider = null;

    public void Initialized(Vector2 Center, Vector2 Size, HitBoxType HitType = HitBoxType.Box)
    {
        Type = HitType;
        AddHitComponent();

        if (HitCollider != null)
        {
            HitCollider.transform.position = Center;

            switch (Type)
            {
                case HitBoxType.Circle:
                    {
                        CircleCollider2D Col = HitCollider as CircleCollider2D;
                        if(Col != null)
                        {
                            Col.radius = Size.x;
                        }
                    }
                    break;
                case HitBoxType.Box:
                    {
                        BoxCollider2D Col = HitCollider as BoxCollider2D;
                        if (Col != null)
                        {
                            Col.size = Size;
                        }
                    }
                    break;
                case HitBoxType.Capsule:
                    {
                        CapsuleCollider2D Col = HitCollider as CapsuleCollider2D;
                        if (Col != null)
                        {
                            Col.size = Size;
                        }
                    }
                    break;

                default:
                    Debug.Log("Not Select Hit Type");
                    break;
            }
        }
            
    }

    [ContextMenu("Generate")]
    void AddHitComponent()
    {
        if (HitCollider != null)
        {
            if((Type == HitBoxType.Circle && HitCollider is CircleCollider2D) ||
               (Type == HitBoxType.Box && HitCollider is BoxCollider2D) ||
               (Type == HitBoxType.Capsule && HitCollider is CapsuleCollider2D))
            { 
                return;
            }
        }

        if (HitCollider != null)
        {
#if UNITY_EDITOR
            DestroyImmediate(HitCollider);
#else
            Destory(HitCollider);
#endif
            HitCollider = null;
        }

        switch (Type)
        {
            case HitBoxType.Circle:
                HitCollider = gameObject.AddComponent<CircleCollider2D>();
                break;
            case HitBoxType.Box:
                HitCollider = gameObject.AddComponent<BoxCollider2D>();
                break;
            case HitBoxType.Capsule:
                HitCollider = gameObject.AddComponent<CapsuleCollider2D>();
                break;

            default:
                Debug.Log("Not Select Hit Type");
                break;
        }

        if(HitCollider != null)
        {
            HitCollider.isTrigger = Trigger;
        }
    }
}
