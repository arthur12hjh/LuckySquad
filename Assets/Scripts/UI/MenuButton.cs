using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.Controls;
using UnityEngine.UI;
using DG.Tweening;

public class MenuButton : MonoBehaviour
{
    [SerializeField] private Image buttonImage;
    [SerializeField] private Sprite normalSprite;
    [SerializeField] private Sprite selectedSprite;

    bool isPressed = false;

    private readonly float pressOffset = 30f;

    Toggle toggle; 
    RectTransform rect;

    public GameObject panel;
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
                panel.SetActive(true);
                buttonImage.sprite = selectedSprite;
            }

            isPressed = true;
        }
        else
        {
            if (isPressed == true)
            {
                rect.DOAnchorPos(new Vector2(rect.anchoredPosition.x, rect.anchoredPosition.y - pressOffset), 0.2f).SetEase(Ease.OutBack);
                panel.SetActive(false);
                buttonImage.sprite = normalSprite;
            }
            isPressed = false;
        }
    }

}
