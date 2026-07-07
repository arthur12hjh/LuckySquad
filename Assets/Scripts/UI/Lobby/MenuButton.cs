using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.Controls;
using UnityEngine.UI;
using static Enums;

public class MenuButton : MonoBehaviour
{
    [SerializeField] private Sprite normalSprite;
    [SerializeField] private Sprite selectedSprite;
    [SerializeField] private LobbyScreen sceneType;

    bool isPressed = false;

    private readonly float pressOffset = 30f;

    Toggle toggle; 
    RectTransform rect;

    void Start()
    {
        rect = GetComponent<RectTransform>();
        toggle = GetComponent<Toggle>();
        if(toggle.isOn)
        {
            toggle.Select();
            ToggleEvent();
        }
    }
  
    public void ToggleEvent()
    {
        if(toggle.isOn)
        {
            if(isPressed == false)
            {   
                rect.DOAnchorPos(new Vector2(rect.anchoredPosition.x, rect.anchoredPosition.y + pressOffset), 0.2f).SetEase(Ease.OutBack);
                LobbyManager.Instance.ScreenChage(sceneType);
            }

            isPressed = true;
        }
        else
        {
            if (isPressed == true)
            {
                rect.DOAnchorPos(new Vector2(rect.anchoredPosition.x, rect.anchoredPosition.y - pressOffset), 0.2f).SetEase(Ease.OutBack);
            }
            isPressed = false;
        }
    }

}
